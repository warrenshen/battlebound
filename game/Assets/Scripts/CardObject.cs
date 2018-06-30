using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CardObject : MonoBehaviour
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

    public struct Reset
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
    }
    public Reset reset;

    public HyperCard.Card visual;


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
        //set sprite etc here
        //image = Resources.Load(i) as Texture2D;
        //spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        //spr.sortingOrder = 10;
        collider = gameObject.AddComponent<BoxCollider>() as Collider;
        collider.GetComponent<BoxCollider>().size = new Vector3(2.3f, 3.5f, 0.2f);
    }

    private HyperCard.Card VisualizeCard()
    {
        GameObject visualPrefab = Resources.Load("Prefabs/Card_Default") as GameObject;
        Transform created = Instantiate(visualPrefab, this.transform).transform as Transform;
        created.localPosition = Vector3.zero;
        created.localRotation = Quaternion.identity;
        created.Rotate(0, 180, 0, Space.Self);
        return created.GetComponent<HyperCard.Card>();

        //set sprites and set textmeshpro labels using TmpTextObjects (?)
    }

    public void EnterFocus()
    {
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;

        float scaling = 1.2f;
        LeanTween.scale(gameObject, new Vector3(scaling, scaling, scaling), 0.05f);
        //transform.localScale = new Vector3(scaling, scaling, scaling);
    }

    public void ExitFocus()
    {
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        LeanTween.scale(gameObject, Vector3.one, 0.05f);
        //transform.localScale = new Vector3(1, 1, 1);
    }

    //public void InFocus()
    //{
    //    if (!card.Owner.HasTurn)
    //        return;
    //    if (Input.GetMouseButtonUp(1)) Debug.Log("Pressed right click.");
    //}

    public void MouseDown()
    {
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults
        reset.position = transform.localPosition;
        reset.scale = transform.localScale;
        reset.rotation = transform.localRotation;

        ActionManager.Instance.SetDragTarget(this);
    }

    public void MouseUp()
    {
        if (!this.owner.HasTurn)
            return;
        if (!ActionManager.Instance.HasDragTarget())
            return;
        ////resets card object position to original, handled by actionmanager
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
