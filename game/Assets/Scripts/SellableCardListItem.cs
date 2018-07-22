using System;
using UnityEngine;
using UnityEngine.UI;

public class SellableCardListItem : MonoBehaviour
{
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Button createAuctionButton;

    private Card card;

    public void Awake()
    {
        this.createAuctionButton.onClick.AddListener(OnCreateAuctionButtonClick);
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
        CreateAuctionModalPanel.Instance.ShowModalForCard(this.card);
    }
}
