using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class CardObject : MouseWatchable
{
    protected static Vector3 CARD_BOUNDS = new Vector3(2.3F, 3.5F, 0.2F);

    [SerializeField]
    protected Card card;
    public Card Card => card;

    [SerializeField]
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

    public static CardObject Create(Card card)
    {
        GameObject created = new GameObject(card.Name);
        CardObject cardObject = created.AddComponent<CardObject>();
        BoxCollider boxCollider = created.AddComponent<BoxCollider>();
        cardObject.colliderBox = boxCollider;
        cardObject.Initialize(card);
        return cardObject;
    }

    public virtual void Initialize(Card card)
    {
        this.card = card;
        this.gameObject.layer = LayerMask.NameToLayer("Card");
        card.wrapper = this;

        //make render changes according to card class here
        LoadCardArtwork();
        if (this.visual == null)
        {
            this.visual = VisualizeCard();
        }
        else
        {
            this.visual.ResetParams();
            Card.SetHyperCardFromData(ref this.visual, this.card);
            Card.SetHyperCardArtwork(ref this.visual, this.card);
        }

        SetThisResetValues();
        SetVisualResetValues();

        //set sprite etc here
        this.colliderBox.size = CardObject.CARD_BOUNDS;
    }

    protected virtual void LoadCardArtwork()
    {
        this.frontImage = this.card.GetFrontImageTexture();
        this.backImage = this.card.GetBackImageTexture();
    }

    protected HyperCard.Card VisualizeCard()
    {
        HyperCard.Card cardVisual = CardSingleton.Instance.TakeCardFromPool();

        cardVisual.transform.parent = this.transform;
        cardVisual.transform.localPosition = Vector3.zero;
        cardVisual.transform.localRotation = Quaternion.identity;
        cardVisual.transform.Rotate(0, 180, 0, Space.Self);

        Card.SetHyperCardFromData(ref cardVisual, this.card);
        Card.SetHyperCardArtwork(ref cardVisual, this.card);

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

    public void Recycle()
    {
        if (this.visual != null)
        {
            CardSingleton.Instance.ReturnCardToPool(this.visual);
        }
        Destroy(gameObject);
    }

    public void Burn(UnityAction onBurnFinish, float duration = 1F)
    {
        StartCoroutine("Dissolve", new object[2] { duration, onBurnFinish });  //called when overdraw
        //SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);
    }

    protected virtual IEnumerator Dissolve(object[] args)
    {
        float duration = (float)args[0];
        UnityAction onBurnFinish = args[1] as UnityAction;

        float elapsedTime = 0;
        bool textHidden = false;
        while (elapsedTime < duration)
        {
            if (LeanTween.isTweening(this.gameObject))
                LeanTween.cancel(this.gameObject);

            if (elapsedTime > duration / 2.5f && !textHidden)
            {
                this.visual.EmptyAllText();
                textHidden = true;
            }
            this.visual.BurningAmount = Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Recycle();
        onBurnFinish.Invoke();
    }
}
