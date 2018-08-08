using System;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

public class SellableCardListItem : CardListItem
{
    [SerializeField]
    private Button createAuctionButton;

    public void OnEnable()
    {
        this.createAuctionButton.onClick.AddListener(OnCreateAuctionButtonClick);
    }

    public void OnDisable()
    {
        this.createAuctionButton.onClick.RemoveAllListeners();
    }

    public void Initialize(Card card)
    {
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
            new UnityAction(CancelAction),
            contentTypeOne: InputField.ContentType.IntegerNumber,
            contentTypeTwo: InputField.ContentType.IntegerNumber,
            contentTypeThree: InputField.ContentType.IntegerNumber
        );
    }

    private void SubmitCreateAuctionTransaction(UMP_ThreeInputDialogUI dialog, string startPrice, string endPrice, string auctionDuration)
    {
        int tokenId = Convert.ToInt32(card.Id.Substring(1));
        long startingPrice = Convert.ToInt64(startPrice);
        long endingPrice = Convert.ToInt64(endPrice);
        int duration = Convert.ToInt32(auctionDuration);

        //CryptoSingleton.Instance.CreateAuction(
        //    account,
        //    tokenId,
        //    startingPrice,
        //    endingPrice,
        //    duration
        //);

        dialog.Close();
    }

    private void CancelAction()
    {

    }
}
