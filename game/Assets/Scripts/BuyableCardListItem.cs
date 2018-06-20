using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyableCardListItem : MonoBehaviour
{
	[SerializeField]
    private Image cardImage;
	[SerializeField]
	private Text priceText;
    [SerializeField]
    private Button bidAuctionButton;
    
	private CardAuction cardAuction;

	public void Awake()
    {
		this.bidAuctionButton.onClick.AddListener(OnBidAuctionButtonClick);
    }

	public void InitializeCardAuction(CardAuction cardAuction)
    {
        this.cardAuction = cardAuction;
		Debug.Log(cardAuction.Auction.StartingPrice);
		Debug.Log(cardAuction.Auction.EndingPrice);
		Debug.Log(cardAuction.Auction.Duration);
		Debug.Log(cardAuction.Auction.StartedAt);

        Texture2D texture = Resources.Load(cardAuction.Image) as Texture2D;
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
		GenericModalPanel.Instance.Show(
			"Confirm bid?",
			new UnityAction(ConfirmBidAuction),
			new UnityAction(CancelBidAuction)
		);
    }

    private void ConfirmBidAuction()
	{
        CryptoSingleton.Instance.BidAuction(
            Int32.Parse(cardAuction.Id.Substring(1)),
            1000L
        );
	}

	private void CancelBidAuction()
    {
    }
}
