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
    private List<Deck> decks;
    [SerializeField]
    private Deck chosenDeck;
    private Dictionary<string, Card> idToCard;

    private List<CardCutout> cutouts;
    private GameObject panel;
    private Collider panelCollider;
    private GameObject collectionObject;

    private CollectionCardObject selectedCard;
    public GameObject deckPanel;
    private LogEventResponse cardsResponse;

    public Dictionary<string, CardTemplate> cardTemplates;

    public static CollectionManager Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        this.collection = new List<Card>();
        this.decks = new List<Deck>();
        this.cutouts = new List<CardCutout>();
        this.idToCard = new Dictionary<string, Card>();

        string codexPath = Application.dataPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + "codex.txt";
        this.cardTemplates = CodexHelper.ParseFile(codexPath);
    }

    private void Start()
    {
        //ping server for collection json
        GetCollectionRequest();

        collectionObject = new GameObject("Collection");
        panel = GameObject.Find("Build Panel");
        panelCollider = panel.GetComponent<BoxCollider>() as Collider;
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
        foreach (GSData elem in data)
        {
            int category = (int)elem.GetInt("category");
            Card newCard;
            switch (category)
            {
                case 0: //creature
                    newCard = JsonUtility.FromJson<CreatureCard>(elem.JSON);
                    break;
                case 1:  //spell
                    newCard = JsonUtility.FromJson<SpellCard>(elem.JSON);
                    break;
                case 2:  //weapon
                    newCard = JsonUtility.FromJson<WeaponCard>(elem.JSON);
                    break;
                case 3:  //structure
                    newCard = JsonUtility.FromJson<StructureCard>(elem.JSON);
                    break;
                default:
                    newCard = null;
                    Debug.LogError("Card has no category/type field!");
                    break;
            }
            collection.Add(newCard);
        }
        this.CreateCardObjects();

        //Create decks by mapping to cards
        foreach (string deckName in decksData.BaseData.Keys)
        {
            List<string> gdata = decksData.GetStringList(deckName);
            Deck created = new Deck(deckName, new List<Card>(), Deck.DeckClass.Warrior);
            foreach (string cardId in gdata)
            {
                //Card newCard = JsonUtility.FromJson<Card>(card.JSON);
                created.Cards.Add(idToCard[cardId]);
            }
            decks.Add(created);
        }
        this.CreateDecksView();
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

            selectedCard = ActionManager.Instance.GetDragTarget() as CollectionCardObject;     //assumed due to scene file
            Ray ray = new Ray(selectedCard.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (selectedCard && panelCollider.Raycast(ray, out hit, 100.0F))
                MinifyCard(selectedCard);
            else
                RevertMinify(selectedCard);
        }
    }

    private void RaycastMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (panelCollider.Raycast(ray, out hit, 100.0F) && selectedCard)
            {
                //add to deck
                AddToDecklist(selectedCard);
                RevertMinify(selectedCard);
            }
            else if (Physics.Raycast(ray, out hit, 100.0F))
            {
                if (hit.collider.name == "Save Button")
                {
                    SaveCollectionRequest();
                }
                else if (hit.collider.name == "Back Button")
                {
                    SceneManager.LoadScene("Collection");
                }
            }
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

    public void AddToDecklist(CollectionCardObject collectionCardObject)
    {
        //add to data structure
        chosenDeck.AddCard(collectionCardObject.Card);
        AddToBuildPanel(collectionCardObject);
    }

    public void AddToBuildPanel(CardObject wrapper)
    {
        wrapper.gameObject.SetActive(false);
        //create and set visuals
        GameObject instance = new GameObject("Added " + wrapper.Card.Name) as GameObject;
        CardCutout cutout = instance.AddComponent<CardCutout>();
        //cutout.Initialize(wrapper, chosenDeck.Cards);
        cutouts.Add(cutout);
        //reposition all cutouts
        RenderDecklist();
    }

    public void RemoveFromDecklist(CollectionCardObject collectionCardObject, CardCutout cutout)
    {
        chosenDeck.RemoveCard(collectionCardObject.Card);
        collectionCardObject.gameObject.SetActive(true);

        cutouts.Remove(cutout);
        //do some resetting of collection/gray cards, reposition the rest of the cards in the panel
        RenderDecklist();
    }

    private void RenderDecklist()
    {
        for (int i = 0; i < cutouts.Count; ++i)
        {
            cutouts[i].PositionCutout(i);
        }
    }

    private void SaveCollectionRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("CreateUpdatePlayerDeck");
        List<string> cardIds = new List<string>();
        foreach (Card card in chosenDeck.Cards)
        {
            cardIds.Add(card.Id);
        }
        request.SetEventAttribute("cardIds", cardIds);
        request.SetEventAttribute("previousName", chosenDeck.Name);
        request.SetEventAttribute("name", chosenDeck.Name);
        request.Send(SaveCollectionSuccess, Error);
    }

    private void SaveCollectionSuccess(LogEventResponse resp)
    {
        Debug.Log("Successfully saved deck.");
    }

    private void Error(LogEventResponse resp)
    {
        Debug.Log("Gamesparks Request Error!");
    }

    private void CreateCardObjects()
    {
        int index = 0;
        int rowSize = 4;

        Vector3 topLeft = new Vector3(-5.05f, 3.11f, 18.18f);
        Vector3 horizontalOffset = new Vector3(2.64f, 0f, 0f);
        Vector3 verticalOffset = new Vector3(0f, -3.75f, 0f);

        Transform grayed = new GameObject("Grayed").transform as Transform;
        foreach (Card card in collection)
        {
            GameObject created = new GameObject(card.Name);
            created.transform.parent = collectionObject.transform;
            CollectionCardObject collectionCardObject = created.AddComponent<CollectionCardObject>();
            collectionCardObject.Initialize(card);
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

        //do visual stuff
        CollectionCardObject collectionCardObject = created.AddComponent<CollectionCardObject>();
        collectionCardObject.Initialize(card);
        collectionCardObject.visual.SetGrayscale(true);
        return created.transform;
    }

    private void MinifyCard(CollectionCardObject collectionCardObject)
    {
        if (!collectionCardObject.minified)
            collectionCardObject.Minify(true);
    }

    private void RevertMinify(CollectionCardObject collectionCardObject)
    {
        if (collectionCardObject.minified)
            collectionCardObject.Minify(false);
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
}
