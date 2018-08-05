using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using Nethereum.Web3.Accounts;

public class BuyableCardListItem : CardListItem
{
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Button bidAuctionButton;

    [SerializeField]
    protected CardAuction cardAuction;

    public new void Awake()
    {
        base.Awake();
        this.bidAuctionButton.onClick.AddListener(OnBidAuctionButtonClick);
    }

    public void InitializeCardAuction(CardAuction cardAuction)
    {
        this.cardAuction = cardAuction;
        this.card = cardAuction.Card;

        Debug.Log(cardAuction.Auction.StartingPrice);
        Debug.Log(cardAuction.Auction.EndingPrice);
        Debug.Log(cardAuction.Auction.Duration);
        Debug.Log(cardAuction.Auction.StartedAt);

        Texture2D texture = ResourceSingleton.Instance.GetImageTextureByName(cardAuction.Card.GetFrontImage());
        this.cardImage.sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );

        this.priceText.text = cardAuction.Auction.StartingPrice.ToString();
    }

    public void OnBidAuctionButtonClick()
    {
        GenericModalPanel.Instance.Show(
            "Confirm you would like to buy this card? You'll authorize a transaction next.",
            new UnityAction(AuthorizeBidAuction),
            new UnityAction(CancelBidAuction),
            "Proceed",
            "Cancel"
        );
    }

    private void AuthorizeBidAuction()
    {
        PrivateKeyModal.Instance.ShowModalWithCallback(
            new UnityAction<Account>(SubmitBidAuction)
        );
    }

    private void CancelBidAuction()
    {
    }

    private void SubmitBidAuction(Account account)
    {
        CryptoSingleton.Instance.BidAuction(
            account,
            Int32.Parse(cardAuction.GetId().Substring(1)),
            1000L
        );
    }


}
