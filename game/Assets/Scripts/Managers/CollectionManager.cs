using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using GameSparks.Api.Responses;

[System.Serializable]
public class CollectionManager : MonoBehaviour
{
    private const int REQUIRED_DECK_SIZE = 30;
    private const float TWEEN_TIME = 0.5f;

    [SerializeField]
    private List<CollectionCardObject> collection;

    [SerializeField]
    private List<Card> includedCards;  //list that is currently rendered/being changed
    public List<Card> IncludedCards => includedCards;
    private int activeDeck;

    [SerializeField]
    private List<Button> deckButtons;
    [SerializeField]
    private GameObject deckSelection;
    [SerializeField]
    private GameObject rightSidebar;
    [SerializeField]
    private GameObject cutoutContent;
    [SerializeField]
    private GameObject collectionObject;

    [SerializeField]
    private Text activeDeckName;
    [SerializeField]
    private Text deckSize;

    private CollectionCardObject selectedCard;
    private LogEventResponse cardsResponse;

    public bool editMode = false;

    [SerializeField]
    private GraphicRaycaster m_Raycaster;
    [SerializeField]
    private EventSystem m_EventSystem;
    private PointerEventData m_PointerEventData;

    [SerializeField]
    private GameObject creatureCardObjectPrefab;
    [SerializeField]
    private GameObject spellCardObjectPrefab;
    [SerializeField]
    private GameObject structureCardObjectPrefab;
    [SerializeField]
    private GameObject weaponCardObjectPrefab;

    private Dictionary<Card.CardType, Stack<CollectionCardObject>> collectionCardObjectPools;

    private Stack<CardCutout> cardCutoutPool;
    private List<CardCutout> sortedCardCutouts;

    [SerializeField]
    private GameObject cardCutoutPrefab;

    public static CollectionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.collection = new List<CollectionCardObject>();
        this.includedCards = new List<Card>();
        this.sortedCardCutouts = new List<CardCutout>();

