using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionCardObject : CardObject
{
    [SerializeField]
    public bool minified;


    public override void Initialize(Card card)
    {
        if (!CollectionManager.Instance.cardTemplates.ContainsKey(card.Name))
        {
            Debug.LogError(string.Format("Card with name {0} does not exist.", card.Name));
        }

        this.templateData = CollectionManager.Instance.cardTemplates[card.Name];
        //does the visual stuff using templateData
        base.Initialize(card);
    }

    public override void EnterHover()
    {
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of hypercard
        this.SetVisualResetValues();

        float scaling = 1.1f;
        this.visual.transform.localScale = scaling * this.visual.reset.scale;
        this.visual.transform.Translate(Vector3.forward * 0.33f, Space.Self);

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
        this.visual.transform.Translate(Vector3.forward * 0.33f, Space.Self);
        this.gameObject.SetLayer(LayerMask.NameToLayer("UI"));
        ActionManager.Instance.SetDragTarget(this);
    }

    public override void Release()
    {
        if (Time.time - lastClicked < 0.5f)
            DoubleClickUp();
        lastClicked = Time.time;

        LeanTween.scale(this.visual.gameObject, this.visual.reset.scale, 0.1f);
        this.visual.transform.localPosition = this.visual.reset.position;

        ActionManager.Instance.ResetTarget().setOnComplete(() => this.gameObject.SetLayer(LayerMask.NameToLayer("Card")));
    }

    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
    }

    public void Minify(bool value)
    {
        //Create CardCutout
        this.minified = true;
        this.visual.gameObject.SetActive(false);
        GameObject instance = Instantiate(CollectionManager.Instance.cutoutPrefab, Vector3.zero, Quaternion.identity);
        instance.transform.parent = this.transform;
        instance.transform.localPosition = Vector3.zero;

        CardCutout cutout = instance.AddComponent<CardCutout>();
        cutout.Initialize(this); //, chosenDeck.Cards);
    }
}
