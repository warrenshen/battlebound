using UnityEngine;

[System.Serializable]
public abstract class CardObject : MouseWatchable
{
    protected static Vector3 CARD_BOUNDS = new Vector3(2.3F, 3.5F, 0.2F);

    [SerializeField]
    protected Card card;
    public Card Card => card;

    protected BoxCollider colliderBox;

    public HyperCard.Card visual;

    public Texture2D frontImage;
    public Texture2D backImage;

    public struct Reset
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }

    [SerializeField]
    public Reset reset;

    public virtual void Initialize(Card card)
    {
        this.card = card;
        this.gameObject.layer = LayerMask.NameToLayer("Card");
        card.wrapper = this;

        //make render changes according to card class here
        this.LoadCardArtwork();
        this.visual = this.VisualizeCard();

        this.SetThisResetValues();
        this.SetVisualResetValues();

        //set sprite etc here
        colliderBox = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        colliderBox.size = CardObject.CARD_BOUNDS;
    }

    public void Reinitialize(Card card)
    {
        this.card = card;
        card.wrapper = this;
        this.LoadCardArtwork();

        if (this.visual == null)
        {
            Debug.LogError("Reinitialize called on card object without card visual.");
            return;
        }

        CardObject.SetHyperCardFromData(this.visual, this.card);
        CardObject.SetHyperCardArtwork(this.visual, this.card);
    }

    protected virtual void LoadCardArtwork()
    {
        this.frontImage = this.card.GetFrontImageTexture();
        this.backImage = this.card.GetBackImageTexture();
    }

    protected HyperCard.Card VisualizeCard()
    {
        GameObject cardPrefab = ResourceSingleton.Instance.GetCardPrefab();

        Transform created = Instantiate(cardPrefab, this.transform).transform as Transform;
        created.localPosition = Vector3.zero;
        created.localRotation = Quaternion.identity;
        created.Rotate(0, 180, 0, Space.Self);

        HyperCard.Card cardVisual = created.GetComponent<HyperCard.Card>();
        CardObject.SetHyperCardFromData(cardVisual, this.card);
        CardObject.SetHyperCardArtwork(cardVisual, this.card);

        return cardVisual;
    }

    protected void SetVisualResetValues()
    {
        this.visual.reset.position = this.visual.transform.localPosition;
        this.visual.reset.scale = this.visual.transform.localScale;
        this.visual.reset.rotation = this.visual.transform.localRotation;
    }

    protected void SetThisResetValues()
    {
        this.reset.position = this.transform.localPosition;
        this.reset.scale = this.transform.localScale;
        this.reset.rotation = this.transform.localRotation;
    }

    public virtual void Release()
    {

    }

    protected static void SetHyperCardArtwork(HyperCard.Card cardVisual, Card card)
    {
        cardVisual.SetFrontTiling(card.GetFrontScale(), card.GetFrontOffset());
        cardVisual.SetBackTiling(card.GetBackScale(), card.GetBackOffset());
        cardVisual.SetCardArtwork(
            card.GetFrontImageTexture(),
            card.GetBackImageTexture()
        );

        cardVisual.Stencil = ActionManager.Instance.stencilCount;
        ActionManager.Instance.stencilCount = Mathf.Max(ActionManager.Instance.stencilCount + 3 % 255, 2);
    }

    protected static void SetHyperCardFromData(HyperCard.Card cardVisual, Card card)
    {
        //set sprites and set textmeshpro labels using TmpTextObjects (?)
        cardVisual.SetTextFieldWithKey("Title", card.GetName());
        cardVisual.SetTextFieldWithKey("Description", card.GetDescription());
        cardVisual.SetTextFieldWithKey("Cost", card.GetCost().ToString());

        switch (card.GetRarity())
        {
            case Card.RarityType.Common:
                cardVisual.SetTextFieldWithKey("Rarity", "N");
                break;
            case Card.RarityType.Uncommon:
                cardVisual.SetTextFieldWithKey("Rarity", "UN");
                break;
            case Card.RarityType.Rare:
                cardVisual.SetTextFieldWithKey("Rarity", "R");
                break;
            case Card.RarityType.Epic:
                cardVisual.SetTextFieldWithKey("Rarity", "EP");
                break;
            case Card.RarityType.Legendary:
                cardVisual.SetTextFieldWithKey("Rarity", "LG");
                break;
            case Card.RarityType.Cosmic:
                cardVisual.SetTextFieldWithKey("Rarity", "CL");
                break;
        }

        bool isCreature = card.GetType() == typeof(CreatureCard);
        if (isCreature)
        {
            CreatureCard creatureCard = card as CreatureCard;
            cardVisual.SetTextFieldWithKey("Attack", creatureCard.GetAttack().ToString());
            cardVisual.SetTextFieldWithKey("Health", creatureCard.GetHealth().ToString());
        }
        else
        {
            cardVisual.GetTextFieldWithKey("Attack").TmpObject.enabled = false;
            cardVisual.GetTextFieldWithKey("Health").TmpObject.enabled = false;
        }
        cardVisual.Redraw();
    }
}
