using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class SellableCardListItem : CardListItem
{
    [SerializeField]
    private Button createAuctionButton;

    private void Awake()
    {
        this.createAuctionButton.onClick.AddListener(OnCreateAuctionButtonClick);
    }

    public void Initialize(Card card)
    {
        if (card == null)
        {
            Debug.LogError("Invalid card auction parameter.");
            return;
        }

        this.card = card;

        Texture2D texture = ResourceSingleton.Instance.GetImageTextureByName(card.GetFrontImage());
        this.cardImage.sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
    }

    public void OnCreateAuctionButtonClick()
    {
        UMPSingleton.Instance.ShowThreeInputFieldDialog(
            "Create Auction",
            "Input your price spread and auction length to continue.",
            "Initial Price",
            "Final Price",
            "Duration",
            new UnityAction<UMP_ThreeInputDialogUI, string, string, string>(SubmitCreateAuctionTransaction),
            () => { },
            contentTypeOne: InputField.ContentType.IntegerNumber,
            contentTypeTwo: InputField.ContentType.IntegerNumber,
            contentTypeThree: InputField.ContentType.IntegerNumber
        );
    }

    private void SubmitCreateAuctionTransaction(
        UMP_ThreeInputDialogUI dialog,
        string startPrice,
        string endPrice,
        string auctionDuration
    )
    {
        int tokenId = Convert.ToInt32(card.Id.Substring(1));
        long startingPrice = Convert.ToInt64(startPrice);
        long endingPrice = Convert.ToInt64(endPrice);
        int duration = Convert.ToInt32(auctionDuration);

        UMPSingleton.Instance.ShowInputFieldDialog(
            "Authorize Auction",
            "Please enter your password to verify the transaction.",
            new UnityAction<UMP_InputDialogUI, string>(
                (UMP_InputDialogUI dialogUI, string password) => ProcessPassword(dialogUI, password, tokenId, startingPrice, endingPrice, duration)
            ),
            () => { },
            placeholderMessage: "Enter password...",
            contentType: InputField.ContentType.Password
        );

        dialog.Close();
    }

    private void ProcessPassword(
        UMP_DialogUI dialog,
        string password,
        int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
    )
    {
        ProcessPasswordHelper(
            dialog,
            password,
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );
    }

    private async Task ProcessPasswordHelper(
        UMP_DialogUI dialog,
        string password,
        int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
    )
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
        }
        else
        {
            dialog.Close();

            string txHash = await CryptoSingleton.Instance.CreateAuction(
                account,
                tokenId,
                startingPrice,
                endingPrice,
                duration
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
