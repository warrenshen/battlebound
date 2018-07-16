using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionCardObject : CardObject
{
    protected static Vector3 CUTOUT_BOUNDS = new Vector3(3.0F, 0.6F, 0.2F);
    protected static float FOCUS_OFFSET = 0.2F;

    [SerializeField]
    public bool minified;
    public HyperCard.Card cutout;

    public float lastClickedTime;
    public float lastDoubleClickedTime;

    public override void Initialize(Card card)
    {
        if (!CollectionManager.Instance.cardTemplates.ContainsKey(card.Name))
        {
            Debug.LogError(string.Format("Card with name {0} does not exist.", card.Name));
        }

        //does the visual stuff using templateData
        base.Initialize(card);
        //this is always at the end
        CreateCutout();
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

    private void CreateCutout()
    {
        GameObject instance = Instantiate(CollectionManager.Instance.cutoutPrefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = this.transform;
        instance.transform.localPosition = Vector3.zero;
        instance.transform.Rotate(0, 180, 0);

        cutout = instance.GetComponent<HyperCard.Card>();
        cutout.SetOutline(false);

        CardObject.SetHyperCardFromData(cutout, this.card);
        CardObject.SetHyperCardArtwork(cutout, this.card);
        cutout.gameObject.SetActive(false);
    }

    public override void EnterHover()
    {
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of hypercard

        float scaling = 1.1f;
        this.visual.transform.localScale = scaling * this.visual.reset.scale;
        this.visual.transform.Translate(Vector3.forward * CollectionCardObject.FOCUS_OFFSET, Space.Self);

        this.visual.SetOutline(true);
    }

    public override void ExitHover()
    {
        if (ActionManager.Instance.HasDragTarget())
            return;

        this.visual.transform.localScale = this.visual.reset.scale;
        this.visual.transform.localPosition = this.visual.reset.position;

        this.visual.SetOutline(false);
    }

    public override void MouseDown()
    {
        this.visual.transform.Translate(Vector3.forward * CollectionCardObject.FOCUS_OFFSET, Space.Self);
        this.gameObject.SetLayer(LayerMask.NameToLayer("UI"));
        ActionManager.Instance.SetDragTarget(this);
    }

    public override void MouseUp()
    {
        //nothing
    }

    public override void Release()
    {
        LeanTween.scale(this.visual.gameObject, this.visual.reset.scale, 0.1f);
        this.visual.transform.localPosition = this.visual.reset.position;

        ActionManager.Instance.ResetTarget().setOnComplete(() => this.gameObject.SetLayer(LayerMask.NameToLayer("Card")));
    }

    public void DoubleClickUp()
    {
        this.lastDoubleClickedTime = Time.time;

        if (!CollectionManager.Instance.ActiveDecklist.Contains(this))
        {
            LeanTween.cancel(this.gameObject);
            this.noInteraction = false;

            CollectionManager.Instance.AddToDecklist(this);  //to-do: buggy af if spam-clicked
        }
    }

    public void SetMinify(bool value)
    {
        //Create CardCutout
        this.minified = value;
        this.visual.gameObject.SetActive(!value);
        this.cutout.gameObject.SetActive(value);

        if (value)
        {
            this.colliderBox.size = CollectionCardObject.CUTOUT_BOUNDS;
            this.transform.parent = null;
        }
        else
        {
            this.colliderBox.size = CollectionCardObject.CARD_BOUNDS;
            this.transform.parent = CollectionManager.Instance.collectionObject.transform;
        }
    }
}
