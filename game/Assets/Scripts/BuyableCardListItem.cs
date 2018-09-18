using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class BuyableCardListItem : CardListItem
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text priceText;
    [SerializeField]
    private Button bidAuctionButton;

    [SerializeField]
    protected CardAuction cardAuction;

    private void Awake()
    {
        this.bidAuctionButton.onClick.AddListener(OnBidAuctionButtonClick);
    }

    public void InitializeCardAuction(CardAuction cardAuction)
    {
        if (cardAuction == null)
        {
            Debug.LogError("Invalid card auction parameter.");
            return;
        }
        else if (cardAuction.Card == null)
        {
            Debug.LogError("Invalid card parameter.");
            return;
        }

        this.cardAuction = cardAuction;
        this.card = cardAuction.Card;

        Texture2D texture = ResourceSingleton.Instance.GetImageTextureByName(this.card.GetFrontImage());
        this.cardImage.sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );

        this.nameText.text = this.card.GetName();
        this.priceText.text = this.cardAuction.Auction.StartingPrice.ToString();
    }

    public void OnBidAuctionButtonClick()
    {
        UMPSingleton.Instance.ShowConfirmationDialog(
            "Confirm Bid",
            "Are you sure you would like to bid for this card? You'll be asked to authorize the transaction next.",
            new UnityAction(AuthorizeBidAuction),
            () => { }
        );
    }

    private void AuthorizeBidAuction()
    {
        UMPSingleton.Instance.ShowInputFieldDialog(
            "Authorize Bid",
            "Please enter your password to verify the transaction.",
            new UnityAction<UMP_InputDialogUI, string>(SubmitBidAuction),
            () => { },
            placeholderMessage: "Enter password...",
            contentType: InputField.ContentType.Password
        );
    }

    private void SubmitBidAuction(UMP_InputDialogUI dialog, string password)
    {
        SubmitBidAuctionHelper(dialog, password);
    }

    private async Task SubmitBidAuctionHelper(UMP_InputDialogUI dialog, string password)
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
            dialog.Close();

            string txHash = await CryptoSingleton.Instance.BidAuction(
                account,
                Int32.Parse(cardAuction.GetId().Substring(1)),
                this.cardAuction.Auction.StartingPrice
            );

            UMPSingleton.Instance.ShowConfirmationDialog(
                "Transaction Hash",
                txHash,
                () => { },
                null,
                "Continue",
                null
            );
        }
    }
}
