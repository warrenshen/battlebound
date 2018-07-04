using System;
using UnityEngine;
using UnityEngine.UI;

public class SellableCardListItem : MonoBehaviour
{
    [SerializeField]
    private Image cardImage;
    [SerializeField]
    private Button createAuctionButton;

    private CardRaw card;

    public void Awake()
    {
        this.createAuctionButton.onClick.AddListener(OnCreateAuctionButtonClick);
    }

    public void InitializeCard(CardRaw card)
    {
        this.card = card;

        //Texture2D texture = Resources.Load("HS/" + card.Image) as Texture2D;
        Texture2D texture = Resources.Load("HS/Armorsmith") as Texture2D;
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
