using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using GameSparks.Api.Responses;
using TMPro;

[System.Serializable]
public class CollectionManager : MonoBehaviour
{
    [SerializeField]
    private List<Card> collection;
    [SerializeField]
    private List<string> deckNames;

    [SerializeField]
    private Dictionary<string, Card> idToCard;

    [SerializeField]
    private List<CollectionCardObject> cardsInDeck;  //list that is currently rendered/being changed
    public List<CollectionCardObject> CardsInDeck => cardsInDeck;
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

    private CollectionCardObject selectedCard;
    private LogEventResponse cardsResponse;

    public bool editMode = false;

    [SerializeField]
    private GraphicRaycaster m_Raycaster;
    [SerializeField]
    private EventSystem m_EventSystem;
    private PointerEventData m_PointerEventData;

    public static CollectionManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        this.collection = new List<Card>();
        this.deckNames = new List<string>();
        this.idToCard = new Dictionary<string, Card>();
    }

    private void Start()
    {
        DeckStore.Instance().GetDecksWithCallback(Callback);
    }

    private void Callback()
    {
        deckNames = DeckStore.Instance().GetDeckNames();
        for (int i = 0; i < deckButtons.Count; ++i)
        {
            deckButtons[i].onClick.AddListener(EnterEditMode);
        }

        List<Card> cards = DeckStore.Instance().GetCards();
        this.collection = cards;

        CreateCardObjects();
    }

    private void Update()
    {
        if (editMode)
        {
            ScrollToVerticalPan();
        }

        CastGraphicsRaycast();
    }

    private void CastGraphicsRaycast()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (ActionManager.Instance.HasDragTarget())
            {
                //Set up the new Pointer Event, Set the Pointer Event Position to that of the mouse position
                m_PointerEventData = new PointerEventData(m_EventSystem);
                m_PointerEventData.position = Input.mousePosition;

                //Create a list of Raycast Results
                //Raycast using the Graphics Raycaster and mouse click position
                List<RaycastResult> results = new List<RaycastResult>();
                m_Raycaster.Raycast(m_PointerEventData, results);
                CheckForDecklist(results, ActionManager.Instance.GetDragTarget());
            }
        }
    }

    private void CheckForDecklist(List<RaycastResult> hit, CardObject cardObject)
    {
        foreach (RaycastResult element in hit)
        {
            if (element.gameObject != rightSidebar)
                continue;

            IncludeCard(cardObject);
        }
    }

    private void OnSaveButton()
    {
        SaveCollectionRequest();
    }

    private void SaveCollectionRequest()
    {
        List<string> cardIds = new List<string>();
        foreach (CollectionCardObject elem in this.cardsInDeck)
        {
            cardIds.Add(elem.Card.Id);
        }
        string previousName = deckNames[activeDeck];
        string newName = deckNames[activeDeck];   //to-do, let name be changed, and pull from input field

        DeckStore.Instance().CreateUpdatePlayerDeckWithCallback(
            cardIds,
            previousName,
            newName,
            SaveCallback
        );
    }

    private void SaveCallback()
    {

    }

    private void ScrollToVerticalPan()
    {
        float vertical = Mathf.Clamp(collectionObject.transform.localPosition.y + Input.mouseScrollDelta.y * -0.1F, -0.5f, 100);
        collectionObject.transform.localPosition = new Vector3(collectionObject.transform.localPosition.x, vertical, collectionObject.transform.localPosition.z);
    }

    private void EnterEditMode()
    {
        LeanTween.moveLocalX(deckSelection, -800, 0.5f).setEaseOutQuad();
        LeanTween.moveLocalX(rightSidebar, 400, 0.5f).setEaseInQuad();

        LeanTween.moveY(collectionObject, -0.5f, 0.5f)
                 .setEaseInQuad()
                 .setOnComplete(() =>
                 {
                     this.editMode = true;
                 });
    }

    private void CreateCardObjects()
    {
        int index = 0;
        int rowSize = 4;

        Vector3 topLeft = new Vector3(-5.95f, 3.11f, 0);
        Vector3 horizontalOffset = new Vector3(2.9f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.95f, 0f);

        Transform grayed = new GameObject("Grayed").transform as Transform;
        grayed.parent = this.collectionObject.transform;

        foreach (Card card in collection)
        {
            GameObject created = new GameObject(card.Name);
            created.transform.parent = collectionObject.transform;
            created.transform.localPosition = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;

            CollectionCardObject collectionCardObject = created.AddComponent<CollectionCardObject>();
            collectionCardObject.Initialize(card);
            idToCard.Add(card.Id, card);

            CreateGrayed(created.transform, card).parent = grayed;
            collectionCardObject.visual.Redraw();

            index++;
        }
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

    public void IncludeCard(CardObject cardObject)
    {
        cardObject.visual.SetOutline(false);
        GameObject cutout = Instantiate(cardCutout, cutoutContent.transform);
        cardObject.Burn(new UnityAction(() =>
        {
            //Play a nice sound
        }), 0.33f);
    }
}
