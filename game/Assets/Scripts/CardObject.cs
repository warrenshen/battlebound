using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CardObject : MouseWatchable
{
    private string json;

    protected Player owner;
    public Player Owner => owner;

    [SerializeField]
    private Card card;
    public Card Card => card;

    private Collider collider;
    private Texture2D image;

    private float lastClicked;
    public bool minified;

    public HyperCard.Card visual;

    public struct Reset
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }
    public Reset reset;


    public void InitializeCard(Player player, Card card)
    {
        this.owner = player;
        this.card = card;
        this.gameObject.layer = LayerMask.NameToLayer("Card");
        card.wrapper = this;
        //make render changes according to card class here
        string i;
        if (card.Id == "HIDDEN")
        {
            Debug.Log("Card is hidden - setting to Direhorn Hatchling.");
            i = "Direhorn_Hatchling";
        }
        else
        {
            i = card.Image;
        }

        this.visual = VisualizeCard();
        RandomCardArtwork();
        //set sprite etc here
        //image = Resources.Load(i) as Texture2D;
        //spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        //spr.sortingOrder = 10;
        collider = gameObject.AddComponent<BoxCollider>() as Collider;
        collider.GetComponent<BoxCollider>().size = new Vector3(2.3f, 3.5f, 0.2f);
    }

    private void RandomCardArtwork()
    {

        Texture2D fore = Resources.Load(string.Format("Foregrounds/character_{0}", Random.Range(1, 61))) as Texture2D;
        Texture2D back = Resources.Load(string.Format("Backgrounds/background_{0}", Random.Range(0, 77))) as Texture2D;
        this.visual.SetCardArtwork(fore, back);
        this.visual.Stencil = BattleManager.Instance.stencilCount;
        BattleManager.Instance.stencilCount += 1 % 255;
    }

    private HyperCard.Card VisualizeCard()
    {
        GameObject visualPrefab = Resources.Load("Prefabs/Card") as GameObject;
        Transform created = Instantiate(visualPrefab, this.transform).transform as Transform;
        created.localPosition = Vector3.zero;
        created.localRotation = Quaternion.identity;
        created.Rotate(0, 180, 0, Space.Self);

        HyperCard.Card visual = created.GetComponent<HyperCard.Card>();
        //set sprites and set textmeshpro labels using TmpTextObjects (?)
        visual.TmpTextObjects[0].Value = this.Card.Name;
        visual.TmpTextObjects[2].Value = this.Card.Cost.ToString();

        CreatureCard creatureCard = this.Card as CreatureCard;
        if (creatureCard != null)
        {
            visual.TmpTextObjects[3].Value = creatureCard.Attack.ToString();
            visual.TmpTextObjects[4].Value = creatureCard.Health.ToString();
        }
        else
        {
            visual.TmpTextObjects[3].TmpObject.enabled = false;
            visual.TmpTextObjects[4].TmpObject.enabled = false;
        }
        return visual;
    }

    private void SetVisualResetValues()
    {
        this.visual.reset.position = this.visual.transform.localPosition;
        this.visual.reset.scale = this.visual.transform.localScale;
        this.visual.reset.rotation = this.visual.transform.localRotation;
    }

    private void SetThisResetValues()
    {
        this.reset.position = this.transform.localPosition;
        this.reset.scale = this.transform.localScale;
        this.reset.rotation = this.transform.localRotation;
    }

    public override void EnterHover()
    {
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of hypercard
        SetVisualResetValues();

        //Quaternion quart = Quaternion.identity;
        //quart.eulerAngles = new Vector3(0, 180, 0);
        //this.visual.transform.localRotation = quart;

        float scaling = 1.8f;
        this.visual.transform.localScale = scaling * this.visual.reset.scale;

        this.visual.transform.Translate(Vector3.up * 3.5f, Space.Self);
        this.visual.transform.Translate(Vector3.forward * 1, Space.Self);
    }

    public override void ExitHover()
    {
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;

        this.visual.transform.localScale = this.visual.reset.scale;
        this.visual.transform.localPosition = this.visual.reset.position;
    }

    //public void InFocus()
    //{
    //    if (!card.Owner.HasTurn)
    //        return;
    //    if (Input.GetMouseButtonUp(1)) Debug.Log("Pressed right click.");
    //}

    public override void MouseDown()
    {
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of cardobject
        SetThisResetValues();

        LeanTween.scale(this.visual.gameObject, this.visual.reset.scale, 0.1f);
        //this.visual.transform.localScale = this.visual.reset.scale;
        this.visual.transform.localPosition = this.visual.reset.position;

        ActionManager.Instance.SetDragTarget(this);
    }

    public override void MouseUp()
    {
        if (!this.owner.HasTurn)
            return;
        if (!ActionManager.Instance.HasDragTarget())
            return;
        //// card object position reset to original, handled in actionmanager already
        if (Time.time - lastClicked < 0.5f)
            DoubleClickUp();
        lastClicked = Time.time;
    }

    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
        if (Application.loadedLevelName == "Collection")
            ActionManager.Instance.AddCardToDeck(this);
    }

    public void Minify(bool value)
    {
        //if (value)
        //{
        //    spr.sprite = Sprite.Create(image, new Rect(0.0f, image.height / 2 - 40, image.width, 40), new Vector2(0.5f, 0.5f), 100.0f);
        //    minified = true;
        //}
        //else
        //{
        //    spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        //    minified = false;
        //}

    }
}
