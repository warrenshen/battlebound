using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CardObject : MouseWatchable
{
    protected static Vector3 CARD_BOUNDS = new Vector3(2.3F, 3.5F, 0.2F);

    [SerializeField]
    protected Card card;
    public Card Card => card;

    protected BoxCollider colliderBox;
    protected float lastClicked;

    public HyperCard.Card visual;
    protected CardTemplate templateData;
    public CardTemplate TemplateData => templateData;

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

        if (this.templateData == null)
        {
            Debug.LogError("WTF, templateData doesn't exist, OOP??");
        }

        //make render changes according to card class here
        this.LoadCardArtwork();
        this.visual = this.VisualizeCard();

        this.SetThisResetValues();
        this.SetVisualResetValues();

        //set sprite etc here
        colliderBox = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        colliderBox.size = CardObject.CARD_BOUNDS;
    }

    protected virtual void LoadCardArtwork()
    {
        this.frontImage = Resources.Load(this.templateData.frontImage) as Texture2D;
        this.backImage = Resources.Load(this.templateData.backImage) as Texture2D;
    }

    protected static void SetHyperCardArtwork(HyperCard.Card cardVisual, CardTemplate cardTemplate, Texture2D frontImage = null, Texture2D backImage = null)
    {
        if (frontImage == null)
        {
            frontImage = Resources.Load(cardTemplate.frontImage) as Texture2D;
        }
        if (backImage == null)
        {
            backImage = Resources.Load(cardTemplate.backImage) as Texture2D;
        }

        cardVisual.SetFrontTiling(cardTemplate.frontScale, cardTemplate.frontOffset);
        cardVisual.SetBackTiling(cardTemplate.backScale, cardTemplate.backOffset);
        cardVisual.SetCardArtwork(frontImage, backImage);

        cardVisual.Stencil = ActionManager.Instance.stencilCount;
        ActionManager.Instance.stencilCount += 3 % 255;
    }

    protected static void SetHyperCardFromData(HyperCard.Card cardVisual, CardTemplate cardTemplate)
    {
        //set sprites and set textmeshpro labels using TmpTextObjects (?)
        cardVisual.SetTextFieldWithKey("Title", cardTemplate.name);
        cardVisual.SetTextFieldWithKey("Description", cardTemplate.description);
        cardVisual.SetTextFieldWithKey("Cost", cardTemplate.cost.ToString());

        bool isCreature = cardTemplate.cardType == CardRaw.CardType.Creature;
        if (isCreature)
        {
            cardVisual.SetTextFieldWithKey("Attack", cardTemplate.attack.ToString());
            cardVisual.SetTextFieldWithKey("Health", cardTemplate.health.ToString());
        }
        else
        {
            cardVisual.GetTextFieldWithKey("Attack").TmpObject.enabled = false;
            cardVisual.GetTextFieldWithKey("Health").TmpObject.enabled = false;
        }
        cardVisual.Redraw();
    }

    protected HyperCard.Card VisualizeCard()
    {
        GameObject visualPrefab = Resources.Load("Prefabs/Card") as GameObject;
        Transform created = Instantiate(visualPrefab, this.transform).transform as Transform;
        created.localPosition = Vector3.zero;
        created.localRotation = Quaternion.identity;
        created.Rotate(0, 180, 0, Space.Self);

        HyperCard.Card cardVisual = created.GetComponent<HyperCard.Card>();
        CardObject.SetHyperCardFromData(cardVisual, this.templateData);
        CardObject.SetHyperCardArtwork(cardVisual, this.templateData, this.frontImage, this.backImage);

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
}
