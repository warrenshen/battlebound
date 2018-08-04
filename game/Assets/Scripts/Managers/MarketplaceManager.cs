using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class MarketplaceManager : MonoBehaviour
{
    public const int MARKETPLACE_MODE_BUY = 0;
    public const int MARKETPLACE_MODE_SELL = 1;
    public const int MARKETPLACE_MODE_CANCEL = 2;

    private int mode = MARKETPLACE_MODE_BUY;

    private string contractAddressTreasury;
    public string ContractAddressTreasury => contractAddressTreasury;

    private string contractAddressAuction;
    public string ContractAddressAuction => contractAddressAuction;

    private string gasPriceSuggested;
    public string GasPriceSuggested => gasPriceSuggested;

    public HyperCard.Card showcaseCard;

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
    private List<Card> sellableCards;
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
        sellableCards = new List<Card>();
        cancelableCards = new List<CardAuction>();

        if (SparkSingleton.Instance.IsAuthenticated)
        {
            GetMarketplaceData();
        }
        else
        {
            SparkSingleton.Instance.AddAuthenticatedCallback(GetMarketplaceData);
        }

        ShowBuyMode();
    }


    private void GetMarketplaceData()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetMarketplaceData");
        request.Send(OnGetMarketplaceDataSuccess, OnGetMarketplaceDataError);
    }

    private void OnGetMarketplaceDataSuccess(LogEventResponse response)
    {
        this.contractAddressTreasury = response.ScriptData.GetString("contractAddressTreasury");
        this.contractAddressAuction = response.ScriptData.GetString("contractAddressAuction");
        this.gasPriceSuggested = response.ScriptData.GetString("gasPriceSuggested");

        List<GSData> buyableCardsDataList = response.ScriptData.GetGSDataList("buyableCards");
        List<GSData> cancelableCardsDataList = response.ScriptData.GetGSDataList("cancelableCards");
        List<GSData> sellableCardsDataList = response.ScriptData.GetGSDataList("sellableCards");

        this.buyableCards = new List<CardAuction>();
        foreach (GSData buyableCardData in buyableCardsDataList)
        {
            CardAuction cardAuction = CardAuction.GetFromJson(buyableCardData.JSON);
            this.buyableCards.Add(cardAuction);
        }

        this.cancelableCards = new List<CardAuction>();
        foreach (GSData cancelableCardData in cancelableCardsDataList)
        {
            CardAuction cardAuction = CardAuction.GetFromJson(cancelableCardData.JSON);
            this.cancelableCards.Add(cardAuction);
        }

        this.sellableCards = DeckStore.Instance().ParseCardsFromScriptData(sellableCardsDataList);
        ShowBuyMode();
    }

    private void OnGetMarketplaceDataError(LogEventResponse response)
    {
        GSData errors = response.Errors;
        Debug.Log(errors);
    }

    private void OnBuyModeButtonClick()
    {
        ShowBuyMode();
    }

    private void ShowBuyMode()
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
            GameObject listItemObject = Instantiate(buyableCardListItem) as GameObject;
            BuyableCardListItem listItem = listItemObject.GetComponent<BuyableCardListItem>();
            listItem.InitializeCardAuction(cardAuction);
            listItemObject.transform.SetParent(contentPanel);
            listItemObject.transform.localScale = Vector3.one;
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

    private void CreateSellableCardListItems(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            GameObject listItemGO = Instantiate(sellableCardListItem) as GameObject;
            SellableCardListItem listItem = listItemGO.GetComponent<SellableCardListItem>();
            listItem.Initialize(card);
            listItemGO.transform.SetParent(contentPanel);
        }
    }
}
