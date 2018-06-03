using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    private string json;
    public Card card;

    private SpriteRenderer spr;
    public SpriteRenderer Renderer => spr;
    private Collider coll;
    private Texture2D image;

    private Vector3 resetPosition;
    private Vector3 resetScale;
    private Quaternion resetRotation;

    private ActionManager action;
    private float lastClicked;
    public bool minified;

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
        resetPosition = transform.localPosition;
        resetScale = transform.localScale;
        resetRotation = transform.localRotation;

        action.SetDragTarget(transform, Renderer);
    }

    public void OnMouseUp()
    {
        action.ClearDragTarget();

        transform.localPosition = resetPosition;
        transform.localScale = resetScale;
        transform.localRotation = resetRotation;

        if (Time.time - lastClicked < 0.5f)
            DoubleClickUp();
        lastClicked = Time.time;
    }

    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
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