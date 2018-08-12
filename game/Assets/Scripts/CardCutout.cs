using System;
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

        this.front.sprite = ResourceSingleton.Instance.GetSpriteByName(this.card.GetFrontImage());
        this.back.sprite = ResourceSingleton.Instance.GetSpriteByName(this.card.GetBackImage());

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
