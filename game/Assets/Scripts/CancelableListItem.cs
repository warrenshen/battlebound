using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CancelableListItem : MonoBehaviour
{
	[SerializeField]
	private Image cardImage;
	[SerializeField]
	private Button cancelAuctionButton;

	private CardAuction cardAuction;
    
	public void Awake()
	{
		this.cancelAuctionButton.onClick.AddListener(OnCancelAuctionButtonClick);
	}

	public void InitializeCardAuction(CardAuction cardAuction)
    {
        this.cardAuction = cardAuction;

		Texture2D texture = Resources.Load(cardAuction.Image) as Texture2D;
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
            "Confirm cancel?",
			new UnityAction(ConfirmCancelAuction),
            new UnityAction(CancelBidAuction)
        );
    }

	private void ConfirmCancelAuction()
    {
		CryptoSingleton.Instance.CancelAuction(
            Int32.Parse(cardAuction.Id.Substring(1))
        );
    }

    private void CancelBidAuction()
    {
    }
}
