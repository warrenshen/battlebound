using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreateAuctionModalPanel : MonoBehaviour
{
	private static CreateAuctionModalPanel modalPanel;

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
		submitButton.onClick.AddListener(SubmitTransaction);

		Close();
	}
    
	private void Close()
	{
		this.gameObject.SetActive(false);	
	}

	public void ShowModalForCard(CardRaw card)
	{
		this.card = card;
		Texture2D image = Resources.Load(card.Image) as Texture2D;
		cardImage.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
		this.gameObject.SetActive(true);
	}
    
    public void SubmitTransaction()
	{
		if (this.card == null)
		{
			Debug.LogError("Card does not exist - did you forget to call InitializeCard?");
		}

		int tokenId = Convert.ToInt32(card.Id.Substring(1));
		long startingPrice = Convert.ToInt64(startingPriceInputField.text);
		long endingPrice = Convert.ToInt64(endingPriceInputField.text);
		int duration = Convert.ToInt32(durationInputField.text);

		SubmitTransactionHelper(
			tokenId,
			startingPrice,
			endingPrice,
			duration
		);
	}
    
	private async Task<string> SubmitTransactionHelper(
		int tokenId,
        long startingPrice,
        long endingPrice,
        int duration
	)
	{
        string txHash = await CryptoSingleton.Instance.CreateAuction(
            tokenId,
            startingPrice,
            endingPrice,
            duration
        );

		Close();

		return txHash;
	}
}
