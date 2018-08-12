using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardCutout : MonoBehaviour
{
    [SerializeField]
    private Card card;

    [SerializeField]
    private Button button;

    private CardObject.Reset reset;

    [SerializeField]
    private Image front;

    [SerializeField]
    private Image back;

    [SerializeField]
    private Text nameLabel;

    [SerializeField]
    private Text costLabel;

    public void Render(CollectionCardObject cardObject)
    {
        this.card = cardObject.Card;
        this.reset = cardObject.GetThisResetValues();

        this.front.sprite = ResourceSingleton.Instance.GetSpriteByName(this.card.GetFrontImage());
        this.back.sprite = ResourceSingleton.Instance.GetSpriteByName(this.card.GetBackImage());

        this.nameLabel.text = GetName();
        this.costLabel.text = GetCost().ToString();
    }

    public Card GetCard()
    {
        return this.card;
    }

    public string GetId()
    {
        return this.card.Id;
    }

    public int GetLevel()
    {
        return this.card.Level;
    }

    public string GetName()
    {
        return this.card.GetName();
    }

    public int GetCost()
    {
        return this.card.GetCost();
    }

    public CardObject.Reset GetReset()
    {
        return this.reset;
    }

    public void SetClickListener(UnityAction listener)
    {
        this.button.onClick.RemoveAllListeners();
        this.button.onClick.AddListener(listener);
    }
}
