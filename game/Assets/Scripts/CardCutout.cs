using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCutout : MonoBehaviour
{
    [SerializeField]
    private Card card;
    private CardObject.Reset reset;

    [SerializeField]
    private Image front;

    [SerializeField]
    private Image back;

    [SerializeField]
    private Text nameLabel;

    private int cost;
    public int Cost => cost;
    [SerializeField]
    private Text costLabel;

    public void Render(CollectionCardObject cardObject)
    {
        this.card = cardObject.Card;
        this.reset = cardObject.GetThisResetValues();

        Texture2D texture = ResourceSingleton.Instance.GetImageTextureByName(cardObject.Card.GetFrontImage());
        this.front.sprite = Sprite.Create(
            texture,
            new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100.0f
        );
        texture = ResourceSingleton.Instance.GetImageTextureByName(cardObject.Card.GetBackImage());
        if (texture != null)
        {
            this.back.sprite = Sprite.Create(
                texture,
                new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100.0f
            );
        }
        else
        {
            Debug.LogError(String.Format("Could not find ImageTextureByName for image name {0}", cardObject.Card.GetBackImage()));
        }

        this.name = cardObject.Card.GetName();
        this.cost = cardObject.Card.GetCost();
        this.nameLabel.text = this.name;
        this.costLabel.text = this.cost.ToString();
    }

    public Card GetCard()
    {
        return this.card;
    }

    public CardObject.Reset GetReset()
    {
        return this.reset;
    }
}
