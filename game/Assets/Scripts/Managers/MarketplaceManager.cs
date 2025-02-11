﻿using System.Collections.Generic;
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
    private GameObject cancelableCardListItem;

    [SerializeField]
    private Transform contentPanel;

    private List<CardAuction> buyableCards;
    private List<Card> sellableCards;
    private List<CardAuction> cancelableCards;

    //Pooling
    private Stack<BuyableCardListItem> buyableListItemPool;
    private Stack<SellableCardListItem> sellableListItemPool;
    private Stack<CancelableListItem> cancelableListItemPool;
    //End Pooling
    private const int LIST_ITEM_POOL_SIZE = 30;
    private object listItemObject;

    public GameObject marketplacePreviewSummon;
    public List<Button> marketplaceButtons;

    public static MarketplaceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        this.buyModeButton.onClick.AddListener(OnBuyModeButtonClick);
        this.sellModeButton.onClick.AddListener(OnSellModeButtonClick);
        this.cancelModeButton.onClick.AddListener(OnCancelModeButtonClick);

        this.buyableCards = new List<CardAuction>();
        this.sellableCards = new List<Card>();
        this.cancelableCards = new List<CardAuction>();
    }

    private void Start()
    {
        if (SparkSingleton.Instance.IsAuthenticated)
        {
            GetMarketplaceData();
        }
        else
        {
            SparkSingleton.Instance.AddAuthenticationCallback(GetMarketplaceData);
        }

        buyableListItemPool = new Stack<BuyableCardListItem>();
        sellableListItemPool = new Stack<SellableCardListItem>();
        cancelableListItemPool = new Stack<CancelableListItem>();

        for (int i = 0; i < LIST_ITEM_POOL_SIZE; ++i)
        {
            BuyableCardListItem buyable = Instantiate(buyableCardListItem).GetComponent<BuyableCardListItem>();
            buyable.transform.SetParent(this.transform, false);
            buyableListItemPool.Push(buyable);

            SellableCardListItem sellable = Instantiate(sellableCardListItem).GetComponent<SellableCardListItem>();
            sellable.transform.SetParent(this.transform, false);
            sellableListItemPool.Push(sellable);

            CancelableListItem cancelable = Instantiate(cancelableCardListItem).GetComponent<CancelableListItem>();
            cancelable.transform.SetParent(this.transform, false);
            cancelableListItemPool.Push(cancelable);
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
        ClearListItems();
        this.mode = MARKETPLACE_MODE_BUY;
        RenderListItems();
    }

    private void OnSellModeButtonClick()
    {
        ClearListItems();
        this.mode = MARKETPLACE_MODE_SELL;
        RenderListItems();
    }

    private void OnCancelModeButtonClick()
    {
        ClearListItems();
        this.mode = MARKETPLACE_MODE_CANCEL;
        RenderListItems();
    }

    private void ClearListItems()
    {
        if (this.mode == MARKETPLACE_MODE_BUY)
        {
            BuyableCardListItem[] buyableCardListItems = contentPanel.GetComponentsInChildren<BuyableCardListItem>();
            if (buyableCardListItems == null)
            {
                Debug.LogError("Null!");
            }

            foreach (BuyableCardListItem item in buyableCardListItems)
            {
                SetBuyableListItemToPool(item);
            }
        }
        else if (this.mode == MARKETPLACE_MODE_SELL)
        {
            foreach (SellableCardListItem item in contentPanel.GetComponentsInChildren<SellableCardListItem>())
            {
                SetSellableListItemToPool(item);
            }
        }
        else
        {
            foreach (CancelableListItem item in contentPanel.GetComponentsInChildren<CancelableListItem>())
            {
                SetCancelableItemToPool(item);
            }
        }
    }

    private void RenderListItems()
    {
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
            CreateCancelableCardListItems(this.cancelableCards);
        }
    }

    private void CreateBuyableCardListItems(List<CardAuction> cardAuctions)
    {
        foreach (CardAuction cardAuction in cardAuctions)
        {
            BuyableCardListItem listItem = GetBuyableListItemFromPool();
            listItem.InitializeCardAuction(cardAuction);
            listItem.transform.SetParent(contentPanel, false);
            listItem.gameObject.SetActive(true);
            //listItem.transform.localScale = Vector3.one;
        }
    }

    private void CreateCancelableCardListItems(List<CardAuction> auctionedCards)
    {
        foreach (CardAuction auctionedCard in auctionedCards)
        {
            CancelableListItem listItem = GetCancelableListItemFromPool();
            listItem.InitializeCardAuction(auctionedCard);
            listItem.transform.SetParent(contentPanel, false);
            listItem.gameObject.SetActive(true);
            //listItem.transform.localScale = Vector3.one;
        }
    }

    private void CreateSellableCardListItems(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            SellableCardListItem listItem = GetSellableListItemFromPool();
            listItem.Initialize(card);
            listItem.transform.SetParent(contentPanel, false);
            listItem.gameObject.SetActive(true);
            //listItem.transform.localScale = Vector3.one;
        }
    }

    //Pooling getters and setters
    public BuyableCardListItem GetBuyableListItemFromPool()
    {
        if (buyableListItemPool.Count <= 0)
        {
            GameObject created = Instantiate(this.buyableCardListItem, transform.position, Quaternion.identity);
            return created.GetComponent<BuyableCardListItem>();
        }
        BuyableCardListItem chosen = buyableListItemPool.Pop();
        chosen.gameObject.SetActive(true);
        return chosen;
    }

    public void SetBuyableListItemToPool(BuyableCardListItem buyableListItem)
    {
        buyableListItem.transform.SetParent(this.transform, false);
        buyableListItemPool.Push(buyableListItem);
        buyableListItem.gameObject.SetActive(false);
    }

    public SellableCardListItem GetSellableListItemFromPool()
    {
        if (sellableListItemPool.Count <= 0)
        {
            GameObject created = Instantiate(this.sellableCardListItem, transform.position, Quaternion.identity);
            return created.GetComponent<SellableCardListItem>();
        }
        SellableCardListItem chosen = sellableListItemPool.Pop();
        chosen.gameObject.SetActive(true);
        return chosen;
    }

    public void SetSellableListItemToPool(SellableCardListItem sellableListItem)
    {
        sellableListItem.transform.SetParent(this.transform, false);
        sellableListItemPool.Push(sellableListItem);
        sellableListItem.gameObject.SetActive(false);
    }

    public CancelableListItem GetCancelableListItemFromPool()
    {
        if (cancelableListItemPool.Count <= 0)
        {
            GameObject created = Instantiate(this.cancelableCardListItem, transform.position, Quaternion.identity);
            return created.GetComponent<CancelableListItem>();
        }
        CancelableListItem chosen = cancelableListItemPool.Pop();
        chosen.gameObject.SetActive(true);
        return chosen;
    }

    public void SetCancelableItemToPool(CancelableListItem cancelableListItem)
    {
        cancelableListItem.transform.SetParent(this.transform, false);
        cancelableListItemPool.Push(cancelableListItem);
        cancelableListItem.gameObject.SetActive(false);
    }

    public void HoverEnterEffect(GameObject target)
    {
        //LeanTween.scale(target, Vector3.one * 1.1f, TWEEN_DURATION).setEaseInQuad();
        target.transform.localScale = Vector3.one * 1.15f;
    }

    public void HoverExitEffect(GameObject target)
    {
        //LeanTween.scale(target, Vector3.one, TWEEN_DURATION).setEaseInQuad();
        target.transform.localScale = Vector3.one;
    }

    public void SetMarketplacePreview(Card card)
    {
        if (card == null)
        {
            Debug.LogError("Received null for card in SetMarketplacePreview()");
            return;
        }

        this.marketplacePreviewSummon.SetActive(false);  //assumes that initial value is set via inspector
        Vector3 position = this.marketplacePreviewSummon.transform.position;

        this.marketplacePreviewSummon = SummonPoolManager.Instance.GetSummonFromPool(card.GetName());
        this.marketplacePreviewSummon.transform.position = position;
        this.marketplacePreviewSummon.transform.localScale = new Vector3(4, 5, 1);
        this.marketplacePreviewSummon.transform.LookAt(Camera.main.transform);
        this.marketplacePreviewSummon.SetActive(true);
    }

    public void MarketPillButton(GameObject source)
    {
        for (int i = 0; i < marketplaceButtons.Count; i++)
        {
            ColorBlock mcb = marketplaceButtons[i].colors;

            if (i % 2 == 0)
            {
                mcb.normalColor = new Color(0.1f, 0.1f, 0.1f);
            }
            else
            {
                mcb.normalColor = Color.black;
            }
            marketplaceButtons[i].colors = mcb;
        }

        Button button = source.GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(0.08f, 0f, 0.63f);
        button.colors = cb;
    }
}
