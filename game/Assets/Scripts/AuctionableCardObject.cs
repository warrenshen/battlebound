using System;
using UnityEngine;

public class AuctionableCardObject : MonoBehaviour
{
	public Card card;

    private SpriteRenderer spr;
    public SpriteRenderer Renderer => spr;

    private Collider coll;
    private Texture2D image;

    private ActionManager action;
    private float lastClicked;

    public void Awake()
    {
		CreateAuctionModalPanel.Instance().HideModal();
        action = Camera.main.GetComponent<ActionManager>();
        spr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
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
        float scaling = 1.1f;
        transform.localScale = new Vector3(scaling, scaling, scaling);
    }

	public void OnMouseExit()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnMouseUp()
    {
        if (Time.time - lastClicked < 0.5f)
        {
            DoubleClickUp();
        }
        lastClicked = Time.time;
    }
    
    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
		CreateAuctionModalPanel.Instance().ShowModalForCard(this.card);
    }
}
