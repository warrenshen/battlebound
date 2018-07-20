using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class MarketplaceManager : MonoBehaviour
{
    public static int MARKETPLACE_MODE_BUY = 0;
    public static int MARKETPLACE_MODE_SELL = 1;
    public static int MARKETPLACE_MODE_CANCEL = 2;

    private int mode = MARKETPLACE_MODE_BUY;

    [SerializeField]
    private Button buyModeButton;
    [SerializeField]
    private Button sellModeButton;
    [SerializeField]
    private Button cancelModeButton;

    [SerializeField]
    private GameObject buyableCardListItem;
    [SerializeField]
    private GameObject sellableCardListItem;
    [SerializeField]
    private GameObject cancelableCardlistItem;

    [SerializeField]
    private Transform contentPanel;

    private List<CardAuction> buyableCards;
    private List<CardRaw> sellableCards;
    private List<CardAuction> cancelableCards;

    public static MarketplaceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.buyModeButton.onClick.AddListener(OnBuyModeButtonClick);
        this.sellModeButton.onClick.AddListener(OnSellModeButtonClick);
        this.cancelModeButton.onClick.AddListener(OnCancelModeButtonClick);
    }

    private void Start()
    {
        buyableCards = new List<CardAuction>();
        sellableCards = new List<CardRaw>();
        cancelableCards = new List<CardAuction>();

        GetCardAuctions();
        GetPlayerCardAuctions();
    }

    private void OnBuyModeButtonClick()
    {
        this.mode = MARKETPLACE_MODE_BUY;
        RenderListItems();
    }

    private void OnSellModeButtonClick()
    {
        this.mode = MARKETPLACE_MODE_SELL;
        RenderListItems();
    }

    private void OnCancelModeButtonClick()
    {
        this.mode = MARKETPLACE_MODE_CANCEL;
        RenderListItems();
    }

    private void RenderListItems()
    {
        foreach (Transform child in this.contentPanel)
        {
            Debug.Log(child.gameObject);
            Destroy(child.gameObject);
        }

        if (this.mode == MARKETPLACE_MODE_BUY)
        {
            CreateBuyableCardListItems(this.buyableCards);
        }
        else if (this.mode == MARKETPLACE_MODE_SELL)
        {
            CreateSellableCardListItems(this.sellableCards);
        }
        else
        {
            // Render player's auctioned cards.
            CreateCancelableCardListItems(this.cancelableCards);
        }
    }

    private void CreateBuyableCardListItems(List<CardAuction> cardAuctions)
    {
        foreach (CardAuction cardAuction in cardAuctions)
        {
            GameObject listItemGO = Instantiate(buyableCardListItem) as GameObject;
            BuyableCardListItem listItem = listItemGO.GetComponent<BuyableCardListItem>();
            listItem.InitializeCardAuction(cardAuction);
            listItemGO.transform.SetParent(contentPanel);
        }
    }

    private void CreateCancelableCardListItems(List<CardAuction> auctionedCards)
    {
        foreach (CardAuction auctionedCard in auctionedCards)
        {
            GameObject listItemGO = Instantiate(cancelableCardlistItem) as GameObject;
            CancelableListItem listItem = listItemGO.GetComponent<CancelableListItem>();
            listItem.InitializeCardAuction(auctionedCard);
            listItemGO.transform.SetParent(contentPanel);
        }
    }

    private void CreateSellableCardListItems(List<CardRaw> cardRaws)
    {
        foreach (CardRaw card in cardRaws)
        {
            GameObject listItemGO = Instantiate(sellableCardListItem) as GameObject;
            SellableCardListItem listItem = listItemGO.GetComponent<SellableCardListItem>();
            listItem.Initialize(card);
            listItemGO.transform.SetParent(contentPanel);
        }
    }

    private void GetCardAuctions()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetCardAuctions");
        request.Send(OnGetCardAuctionsSuccess, OnGetCardAuctionsError);
    }

    private void OnGetCardAuctionsSuccess(LogEventResponse response)
    {
        // JSON list of card auctions.
        string address = response.ScriptData.GetString("address");
        int balance = (int)response.ScriptData.GetInt("balance");
        Debug.Log(address);
        Debug.Log(balance);
        List<GSData> dataList = response.ScriptData.GetGSDataList("auctions");
        Debug.Log(dataList.Count + " auctions found.");

        buyableCards = new List<CardAuction>();
        foreach (GSData data in dataList)
        {
            CardAuction cardAuction = JsonUtility.FromJson<CardAuction>(data.JSON);
            buyableCards.Add(cardAuction);
        }
    }

    private void OnGetCardAuctionsError(LogEventResponse response)
    {
        GSData errors = response.Errors;
        Debug.Log(errors);
    }

    private void GetPlayerCardAuctions()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerCardAuctions");
        request.Send(OnGetPlayerCardAuctionsSuccess, OnGetPlayerCardAuctionsError);
    }

    private void OnGetPlayerCardAuctionsSuccess(LogEventResponse response)
    {
        // JSON list of card auctions.
        List<GSData> auctionableDataList = response.ScriptData.GetGSDataList("auctionableCards");
        Debug.Log(auctionableDataList.Count + " auctionable cards found.");

        List<GSData> auctionedDataList = response.ScriptData.GetGSDataList("auctionedCards");
        Debug.Log(auctionedDataList.Count + " auctioned cards found.");

        sellableCards = new List<CardRaw>();
        foreach (GSData data in auctionableDataList)
        {
            CardRaw card = JsonUtility.FromJson<CardRaw>(data.JSON);
            sellableCards.Add(card);
        }

        cancelableCards = new List<CardAuction>();
        foreach (GSData data in auctionedDataList)
        {
            CardAuction cardAuction = JsonUtility.FromJson<CardAuction>(data.JSON);
            cancelableCards.Add(cardAuction);
        }
    }

    private void OnGetPlayerCardAuctionsError(LogEventResponse response)
    {
        GSData errors = response.Errors;
        Debug.Log(errors);
    }
}
