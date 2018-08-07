﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class CancelableListItem : CardListItem
{
    [SerializeField]
    private Button cancelAuctionButton;

    [SerializeField]
    protected CardAuction cardAuction;

    public new void Awake()
    {
        base.Awake();
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
        MenuManager.Instance.UMP.ShowConfirmationDialog(
            "Confirm Withdrawal",
            "Are you sure you would like to withdraw your auction? You'll be asked to authorize the transaction next.",
            new UnityAction(AuthorizeWithdrawAuction),
            new UnityAction(CancelWithdrawAuction)
        );
    }

    private void AuthorizeWithdrawAuction()
    {
        PrivateKeyModal.Instance.ShowModalWithCallback(
            new UnityAction<Account>(SubmitWithdrawAuction)
        );
    }

    private void SubmitWithdrawAuction(Account account)
    {
        CryptoSingleton.Instance.CancelAuction(
            account,
            Int32.Parse(cardAuction.GetId().Substring(1))
        );
    }

    private void CancelWithdrawAuction()
    {

    }
}
