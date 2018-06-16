using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using UnityEngine.SceneManagement;

using TMPro;

[System.Serializable]
public class CollectionManager : MonoBehaviour {

    private List<Card> collection;
    private List<Deck> decks;
    [SerializeField]
    private Deck chosenDeck;
    private Dictionary<string, Card> idToCard;

    private List<CardCutout> cutouts;
    private GameObject panel;
    private Collider panelColl;
    private GameObject collectionObject;

    private CardObject selectedCard;
    public GameObject deckPanel;
    private LogEventResponse cardsResponse;

    public static CollectionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start() {
        this.collection = new List<Card>();
        this.decks = new List<Deck>();
        this.cutouts = new List<CardCutout>();
        this.idToCard = new Dictionary<string, Card>();

        //ping server for collection json
        GetCollectionRequest();

        collectionObject = new GameObject("Collection");
        panel = GameObject.Find("Build Panel");
        panelColl = panel.GetComponent<BoxCollider>() as Collider;
    }

    private void Update()
    {
        RaycastMouse();
        RaycastMouseUp();
    }

    private void RaycastMouse()
    {
        if (Input.GetMouseButton(0))
        {
            if (!ActionManager.Instance.HasDragTarget())
                return;
            selectedCard = ActionManager.Instance.GetDragTarget();
            Ray ray = new Ray(selectedCard.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (selectedCard && panelColl.Raycast(ray, out hit, 100.0F))
                MinifyCard(selectedCard);
            else
                RevertMinify(selectedCard);
        }
    }

    private void RaycastMouseUp() {
        if (Input.GetMouseButtonUp(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (panelColl.Raycast(ray, out hit, 100.0F) && selectedCard)
            {
                //add to deck
                AddToDeck(selectedCard);
                RevertMinify(selectedCard);
            }
            else if(Physics.Raycast(ray, out hit, 100.0F)) {
                if(hit.collider.name == "Save Button") {
                    SaveCollectionRequest();
                }
                else if (hit.collider.name == "Back Button")
                {
                    SceneManager.LoadScene("Collection");
                }
            }
        }
    }

    private void MinifyCard(CardObject wrapper) {
        if(!wrapper.minified)
            wrapper.Minify(true);
    }

    private void RevertMinify(CardObject wrapper) {
        if (wrapper.minified)
            wrapper.Minify(false);
        selectedCard = null;
    }

    public void RotateToDeck(Deck deck)
    {
        LeanTween.rotateY(Camera.main.gameObject, 2f, 1f).setEaseOutCubic();
        Debug.Log("# of cards in deck: " + deck.Cards.Count);
        List<Card> existing = new List<Card>(deck.Cards);
        chosenDeck = deck;
        GameObject.Find("Deck Name").GetComponent<TextMeshPro>().text = deck.Name;

        foreach (Card card in existing)
        {
            AddToBuildPanel(card.wrapper);
        }
    }

    private void CreateDecksView()
    {
        GameObject placeholders = GameObject.Find("Deck Panel Placeholders");
        int count = 0;
        foreach (Deck deck in decks)
        {
            Transform t = placeholders.transform.GetChild(count);
            GameObject created = GameObject.Instantiate(deckPanel, t.position, t.localRotation);
            created.transform.Find("Deck Name").GetComponent<TextMeshPro>().text = deck.Name;
            created.GetComponent<DeckPanel>().Initialize(deck);
            ++count;
        }
    }

    public void AddToDeck(CardObject wrapper) {
        //add to data structure
        chosenDeck.AddCard(wrapper.card);
        AddToBuildPanel(wrapper);
    }

    public void AddToBuildPanel(CardObject wrapper) {
        wrapper.gameObject.SetActive(false);
        //create and set visuals
        GameObject instance = new GameObject("Added " + wrapper.card.Name) as GameObject;
        CardCutout cutout = instance.AddComponent<CardCutout>();
        cutout.Initialize(wrapper, chosenDeck.Cards);
        cutouts.Add(cutout);
        //reposition all cutouts
        RenderDecklist();
    }

    public void RemoveFromDeck(CardObject wrapper, CardCutout cutout) {
        chosenDeck.RemoveCard(wrapper.card);
        wrapper.gameObject.SetActive(true);

        cutouts.Remove(cutout);
        //do some resetting of collection/gray cards, reposition the rest of the cards in the panel
        RenderDecklist();
    }

    private void RenderDecklist() {
        for (int i = 0; i < cutouts.Count; ++i)
        {
            cutouts[i].PositionCutout(i);
        }
    }

    private void SaveCollectionRequest() {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("CreateUpdatePlayerDeck");
        List<string> cardIds = new List<string>();
        foreach(Card card in chosenDeck.Cards) {
            cardIds.Add(card.Id);
        }
        request.SetEventAttribute("cardIds", cardIds);
        request.SetEventAttribute("previousName", chosenDeck.Name);
        request.SetEventAttribute("name", chosenDeck.Name);
        request.Send(SaveCollectionSuccess, Error);
    }

    private void SaveCollectionSuccess(LogEventResponse resp) {
        Debug.Log("Successfully saved deck.");
    }

    private void GetCollectionRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerCards");
        request.Send(GetCollectionSuccess, Error); //TODO: set callbacks
    }

    private void GetCollectionSuccess(LogEventResponse resp)
    {
        cardsResponse = resp;
        GSData decksData = resp.ScriptData.GetGSData("decks");
        Debug.Log(decksData.BaseData.Count + " decks found.");

        //Create pool of cards
        List<GSData> data = resp.ScriptData.GetGSDataList("cards");
        foreach(GSData elem in data) {
            Card newCard = JsonUtility.FromJson<Card>(elem.JSON);
            collection.Add(newCard);
        }
        CreateCardObjects();

        //Create decks by mapping to cards
        foreach (string key in decksData.BaseData.Keys)
        {
            List<string> gdata = decksData.GetStringList(key);
            Deck created = new Deck(key, new List<Card>(), Deck.DeckClass.Warrior);
            foreach (string cardId in gdata)
            {
                //Card newCard = JsonUtility.FromJson<Card>(card.JSON);
                created.Cards.Add(idToCard[cardId]);
            }
            decks.Add(created);
        }
        CreateDecksView();
    }

    private void Error(LogEventResponse resp)
    {
        Debug.Log("Gamesparks Request Error!");
    }

    private void CreateCardObjects()
    {
        int index = 0;
        int rowSize = 4;

        //Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0.113f, 0.8f, transform.position.z));
        Vector3 topLeft = new Vector3(-5.05f, 3.11f, 18.18f);
        Vector3 horizontalOffset = new Vector3(2.64f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.75f, 0f);

        Transform grayed = new GameObject("Grayed").transform as Transform;
        foreach (Card card in collection)
        {
            GameObject created = new GameObject(card.Name);
            created.transform.parent = collectionObject.transform;
            CardObject wrapper = created.AddComponent<CardObject>();
            wrapper.InitializeCard(card);
            idToCard.Add(card.Id, card);

            created.transform.position = topLeft + index % rowSize * horizontalOffset + index / rowSize * verticalOffset;
            CreateGrayed(created.transform, card).parent = grayed;
            index++;
        }
    }

    private Transform CreateGrayed(Transform source, Card card)
    {
        GameObject created = new GameObject(card.Name + "_gray");
        created.transform.position = source.position;

        SpriteRenderer sprenderer = created.AddComponent<SpriteRenderer>() as SpriteRenderer;
        Texture2D image = Resources.Load(card.Image) as Texture2D;
        sprenderer.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        sprenderer.sortingOrder = -100;
        sprenderer.material.shader = Shader.Find("Custom/Grayscale");

        return created.transform;
    }

    private void CreateDeckLocal()
    {
        //jank json test, for generating list of json objects
        collection.Add(new CreatureCard("C1", "Direhorn Hatchling", 5, "Direhorn_Hatchling", 3, 6));
        collection.Add(new CreatureCard("C2", "Fiery War Axe", 3, "Fiery_War_Axe", 3, 2));
    }
}
