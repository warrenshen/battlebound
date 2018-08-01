using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class CancelableListItem : CardListItem
{
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Button cancelAuctionButton;

    [SerializeField]
    protected CardAuction cardAuction;

    public void Awake()
    {
        this.cancelAuctionButton.onClick.AddListener(OnCancelAuctionButtonClick);
    }

    public void InitializeCardAuction(CardAuction cardAuction)
    {
        this.cardAuction = cardAuction;
        this.card = cardAuction.Card;

        Texture2D texture = ResourceSingleton.Instance.GetImageTextureByName(cardAuction.Card.GetFrontImage());
        this.cardImage.sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
    }

    public void OnCancelAuctionButtonClick()
    {
        GenericModalPanel.Instance.Show(
            "Confirm you would like to cancel this auction? You'll authorize a transaction next.",
            new UnityAction(AuthorizeCancelAuction),
            new UnityAction(CancelBidAuction)
        );
    }

    private void AuthorizeCancelAuction()
    {
        PrivateKeyModal.Instance.ShowModalWithCallback(
            new UnityAction<Account>(SubmitCancelAuction)
        );
    }

    private void SubmitCancelAuction(Account account)
    {
        CryptoSingleton.Instance.CancelAuction(
            account,
            Int32.Parse(cardAuction.GetId().Substring(1))
        );
    }

    private void CancelBidAuction()
    {
    }
}
