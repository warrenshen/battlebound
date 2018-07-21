using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Nethereum.Web3.Accounts;

public class CreateAuctionModalPanel : MonoBehaviour
{
    private CardRaw card;

    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private InputField startingPriceInputField;
    [SerializeField]
    private InputField endingPriceInputField;
    [SerializeField]
    private InputField durationInputField;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button submitButton;

    public static CreateAuctionModalPanel Instance { get; private set; }

    public void Awake()
    {
        Instance = this;

        cancelButton.onClick.AddListener(Close);
        submitButton.onClick.AddListener(AuthorizeBidAuction);

        Close();
    }

    private void Close()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowModalForCard(CardRaw card)
    {
        Card aCard;
        if (card.Category == CardRaw.CardType.Creature)
        {
            aCard = new CreatureCard(card.Id, card.Name, 1);

            this.card = card;
            Texture2D image = ResourceSingleton.Instance.GetImageTextureByName(aCard.GetFrontImage());
            cardImage.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
            this.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Only suppport creature cards now");
        }
    }

    public void AuthorizeBidAuction()
    {
        if (this.card == null)
        {
            Debug.LogError("Card does not exist - did you forget to call InitializeCard?");
        }

        int tokenId = Convert.ToInt32(card.Id.Substring(1));
        long startingPrice = Convert.ToInt64(startingPriceInputField.text);
        long endingPrice = Convert.ToInt64(endingPriceInputField.text);
        int duration = Convert.ToInt32(durationInputField.text);

        // TODO: validations.

        PrivateKeyModal.Instance.ShowModalWithCallback(
            new UnityAction<Account>(SubmitCreateAuctionTransaction)
        );
    }

    private void SubmitCreateAuctionTransaction(Account account)
    {
        int tokenId = Convert.ToInt32(card.Id.Substring(1));
        long startingPrice = Convert.ToInt64(startingPriceInputField.text);
        long endingPrice = Convert.ToInt64(endingPriceInputField.text);
        int duration = Convert.ToInt32(durationInputField.text);

        CryptoSingleton.Instance.CreateAuction(
            account,
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );

        //Close();

        //return txHash;
    }
}
