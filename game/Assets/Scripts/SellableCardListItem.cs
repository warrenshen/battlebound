﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellableCardListItem : CardListItem
{
    [SerializeField]
    private Button createAuctionButton;

    public new void Awake()
    {
        base.Awake();
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
