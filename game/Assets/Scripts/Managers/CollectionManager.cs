using System.Collections;
using System.Collections.Generic;
using System.IO;

using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

[System.Serializable]
public class CollectionManager : MonoBehaviour
{
    [SerializeField]
    private List<Card> collection;
    private List<DeckRaw> decksRaw;

    [SerializeField]
    private Dictionary<string, Card> idToCard;
    [SerializeField]
    private List<CollectionCardObject> activeDecklist;  //list that is currently rendered/being changed
    public List<CollectionCardObject> ActiveDecklist => activeDecklist;
    private int activeDeck;

    public GameObject cutoutPrefab;
    public GameObject deckPanelPrefab;

    private GameObject buildPanel;
    private Collider buildPanelCollider;
    public GameObject collectionObject;

    private CollectionCardObject lastClickedCard;
    private CollectionCardObject selectedCard;
    private LogEventResponse cardsResponse;

    public static CollectionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.collection = new List<Card>();
        this.decksRaw = new List<DeckRaw>();
        this.activeDecklist = new List<CollectionCardObject>();
        this.idToCard = new Dictionary<string, Card>();
    }

    private void Start()
    {
        this.collectionObject = GameObject.Find("Collection");
        this.buildPanel = GameObject.Find("Build Panel");
        this.buildPanelCollider = buildPanel.GetComponent<BoxCollider>() as Collider;

        DeckStore.Instance().GetDecksWithCallback(Callback);
    }

    private void Callback()
    {
        List<Card> cards = DeckStore.Instance().GetCards();
        this.collection = cards;
        CreateCardObjects();

        //Create decks by mapping to cards
        foreach (string deckName in DeckStore.Instance().GetDeckNames())
        {
            List<string> cardIds = DeckStore.Instance().GetCardIdsByDeckName(deckName);
            DeckRaw created = new DeckRaw(deckName, cardIds, DeckRaw.DeckClass.Warrior);
            decksRaw.Add(created);
        }
        this.CreateDecksView();
    }

    private void Update()
    {
        RaycastMouse();
        RaycastMouseUp();
        ScrollToPan(Vector3.up);  //to-do only allow situationally
    }

    private void ScrollToPan(Vector3 axes)
    {
        collectionObject.transform.Translate(axes * Input.mouseScrollDelta.y * -0.1F);
    }

    private void RaycastMouse()
    {
        if (!Input.GetMouseButton(0))
            return;
        if (!ActionManager.Instance.HasDragTarget())
            return;

        selectedCard = ActionManager.Instance.GetDragTarget() as CollectionCardObject;     //assumed due to scene file
        Ray ray = new Ray(selectedCard.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (selectedCard && buildPanelCollider.Raycast(ray, out hit, 100.0F))
        {
            MinifyCard(selectedCard);
        }
        else
        {
            RevertMinify(selectedCard);
        }
    }

    private void RaycastMouseUp()
    {
        if (!Input.GetMouseButtonUp(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(ray, out hit, 100.0F);

        CollectionCardObject hitCardObject = hit.collider.GetComponent<CollectionCardObject>();
        if (hitCardObject != null && this.activeDecklist.Contains(hitCardObject))   //if hit a card object, and it is minified
        {
            RemoveFromDecklist(hitCardObject);
        }
        else if (buildPanelCollider.Raycast(ray, out hit, 100.0F) && selectedCard)
        {
            //add to deck
            AddToDecklist(selectedCard);
        }
        else if (ActionManager.Instance.HasDragTarget())
        {
            ActionManager.Instance.GetDragTarget().Release();
        }

        //check for double clicks
        if (hitCardObject != null)
        {
            if (hitCardObject == lastClickedCard &&
                (Time.time - hitCardObject.lastClickedTime) < 0.5F &&
                (Time.time - hitCardObject.lastDoubleClickedTime) > 2)
            {
                hitCardObject.DoubleClickUp();
            }
            hitCardObject.lastClickedTime = Time.time;
        }
        lastClickedCard = hitCardObject;
    }

    private void OnSaveButton()
    {
        SaveCollectionRequest();
    }

    private void OnBackButton()
    {
        SceneManager.LoadScene("Collection");
    }

    private void CreateDecksView()
    {
        GameObject placeholders = GameObject.Find("Deck Panel Placeholders");
        int count = 0;
        foreach (DeckRaw deckRaw in decksRaw)
        {
            Transform t = placeholders.transform.GetChild(count);
            GameObject created = GameObject.Instantiate(deckPanelPrefab, t.position, t.localRotation);
            created.transform.Find("Deck Name").GetComponent<TextMeshPro>().text = deckRaw.name;
            created.GetComponent<DeckPanel>().Initialize(deckRaw);
            ++count;
        }
    }

    public void AddToDecklist(CollectionCardObject collectionCardObject)
    {
        //add to data structure
        collectionCardObject.SetMinify(true);
        this.activeDecklist.Add(collectionCardObject);

        //reposition all cutouts
        RenderDecklist();
    }

    public void RemoveFromDecklist(CollectionCardObject collectionCardObject)
    {
        collectionCardObject.SetMinify(false);

        this.activeDecklist.Remove(collectionCardObject);
        collectionCardObject.Release();
        RenderDecklist();  //do some resetting of collection/gray cards, reposition the rest of the cards in the panel
    }

    private void RenderDecklist()
    {
        for (int i = 0; i < this.activeDecklist.Count; i++)
        {
            this.PositionForDecklist(this.activeDecklist[i], i);
        }
    }

    private void PositionForDecklist(CollectionCardObject targetObject, int index)
    {
        CardTween.move(targetObject, buildPanel.transform.position + Vector3.down * (index + 1) * 0.6F, ActionManager.TWEEN_DURATION);
    }

    private void SaveCollectionRequest()
    {
        List<string> cardIds = new List<string>();
        foreach (CollectionCardObject elem in this.activeDecklist)
        {
            cardIds.Add(elem.Card.Id);
        }
        string previousName = decksRaw[activeDeck].name;
        string name = decksRaw[activeDeck].name;   //to-do, let name be changed, and pull from input field

        DeckStore.Instance().CreateUpdatePlayerDeckWithCallback(
            cardIds,
            previousName,
            name,
            SaveCallback
        );
    }

    private void SaveCallback()
    {

    }

    private void CreateCardObjects()
    {
        int index = 0;
        int rowSize = 4;

        Vector3 topLeft = new Vector3(-5.65f, 3.11f, 18.18f);
        Vector3 horizontalOffset = new Vector3(2.75f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.75f, 0f);

        Transform grayed = new GameObject("Grayed").transform as Transform;
        grayed.parent = this.collectionObject.transform;

        foreach (Card card in collection)
        {
            GameObject created = new GameObject(card.Name);
            created.transform.parent = collectionObject.transform;
            created.transform.position = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;

            CollectionCardObject collectionCardObject = created.AddComponent<CollectionCardObject>();
            collectionCardObject.Initialize(card);
            collectionCardObject.visual.SetOutline(false);
            idToCard.Add(card.Id, card);

            CreateGrayed(created.transform, card).parent = grayed;
            index++;
        }
    }

    private Transform CreateGrayed(Transform source, Card card)
    {
        GameObject created = new GameObject("_gray_" + card.Name);
        created.transform.position = source.position;

        //do visual stuff
        CollectionCardObject collectionCardObject = created.AddComponent<CollectionCardObject>();
        collectionCardObject.InitializeHollow(card);
        collectionCardObject.visual.Stencil = -100;
        collectionCardObject.visual.SetGrayscale(true);
        collectionCardObject.visual.SetOutline(false);
        collectionCardObject.gameObject.SetLayer(LayerMask.NameToLayer("Board"));
        return created.transform;
    }

    private void MinifyCard(CollectionCardObject collectionCardObject)
    {
        if (!collectionCardObject.minified)
            collectionCardObject.SetMinify(true);
    }

    private void RevertMinify(CollectionCardObject collectionCardObject)
    {
        if (collectionCardObject.minified)
            collectionCardObject.SetMinify(false);
        selectedCard = null;
    }

    public void RotateToDeck(DeckRaw deckRaw)
    {
        this.activeDeck = decksRaw.IndexOf(deckRaw);
        Debug.Log("# of cards in deck: " + deckRaw.cardIds.Count);

        GameObject.Find("Deck Name").GetComponent<TextMeshPro>().text = deckRaw.name;
        LeanTween.rotateY(Camera.main.gameObject, 2f, 1f).setEaseOutCubic();

        foreach (string cardId in deckRaw.cardIds)
        {
            this.AddToDecklist(idToCard[cardId].wrapper as CollectionCardObject);
        }
    }
}
