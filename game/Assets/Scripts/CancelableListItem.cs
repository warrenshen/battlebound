using System;
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
        MenuManager.Instance.UMP.ShowInputFieldDialog(
            "Authorize Withdrawal",
            "Please enter your password to verify the transaction.",
            new UnityAction<string, UMP_InputDialogUI>(SubmitWithdrawAuction),
            new UnityAction(CancelWithdrawAuction)
        );
    }

    private void SubmitWithdrawAuction(string password, UMP_InputDialogUI dialog)
    {
        if (!PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_ENCRYPTED_KEY_STORE))
        {
            dialog.SetMessage("Ethereum hot wallet not configured for this device.");
            dialog.SetMessageColor(Color.red);
            return;
        }
        else if (!PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS))
        {
            dialog.SetMessage("Ethereum hot wallet not configured for this device.");
            dialog.SetMessageColor(Color.red);
            return;
        }

        Account account = CryptoSingleton.Instance.GetAccountWithPassword(password);

        string publicAddress = PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS);
        if (publicAddress != account.Address)
        {
            dialog.SetMessage("Entered password incorrect! Please try again.");
            dialog.SetMessageColor(Color.red);
            return;
        }
        else
        {
            //Actual end result/payload begin
            CryptoSingleton.Instance.CancelAuction(
                account,
                Int32.Parse(cardAuction.GetId().Substring(1))
            );
            //Actual end result/payload end
        }
        dialog.Close();
    }

    private void CancelWithdrawAuction()
    {

    }
}
