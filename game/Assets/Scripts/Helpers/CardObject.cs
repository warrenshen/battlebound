using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CardObject : MouseWatchable
{
    [SerializeField]
    protected Card card;
    public Card Card => card;

    protected Collider colliderBox;
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
        this.visual = this.VisualizeCard();
        this.LoadCardArtwork();
        this.SetThisResetValues();
        this.SetVisualResetValues();

        //set sprite etc here
        colliderBox = gameObject.AddComponent<BoxCollider>() as Collider;
        colliderBox.GetComponent<BoxCollider>().size = new Vector3(2.3f, 3.5f, 0.2f);
    }

    protected virtual void LoadCardArtwork()
    {
        this.frontImage = Resources.Load(this.templateData.frontImage) as Texture2D;
        this.backImage = Resources.Load(this.templateData.backImage) as Texture2D;

        this.visual.SetFrontTiling(this.templateData.frontScale, this.templateData.frontOffset);
        this.visual.SetBackTiling(this.templateData.backScale, this.templateData.backOffset);
        this.visual.SetCardArtwork(this.frontImage, this.backImage);

        this.visual.Stencil = ActionManager.Instance.stencilCount;
        ActionManager.Instance.stencilCount += 3 % 255;
    }

    private HyperCard.Card VisualizeCard()
    {
        GameObject visualPrefab = Resources.Load("Prefabs/Card") as GameObject;
        Transform created = Instantiate(visualPrefab, this.transform).transform as Transform;
        created.localPosition = Vector3.zero;
        created.localRotation = Quaternion.identity;
        created.Rotate(0, 180, 0, Space.Self);

        HyperCard.Card cardVisual = created.GetComponent<HyperCard.Card>();
        //set sprites and set textmeshpro labels using TmpTextObjects (?)
        cardVisual.TmpTextObjects[0].Value = this.card.Name;
        cardVisual.TmpTextObjects[1].Value = this.templateData.description;
        cardVisual.TmpTextObjects[2].Value = this.card.Cost.ToString();

        CreatureCard creatureCard = this.card as CreatureCard;
        if (creatureCard != null)
        {
            cardVisual.TmpTextObjects[3].Value = creatureCard.Attack.ToString();
            cardVisual.TmpTextObjects[4].Value = creatureCard.Health.ToString();
        }
        else
        {
            cardVisual.TmpTextObjects[3].TmpObject.enabled = false;
            cardVisual.TmpTextObjects[4].TmpObject.enabled = false;
        }
        cardVisual.Redraw();
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
