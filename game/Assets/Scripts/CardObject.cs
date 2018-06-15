using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardObject : MonoBehaviour
{
    private string json;
    public Card card;

    private SpriteRenderer spr;
    public SpriteRenderer Renderer => spr;
    private Collider coll;
    private Texture2D image;

    private ActionManager action;
    private float lastClicked;
    public bool minified;

    public struct Reset {
        public Vector3 resetPosition;
        public Vector3 resetScale;
        public Quaternion resetRotation;
    }
    public Reset reset;

    public void Awake()
    {
        action = Camera.main.GetComponent<ActionManager>();
        spr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    //if pass-in json overload method
    public void InitializeCard(string json)
    {
        Card parsed = JsonUtility.FromJson<Card>(json);
        InitializeCard(parsed);
    }

    public void InitializeCard(Card card)
    {
        this.card = card;
        card.wrapper = this;
        //make render changes according to card class here
        image = Resources.Load(card.Image) as Texture2D;
        spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        spr.sortingOrder = 10;
        coll = gameObject.AddComponent<BoxCollider>() as Collider;
        coll.GetComponent<BoxCollider>().size = new Vector3(2.5f, 3.5f, 0.2f);
    }

    public void OnMouseEnter()
    {
        if (action.HasDragTarget())
            return;

        float scaling = 1.1f;
        transform.localScale = new Vector3(scaling, scaling, scaling);
    }

    public void OnMouseExit()
    {
        if (action.HasDragTarget())
            return;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(1)) Debug.Log("Pressed right click.");
    }

    public void OnMouseDown()
    {
        reset.resetPosition = transform.localPosition;
        reset.resetScale = transform.localScale;
        reset.resetRotation = transform.localRotation;

        action.SetDragTarget(this, Renderer);
    }

    public void OnMouseUp()
    {
        if (!action.HasDragTarget())
            return;
        ////resets card object position to original, handled by actionmanager
        if (Time.time - lastClicked < 0.5f)
            DoubleClickUp();
        lastClicked = Time.time;
    }

    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
        if(Application.loadedLevelName == "Collection")
            action.AddCardToDeck(this);
    }

    public void Minify(bool val)
    {
        if(val) {
            spr.sprite = Sprite.Create(image, new Rect(0.0f, image.height / 2 - 40, image.width, 40), new Vector2(0.5f, 0.5f), 100.0f);
            minified = true;
        }
        else {
            spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
            minified = false;
        }

    }
}