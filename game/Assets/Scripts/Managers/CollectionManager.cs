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
    private List<string> decks;

    [SerializeField]
    private List<Card> cardsInDeck;  //list that is currently rendered/being changed
    public List<Card> CardsInDeck => cardsInDeck;
    private int activeDeck;

    [SerializeField]
    private List<Button> deckButtons;
    [SerializeField]
    private GameObject cardCutout;
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

    private GameObject collectionCardObjectPrefab;
    private Stack<CollectionCardObject> collectionCardObjectPool;

    public static CollectionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.collection = new List<CollectionCardObject>();
        this.decks = new List<string>();
        this.cardsInDeck = new List<Card>();

        InitializeCollectionCardObjectPool();
    }

    private void Start()
    {
        DeckStore.Instance().GetDecksWithCallback(Callback);
    }

    private void Callback()
    {
        decks = DeckStore.Instance().GetDeckNames();
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
            int deckId = i;
            deckButtons[i].onClick.AddListener(new UnityAction(() => EnterEditMode(deckId)));
        }

        List<Card> cards = DeckStore.Instance().GetCards();
        this.collection = CreateCardObjects(cards);
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

        //Create a list of Raycast Results
        //Raycast using the Graphics Raycaster and mouse click position
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
        foreach (Card elem in this.cardsInDeck)
        {
            cardIds.Add(elem.Id);
        }
        string previousName = decks[activeDeck];
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
        LeanTween.moveLocalX(deckSelection, -800, TWEEN_TIME).setEaseOutQuad();
        LeanTween.moveLocalX(rightSidebar, 320, TWEEN_TIME).setEaseInQuad();

        LeanTween.moveY(collectionObject, -0.5f, TWEEN_TIME)
                 .setEaseInQuad()
                 .setOnComplete(() =>
                 {
                     this.editMode = true;
                     this.activeDeck = deckId;

                     InputField deckNameField = rightSidebar.GetComponentInChildren<InputField>();
                     deckNameField.text = decks[deckId];

                     LoadDecklist(deckId);
                 });
    }

    private void ExitEditMode()
    {
        LeanTween.moveLocalX(deckSelection, 0, TWEEN_TIME).setEaseOutQuad();
        LeanTween.moveLocalX(rightSidebar, 480, TWEEN_TIME).setEaseInQuad();

        LeanTween
            .moveY(collectionObject, -10, TWEEN_TIME)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                this.editMode = false;
                foreach (Transform child in cutoutContent.transform)
                {
                    RemoveCard(child.GetComponent<CardCutout>());
                }
            });
    }

    private void LoadDecklist(int deckId)
    {
        foreach (Card card in DeckStore.Instance().GetCardsByDeckName(decks[deckId]))
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

            CreateGrayed(collectionCardObject.transform, card).parent = grayed;
            collectionCardObject.visual.Redraw();

            createdCardObjects.Add(collectionCardObject);
            index++;
        }
        return createdCardObjects;
    }

    private Transform CreateGrayed(Transform source, Card card)
    {
        GameObject created = new GameObject("_gray_" + card.Name);
        created.transform.position = source.position;

        //do visual stuff
        CollectionCardObject grayed = created.AddComponent<CollectionCardObject>();
        grayed.InitializeHollow(card);
        grayed.visual.Stencil = -100;
        foreach (HyperCard.Card.CustomSpriteParam spriteParam in grayed.visual.SpriteObjects)
        {
            spriteParam.IsAffectedByFilters = true;
        }
        grayed.visual.SetGrayscale(true);
        grayed.visual.SetOpacity(0.7f);
        grayed.gameObject.SetLayer(LayerMask.NameToLayer("Board"));

        return created.transform;
    }


    public void IncludeCard(CollectionCardObject cardObject)
    {
        if (cardsInDeck.Count >= REQUIRED_DECK_SIZE)
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
        int insertionIndex = cutoutContent.transform.childCount;

        for (int i = 0; i < cutoutContent.transform.childCount; ++i)
        {
            CardCutout selected = cutoutContent.transform.GetChild(i).GetComponent<CardCutout>();
            if (cardCost < selected.Cost)
            {
                insertionIndex = i;
                break;
            }
            else if (cardCost == selected.Cost)
            {
                if (String.Compare(cardName, selected.name, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    insertionIndex = i;
                    break;
                }
            }
        }
        //create cutout and set to child
        CardCutout cutout = Instantiate(cardCutout, cutoutContent.transform).GetComponent<CardCutout>();
        cutout.transform.localPosition = Vector3.zero;
        cutout.transform.SetSiblingIndex(insertionIndex);

        cutout.Render(cardObject);
        cutout.GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
        {
            RemoveCard(cutout);
        }));

        collection.Remove(cardObject);
        cardsInDeck.Add(cardObject.Card);
        UpdateDeckSize();

        cardObject.Burn(new UnityAction(() =>
        {
            //Do something?
        }), 0.2f);
    }

    public void RemoveCard(CardCutout source)
    {
        //source.transform.parent = this.rightSidebar.transform;
        LeanTween
            .moveX(source.gameObject, source.transform.position.x + 4, 0.5f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                Destroy(source.gameObject); //to-do: play effect
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

        cardsInDeck.Remove(source.GetCard());
        collection.Add(created);
        UpdateDeckSize();
    }

    private void UpdateDeckSize()
    {
        deckSize.text = String.Format("{0}/{1}", cardsInDeck.Count, REQUIRED_DECK_SIZE);
    }

    private void InitializeCollectionCardObjectPool()
    {
        this.collectionCardObjectPrefab = Resources.Load("Prefabs/CollectionCardObject") as GameObject;
        this.collectionCardObjectPool = new Stack<CollectionCardObject>();

        for (int i = 0; i < 40; i++)
        {
            GameObject collectionCardGameObject = Instantiate(
                this.collectionCardObjectPrefab,
                transform.position,
                Quaternion.identity
            );
            collectionCardGameObject.transform.parent = this.transform;
            collectionCardGameObject.SetActive(false);
            CollectionCardObject collectionCardObject = collectionCardGameObject.GetComponent<CollectionCardObject>();
            this.collectionCardObjectPool.Push(collectionCardObject);
        }
    }

    private void RecycleCollectionCardObject(CollectionCardObject collectionCardObject)
    {
        collectionCardObject.gameObject.SetActive(false);
        collectionCardObject.transform.parent = this.transform;
        this.collectionCardObjectPool.Push(collectionCardObject);
    }

    public CollectionCardObject InitializeCollectionCardObject(Card card)
    {
        CollectionCardObject collectionCardObject;

        if (this.collectionCardObjectPool.Count <= 0)
        {
            GameObject collectionCardGameObject = Instantiate(
                this.collectionCardObjectPrefab,
                transform.position,
                Quaternion.identity
            );
            collectionCardObject = collectionCardGameObject.GetComponent<CollectionCardObject>();
        }
        else
        {
            collectionCardObject = this.collectionCardObjectPool.Pop();
        }

        collectionCardObject.Initialize(card);
        collectionCardObject.gameObject.SetActive(true);
        return collectionCardObject;
    }
}
