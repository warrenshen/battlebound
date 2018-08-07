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
        MenuManager.Instance.UMP.ShowConfirmationDialog(
            "Confirm Bid",
            "Are you sure you would like to bid for this card? You'll be asked to authorize the transaction next.",
            new UnityAction(AuthorizeBidAuction),
            new UnityAction(DoNothing)
        );
    }

    private void AuthorizeBidAuction()
    {
        MenuManager.Instance.UMP.ShowInputFieldDialog<Account>(
            "Authorize Transaction",
            "Please enter your password to verify transaction.",
            new UnityAction<Account>(SubmitBidAuction),
            new UnityAction<Account>(DoNothing)
        );
    }

    private void DoNothing()
    {

    }
    private void DoNothing(Account account)
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
