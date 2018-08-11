using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class CollectionCardObject : CardObject
{
    protected static Vector3 CUTOUT_BOUNDS = new Vector3(3.0F, 0.6F, 0.2F);
    protected static float FOCUS_OFFSET = 0.2F;
    protected static float FOCUS_GROW = 1.1F;

    private bool grabbed = false;

    public new static CollectionCardObject Create(Card card)
    {
        GameObject created = new GameObject(card.Name);
        CollectionCardObject cardObject = created.AddComponent<CollectionCardObject>();
        cardObject.Initialize(card);
        return cardObject;
    }

    public override void Initialize(Card card)
    {
        //does the visual stuff using templateData
        base.Initialize(card);

        this.visual.BurnColor = Color.cyan;
        this.visual.BurnEndColor = Color.white;
    }

    public void InitializeHollow(Card card) //no cutout, no wrapper assignment, no collider
    {
        this.card = card;

        //make render changes according to card class here
        this.visual = this.VisualizeCard();
        this.LoadCardArtwork();
        this.SetThisResetValues();
        this.SetVisualResetValues();
    }

    public override void EnterHover()
    {
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of hypercard
        this.visual.transform.Translate(Vector3.back * CollectionCardObject.FOCUS_OFFSET, Space.Self);
        this.visual.transform.localScale *= FOCUS_GROW;
        this.visual.SetOutline(true);
    }

    public override void ExitHover()
    {
        if (ActionManager.Instance.HasDragTarget())
            return;

        if (!this.grabbed)
        {
            this.visual.transform.localPosition = this.visual.reset.position;
            this.visual.transform.localScale = this.visual.reset.scale;
            this.visual.SetOutline(false);
        }
    }

    public override void MouseDown()
    {
        this.visual.transform.Translate(Vector3.back * CollectionCardObject.FOCUS_OFFSET, Space.Self);
        this.visual.SetOutlineColors(Color.white, this.visual.InitialOutlineEndColor);
        ActionManager.Instance.SetDragTarget(this);
        this.grabbed = true;
    }

    public override void MouseUp()
    {
        ActionManager.Instance.ResetTarget(this).setOnComplete(() =>
        {
            this.visual.SetOutlineColors(this.visual.InitialOutlineStartColor, this.visual.InitialOutlineEndColor);
            this.visual.transform.localPosition = this.visual.reset.position;
            this.visual.transform.localScale = this.visual.reset.scale;
            this.visual.SetOutline(false);
            this.grabbed = false;
        });
    }

    public void ResetTransform()
    {
        this.transform.localPosition = this.reset.position;
        this.transform.localScale = this.reset.scale;
        this.transform.localRotation = this.reset.rotation;
    }

    public void SetBothResetValues()
    {
        this.SetThisResetValues();
        this.SetVisualResetValues();
    }

    public Reset GetThisResetValues()
    {
        return this.reset;
    }
}