        InitializeCollectionCardObjectPools();
    }

    private void Start()
    {
        DeckStore.Instance().GetDecksWithCallback(Callback);
    }

    private void Callback()
    {
        UpdateDecks();

        //assign button event listeners now and forever
        for (int i = 0; i < deckButtons.Count; ++i)
        {
            int deckId = i;
            deckButtons[i].onClick.AddListener(new UnityAction(() => EnterEditMode(deckId)));
        }

        List<Card> cards = DeckStore.Instance().GetCards();
        this.collection = CreateCardObjects(cards);
    }

    private void UpdateDecks()
    {
        List<string> decks = DeckStore.Instance().GetDeckNames();

        for (int i = 0; i < deckButtons.Count; ++i)
        {
            if (i < decks.Count)
            {
                //set names if deck exists
                deckButtons[i].GetComponentInChildren<Text>().text = decks[i];
            }
            else
            {
                decks.Add(String.Format("Deck {0}", i));
            }
        }
    }

    private void Update()
    {
        CastGraphicsRaycast();
    }

    private void CastGraphicsRaycast()
    {
        //Set up the new Pointer Event, Set the Pointer Event Position to that of the mouse position
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results, raycast using the Graphics Raycaster and mouse click position
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        if (!CheckForDecklist(results))
        {
            if (editMode)
            {
                ScrollToVerticalPan();
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && ActionManager.Instance.HasDragTarget())
            {
                CollectionCardObject cardObject = ActionManager.Instance.GetDragTarget() as CollectionCardObject;
                IncludeCard(cardObject);
            }
        }
    }

    private bool CheckForDecklist(List<RaycastResult> hit)
    {
        foreach (RaycastResult element in hit)
        {
            if (element.gameObject != rightSidebar)
                continue;

            return true;
        }
        return false;
    }

    public void OnSaveButton()
    {
        SaveCollectionRequest();
    }

    private void SaveCollectionRequest()
    {
        List<string> cardIds = new List<string>();
        foreach (Card elem in this.includedCards)
        {
            cardIds.Add(elem.Id);
        }
        string previousName = DeckStore.Instance().GetDeckNames()[activeDeck];
        string newName = activeDeckName.text;

        DeckStore.Instance().CreateUpdatePlayerDeckWithCallback(
            cardIds,
            previousName,
            newName,
            SaveCallback
        );
    }

    private void SaveCallback()
    {
        ExitEditMode();
    }

    private void ScrollToVerticalPan()
    {
        float vertical = Mathf.Clamp(collectionObject.transform.localPosition.y + Input.mouseScrollDelta.y * -0.1F, -0.5f, 100);
        collectionObject.transform.localPosition = new Vector3(collectionObject.transform.localPosition.x, vertical, collectionObject.transform.localPosition.z);
    }

    private void EnterEditMode(int deckId)
    {
        LoadDecklist(deckId);

        LeanTween.moveLocalX(deckSelection, -800, TWEEN_TIME).setEaseOutQuad();
        LeanTween.moveLocalX(rightSidebar, 320, TWEEN_TIME).setEaseInQuad();
        LeanTween
            .moveY(collectionObject, -0.5f, TWEEN_TIME)
            .setEaseInQuad()
            .setOnComplete(() =>
            {
                this.editMode = true;
                this.activeDeck = deckId;

                InputField deckNameField = rightSidebar.GetComponentInChildren<InputField>();
                deckNameField.text = DeckStore.Instance().GetDeckNames()[deckId];
            });
    }

    private void ExitEditMode()
    {
        UpdateDecks();
        foreach (CardCutout cardCutout in new List<CardCutout>(this.sortedCardCutouts))
        {
            RemoveCard(cardCutout);
        }

        this.editMode = false;
        LeanTween.moveLocalX(rightSidebar, 480, TWEEN_TIME).setEaseInQuad();
        LeanTween
            .moveY(collectionObject, -10, TWEEN_TIME)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {

            });
        LeanTween.moveLocalX(deckSelection, 0, TWEEN_TIME).setEaseOutQuad();
    }

    private void LoadDecklist(int deckId)
    {
        this.sortedCardCutouts = new List<CardCutout>();
        foreach (Card card in DeckStore.Instance().GetCardsByDeckName(DeckStore.Instance().GetDeckNames()[deckId]))
        {
            IncludeCard(card.wrapper as CollectionCardObject);
        }
    }

    private List<CollectionCardObject> CreateCardObjects(List<Card> cards)
    {
        int index = 0;
        int rowSize = 4;

        Vector3 topLeft = new Vector3(-5.95f, 3.11f, 0);
        Vector3 horizontalOffset = new Vector3(2.9f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.95f, 0f);

        Transform grayed = new GameObject("Grayed").transform as Transform;
        grayed.parent = this.collectionObject.transform;

        List<CollectionCardObject> createdCardObjects = new List<CollectionCardObject>();

        foreach (Card card in cards)
        {
            CollectionCardObject collectionCardObject = InitializeCollectionCardObject(card);
            collectionCardObject.transform.parent = collectionObject.transform;
            collectionCardObject.transform.localPosition = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;
            collectionCardObject.SetBothResetValues();
            collectionCardObject.visual.Redraw();
            collectionCardObject.visual.gameObject.SetActive(true);  //to-do: figure out what is causing this...
            CreateGrayed(collectionCardObject.transform, card).parent = grayed;

            createdCardObjects.Add(collectionCardObject);
            index++;
        }
        return createdCardObjects;
    }

    private Transform CreateGrayed(Transform source, Card card)
    {
        CollectionCardObject grayedCardObject = InitializeCollectionCardObject(card, true);
        grayedCardObject.visual.Stencil = -100;
        grayedCardObject.noInteraction = true;
        foreach (HyperCard.Card.CustomSpriteParam spriteParam in grayedCardObject.visual.SpriteObjects)
        {
            spriteParam.IsAffectedByFilters = true;
        }
        grayedCardObject.visual.SetGrayscale(true);
        grayedCardObject.visual.SetOpacity(0.7f);
        grayedCardObject.visual.gameObject.SetActive(true);  //to-do: figure out how to refactor this, maybe just set this in initialize func
        grayedCardObject.gameObject.transform.position = source.position;
        grayedCardObject.gameObject.SetLayer(LayerMask.NameToLayer("Ignore Raycast"));

        return grayedCardObject.gameObject.transform;
    }

    public void IncludeCard(CollectionCardObject cardObject)
    {
        if (includedCards.Count >= REQUIRED_DECK_SIZE)
        {
            LeanTween.scale(deckSize.gameObject, Vector3.one * 1.3f, 1).setEasePunch();
            return;
        }

        if (cardObject.visual != null)
        {
            cardObject.visual.SetOutline(false);
        }
        else
        {
            Debug.LogError(String.Format("CardObject visual is null for {0}", cardObject.Card.GetName()));
        }

        //get insertion index
        int cardCost = cardObject.Card.GetCost();
        string cardName = cardObject.Card.GetName();
        int cardLevel = cardObject.Card.Level;
        int insertionIndex = this.sortedCardCutouts.Count;

        for (int i = 0; i < this.sortedCardCutouts.Count; i += 1)
        {
            CardCutout currentCardCutout = this.sortedCardCutouts[i];
            if (cardCost < currentCardCutout.GetCost())
            {
                insertionIndex = i;
                break;
            }
            else if (cardCost > currentCardCutout.GetCost())
            {
                continue;

            }

            if (String.Compare(cardName, currentCardCutout.GetName(), StringComparison.OrdinalIgnoreCase) < 0)
            {
                insertionIndex = i;
                break;
            }
            else if (String.Compare(cardName, currentCardCutout.GetName(), StringComparison.OrdinalIgnoreCase) == 0)
            {
                continue;
            }

            if (cardCost < currentCardCutout.GetCost())
            {
                insertionIndex = i;
                break;
            }
            else if (cardCost > currentCardCutout.GetCost())
            {
                continue;

            }

            if (cardLevel < currentCardCutout.GetLevel())
            {
                insertionIndex = i;
                break;
            }
        }

        //create cutout and set to child
        CardCutout cardCutout = InitializeCardCutout();
        cardCutout.transform.localPosition = Vector3.zero;
        cardCutout.transform.SetSiblingIndex(insertionIndex);

        cardCutout.Render(cardObject);
        cardCutout.SetClickListener(new UnityAction(() =>
        {
            RemoveCard(cardCutout);
        }));

        this.sortedCardCutouts.Insert(insertionIndex, cardCutout);
        this.collection.Remove(cardObject);
        this.includedCards.Add(cardObject.Card);

        UpdateDeckSize();

        cardObject.Burn(new UnityAction(() => { }), 0.2f);
    }

    private void RemoveCard(CardCutout source)
    {
        //int removeIndex = this.sortedCardCutouts.FindIndex(
        //    cardCutout => cardCutout.GetId() == source.GetId()
        //);
        //this.sortedCardCutouts.RemoveAt(removeIndex);
        this.sortedCardCutouts.Remove(source);

        LeanTween
            .moveX(source.gameObject, source.transform.position.x + 4, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                RecycleCardCutout(source);
            });

        //instantiate card, reset
        CollectionCardObject created = InitializeCollectionCardObject(source.GetCard());
        created.transform.parent = this.collectionObject.transform;

        CardObject.Reset reset = source.GetReset();
        created.transform.localPosition = reset.position;
        created.transform.localRotation = reset.rotation;
        created.transform.localScale = reset.scale;
        created.SetBothResetValues();

        created.visual.Redraw();

        includedCards.Remove(source.GetCard());
        collection.Add(created);
        UpdateDeckSize();
    }

    private void UpdateDeckSize()
    {
        deckSize.text = String.Format("{0}/{1}", includedCards.Count, REQUIRED_DECK_SIZE);
    }

    private void InitializeCollectionCardObjectPools()
    {
        this.collectionCardObjectPools = new Dictionary<Card.CardType, Stack<CollectionCardObject>>();
        this.collectionCardObjectPools[Card.CardType.Creature] = new Stack<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Spell] = new Stack<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Structure] = new Stack<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Weapon] = new Stack<CollectionCardObject>();

        for (int i = 0; i < REQUIRED_DECK_SIZE * 2; i++)
        {
            InstantiateCreatureCardGameObject();
        }

        this.cardCutoutPool = new Stack<CardCutout>();
        for (int i = 0; i < REQUIRED_DECK_SIZE; i++)
        {
            GameObject cardCutoutGameObject = Instantiate(
                this.cardCutoutPrefab,
                transform.position,
                Quaternion.identity
            );
            cardCutoutGameObject.SetActive(false);
            CardCutout cardCutout = cardCutoutGameObject.GetComponent<CardCutout>();
            cardCutout.transform.SetParent(this.cutoutContent.transform, false);
            this.cardCutoutPool.Push(cardCutout);
        }
    }

    private CollectionCardObject InstantiateCreatureCardGameObject()
    {
        GameObject creatureCardGameObject = Instantiate(
            this.creatureCardObjectPrefab,
            transform.position,
            Quaternion.identity
        );
        creatureCardGameObject.transform.parent = this.transform;
        creatureCardGameObject.SetActive(false);
        CollectionCardObject collectionCardObject = creatureCardGameObject.GetComponent<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Creature].Push(collectionCardObject);
        return collectionCardObject;
    }

    private CollectionCardObject InstantiateSpellCardGameObject()
    {
        GameObject spellCardGameObject = Instantiate(
            this.spellCardObjectPrefab,
            transform.position,
            Quaternion.identity
        );
        spellCardGameObject.transform.parent = this.transform;
        spellCardGameObject.SetActive(false);
        CollectionCardObject collectionCardObject = spellCardGameObject.GetComponent<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Spell].Push(collectionCardObject);
        return collectionCardObject;
    }

    private CollectionCardObject InstantiateStructureCardGameObject()
    {
        GameObject structureCardGameObject = Instantiate(
            this.structureCardObjectPrefab,
            transform.position,
            Quaternion.identity
        );
        structureCardGameObject.transform.parent = this.transform;
        structureCardGameObject.SetActive(false);
        CollectionCardObject collectionCardObject = structureCardGameObject.GetComponent<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Structure].Push(collectionCardObject);
        return collectionCardObject;
    }

    private CollectionCardObject InstantiateWeaponCardGameObject()
    {
        GameObject weaponCardGameObject = Instantiate(
            this.weaponCardObjectPrefab,
            transform.position,
            Quaternion.identity
        );
        weaponCardGameObject.transform.parent = this.transform;
        weaponCardGameObject.SetActive(false);
        CollectionCardObject collectionCardObject = weaponCardGameObject.GetComponent<CollectionCardObject>();
        this.collectionCardObjectPools[Card.CardType.Structure].Push(collectionCardObject);
        return collectionCardObject;
    }

    private void RecycleCollectionCardObject(CollectionCardObject collectionCardObject)
    {
        collectionCardObject.gameObject.SetActive(false);
        collectionCardObject.transform.parent = this.transform;
        this.collectionCardObjectPools[collectionCardObject.GetCardType()].Push(collectionCardObject);
    }

    public CollectionCardObject InitializeCollectionCardObject(Card card, bool isHollow = false)
    {
        CollectionCardObject collectionCardObject;
        Card.CardType cardType = card.GetCardType();

        if (this.collectionCardObjectPools[cardType].Count <= 0)
        {
            switch (cardType)
            {
                case Card.CardType.Creature:
                    collectionCardObject = InstantiateCreatureCardGameObject();
                    break;
                case Card.CardType.Spell:
                    collectionCardObject = InstantiateSpellCardGameObject();
                    break;
                case Card.CardType.Structure:
                    collectionCardObject = InstantiateStructureCardGameObject();
                    break;
                case Card.CardType.Weapon:
                    collectionCardObject = InstantiateWeaponCardGameObject();
                    break;
                default:
                    Debug.LogError("Unsupported card type.");
                    return null;
            }
        }
        else
        {
            collectionCardObject = this.collectionCardObjectPools[cardType].Pop();
        }

        if (isHollow)
        {
            collectionCardObject.InitializeHollow(card);
        }
        else
        {
            collectionCardObject.Initialize(card);
        }
        collectionCardObject.gameObject.SetActive(true);
        return collectionCardObject;
    }

    private void RecycleCardCutout(CardCutout cardCutout)
    {
        cardCutout.gameObject.SetActive(false);
        this.cardCutoutPool.Push(cardCutout);
    }

    public CardCutout InitializeCardCutout()
    {
        if (this.cardCutoutPool.Count <= 0)
        {
            Debug.LogError("Card cutout pool is empty!");
            return null;
        }

        CardCutout cardCutout = this.cardCutoutPool.Pop();
        cardCutout.gameObject.SetActive(true);
        return cardCutout;
    }
}
