using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using TMPro;

public class Collection : MonoBehaviour {

    private List<Card> collection;
    private List<Deck> decks;
    public List<Card> deckToBuild;
    private Dictionary<Card, CardObject> cardToObject;

    private List<CardCutout> cutouts;
    private GameObject panel;
    private Collider panelColl;
    private Camera cam;
    private ActionManager action;
    private GameObject collectionObject;

    private CardObject selectedCard;
    public GameObject deckPanel;
    private LogEventResponse cardsResponse;


    //private class Storage
    //{
    //    private List<Card> used;
    //    private List<Card> remaining;
    //}

    //private void Start()
    //{
    //    StartCoroutine("RotateToDeckbuilding");
    //}

    public void RotateToDeck(Deck deck) {
        LeanTween.rotateY(gameObject, 2f, 1f).setEaseOutCubic();
        foreach(Card card in deck.Cards) {
            AddToDeck(cardToObject[card]);
        }
    }

    private void Awake()
    {
        collection = new List<Card>();
        decks = new List<Deck>();
        cutouts = new List<CardCutout>();
        cardToObject = new Dictionary<Card, CardObject>();

        //ping server for collection json
        GetCollectionRequest();

        cam = Camera.main;
        action = cam.GetComponent<ActionManager>();
        collectionObject = new GameObject("Collection");
        panel = GameObject.Find("Panel");
        panelColl = panel.GetComponent<BoxCollider>() as Collider;
    }

    private void CreateDecksView() {
        GameObject placeholders = GameObject.Find("Deck Panel Placeholders");
        int count = 0;
        foreach(Deck deck in decks) {
            Transform t = placeholders.transform.GetChild(count);
            GameObject created = Instantiate(deckPanel, t.position, t.localRotation);
            created.transform.Find("Deck Name").GetComponent<TextMeshPro>().text = deck.Name;
            created.GetComponent<DeckPanel>().Initialize(deck, this);
            ++count;
        }
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
            if (!action.HasDragTarget())
                return;
            Transform floatingCard = action.GetDragTarget();
            Ray ray = new Ray(floatingCard.position, Camera.main.transform.forward);
            RaycastHit hit;
            selectedCard = floatingCard.GetComponent<CardObject>();

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

    public void AddToDeck(CardObject wrapper) {
        //add to data structure
        deckToBuild.Add(wrapper.card);
        wrapper.gameObject.SetActive(false);

        //create and set visuals
        GameObject instance = new GameObject("Added " + wrapper.card.Name) as GameObject;
        CardCutout cutout = instance.AddComponent<CardCutout>();
        cutout.Initialize(wrapper, deckToBuild, this);
        cutouts.Add(cutout);
    }

    public void RemoveFromDeck(CardObject wrapper, CardCutout cutout) {
        deckToBuild.Remove(wrapper.card);
        wrapper.gameObject.SetActive(true);

        cutouts.Remove(cutout);
        //do some resetting of collection/gray cards, reposition the rest of the cards in the panel
        RenderDecklist();
    }

    private void RenderDecklist() {
        int count = 1;
        foreach(CardCutout cutout in cutouts) {
            cutout.PositionCutout(count);
            count++;
        }
    }

    private void GetCollectionRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerCards");
        request.Send(Success, Error); //TODO: set callbacks
    }

    private void Success(LogEventResponse resp)
    {
        cardsResponse = resp;
        GSData decksData = resp.ScriptData.GetGSData("decks");
        Debug.Log(decksData.BaseData.Count + " decks found.");
        foreach(string key in decksData.BaseData.Keys) {
            List<GSData> gdata = decksData.GetGSDataList(key);
            Deck created = new Deck(key, new List<Card>(), Deck.DeckClass.Warrior);

            foreach(GSData card in gdata) {
                Card newCard = JsonUtility.FromJson<Card>(card.JSON);
                created.Cards.Add(newCard);
            }
            decks.Add(created);
        }

        List<GSData> data = resp.ScriptData.GetGSDataList("cards");
        foreach(GSData elem in data) {
            Card newCard = JsonUtility.FromJson<Card>(elem.JSON);
            collection.Add(newCard);
        }
        CreateCardObjects();
        CreateDecksView();
    }

    private void Error(LogEventResponse resp)
    {
        Debug.LogError("Server-side error in GetCollectionRequest().");
    }

    private void ParseDeck(string json)
    {
        //json = json.Replace("DeckB", "Items");
        Debug.Log(json);
        deckToBuild = JsonList.FromJson<Card>(json);
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
            CardObject cWrapper = created.AddComponent<CardObject>();
            cWrapper.InitializeCard(card);
            cardToObject.Add(card, cWrapper);

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
        collection.Add(new Card("C1", "Direhorn Hatchling", Card.CardType.Creature, 5, 3, 6, "Direhorn_Hatchling(55524)"));
        collection.Add(new Card("C2", "Fiery War Axe", Card.CardType.Weapon, 3, 3, 2, "Fiery_War_Axe(632)_Gold"));
    }
}
