using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        UMPSingleton.Instance.ShowConfirmationDialog(
            "Confirm Bid",
            "Are you sure you would like to bid for this card? You'll be asked to authorize the transaction next.",
            new UnityAction(AuthorizeBidAuction),
            new UnityAction(CancelAction)
        );
    }

    private void AuthorizeBidAuction()
    {
        UMPSingleton.Instance.ShowInputFieldDialog(
            "Authorize Bid",
            "Please enter your password to verify the transaction.",
            new UnityAction<UMP_InputDialogUI, string>(SubmitBidAuction),
            new UnityAction(CancelAction),
            placeholderMessage: "Enter password...",
            contentType: InputField.ContentType.Password
        );
    }

    private void CancelAction()
    {

    }

    private void SubmitBidAuction(UMP_InputDialogUI dialog, string password)
    {
        if (!PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_ENCRYPTED_KEY_STORE))
        {
            dialog.SetMessage("Ethereum hot wallet not configured for this device.");
            dialog.SetMessageColor(Color.red);
            dialog.SetMessageStyle(FontStyle.Bold);
            return;
        }
        else if (!PlayerPrefs.HasKey(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS))
        {
            dialog.SetMessage("Ethereum hot wallet not configured for this device.");
            dialog.SetMessageColor(Color.red);
            dialog.SetMessageStyle(FontStyle.Bold);
            return;
        }

        Account account = CryptoSingleton.Instance.GetAccountWithPassword(password);

        string publicAddress = PlayerPrefs.GetString(CryptoSingleton.PLAYER_PREFS_PUBLIC_ADDRESS);
        if (publicAddress != account.Address)
        {
            dialog.SetMessage("Entered password incorrect! Please try again.");
            dialog.SetMessageColor(Color.red);
            dialog.SetMessageStyle(FontStyle.Bold);
            return;
        }
        else
        {
            //Actual end result/payload begin
            CryptoSingleton.Instance.BidAuction(
                account,
                Int32.Parse(cardAuction.GetId().Substring(1)),
                1000L
            );
            //Actual end result/payload end
        }
        dialog.Close();
    }
}