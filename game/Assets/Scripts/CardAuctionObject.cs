using System;
using UnityEngine;
using TMPro;

public class CardAuctionObject : MonoBehaviour
{
	public CardAuction cardAuction;
    
	private SpriteRenderer spr;
	public SpriteRenderer Renderer => spr;

	private Collider coll;
    private Texture2D image;
	private TextMeshPro textMesh;
	private GameObject go;

	private ActionManager action;
	private float lastClicked;

	public void Awake()
    {
		go = new GameObject();
		textMesh = go.AddComponent<TextMeshPro>();
		textMesh.fontSize = 4;
        
		RectTransform textContainer = textMesh.GetComponent<RectTransform>();
		textContainer.sizeDelta = new Vector2(4, 6);
		textContainer.anchoredPosition = new Vector3(0, -6, 0);

		go.transform.SetParent(gameObject.transform, false);

        action = Camera.main.GetComponent<ActionManager>();
        spr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

	public void InitializeCardAuction(CardAuction cardAuction)
    {
		this.cardAuction = cardAuction;
		cardAuction.wrapper = this;
        //make render changes according to card class here
        image = Resources.Load(cardAuction.Image) as Texture2D;
        spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
        spr.sortingOrder = 10;
        coll = gameObject.AddComponent<BoxCollider>() as Collider;
        coll.GetComponent<BoxCollider>().size = new Vector3(2.5f, 3.5f, 0.2f);

		textMesh.text = "Price: " +
			cardAuction.Auction.StartingPrice +
			"\nSeller: " +
			cardAuction.Auction.Seller;
    }

	public void OnMouseEnter()
	{
        float scaling = 1.1f;
        transform.localScale = new Vector3(scaling, scaling, scaling);
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
		CryptoSingleton.Instance.CreateAuction(
			Int32.Parse(cardAuction.Id.Substring(1)),
            100,
            100,
            3600
		);
    }
}
