using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class BuyableCardListItem : MonoBehaviour
{
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Button bidAuctionButton;

    private CardAuction cardAuction;

    public void Awake()
    {
        this.bidAuctionButton.onClick.AddListener(OnBidAuctionButtonClick);
    }

    public void InitializeCardAuction(CardAuction cardAuction)
    {
        this.cardAuction = cardAuction;
        Debug.Log(cardAuction.Auction.StartingPrice);
        Debug.Log(cardAuction.Auction.EndingPrice);
        Debug.Log(cardAuction.Auction.Duration);
        Debug.Log(cardAuction.Auction.StartedAt);

        //Texture2D texture = Resources.Load("HS/" + card.Image) as Texture2D;
        Texture2D texture = Resources.Load("HS/Armorsmith") as Texture2D;
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
            Int32.Parse(cardAuction.Id.Substring(1)),
            1000L
        );
    }
}
