using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreateAuctionModalPanel : MonoBehaviour
{
	private static CreateAuctionModalPanel modalPanel;

	private Card card;

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
	[SerializeField]
    private GameObject modalPanelObject;

    //TO-DO: NEEDS REVIEW, possible creation of duplicate due to already existing copy via monobehavior and placement in scene
	public static CreateAuctionModalPanel Instance()
	{
		if (!modalPanel)
		{
			modalPanel = FindObjectOfType(typeof(CreateAuctionModalPanel)) as CreateAuctionModalPanel;
			Debug.Log(modalPanel);
			if (!modalPanel)
			{
				Debug.LogError("There needs to be a CreateAuctionModalPanel script on a GameObject in your scene.");
			}
		}

		return modalPanel;
	}

	public void Awake()
	{
		cancelButton.onClick.AddListener(HideModal);
		submitButton.onClick.AddListener(SubmitTransaction);
	}

	public void HideModal()
	{
		modalPanelObject.SetActive(false);	
	}

	public void ShowModalForCard(Card card)
	{
		this.card = card;
		Texture2D image = Resources.Load(card.Image) as Texture2D;
		cardImage.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
		modalPanelObject.SetActive(true);
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

		HideModal();

		return txHash;
	}
}
