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

	private float lastClicked;
	public bool minified;

	public struct Reset
	{
		public Vector3 resetPosition;
		public Vector3 resetScale;
		public Quaternion resetRotation;
	}
	public Reset reset;

	public void Awake()
	{
		spr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
	}

	////if pass-in json overload method
	//public void InitializeCard(string json)
	//{
	//	Card parsed = JsonUtility.FromJson<Card>(json);
	//	InitializeCard(parsed);
	//}

	public void InitializeCard(Card card)
	{
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
		image = Resources.Load(i) as Texture2D;
		spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
		spr.sortingOrder = 10;
		coll = gameObject.AddComponent<BoxCollider>() as Collider;
		coll.GetComponent<BoxCollider>().size = new Vector3(2.3f, 3.2f, 0.1f);
	}

	public void EnterFocus()
	{
		if (!card.Owner.HasTurn)
			return;
		if (ActionManager.Instance.HasDragTarget())
			return;

		float scaling = 1.1f;
		transform.localScale = new Vector3(scaling, scaling, scaling);
	}

	public void ExitFocus()
	{
		if (!card.Owner.HasTurn)
			return;
		if (ActionManager.Instance.HasDragTarget())
			return;
		transform.localScale = new Vector3(1, 1, 1);
	}

	//public void InFocus()
	//{
	//    if (!card.Owner.HasTurn)
	//        return;
	//    if (Input.GetMouseButtonUp(1)) Debug.Log("Pressed right click.");
	//}

	public void MouseDown()
	{
		if (!card.Owner.HasTurn)
			return;
		if (ActionManager.Instance.HasDragTarget())
			return;
		//set defaults
		reset.resetPosition = transform.localPosition;
		reset.resetScale = transform.localScale;
		reset.resetRotation = transform.localRotation;

		ActionManager.Instance.SetDragTarget(this, Renderer);
	}

	public void MouseUp()
	{
		if (!card.Owner.HasTurn)
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
		if (value)
		{
			spr.sprite = Sprite.Create(image, new Rect(0.0f, image.height / 2 - 40, image.width, 40), new Vector2(0.5f, 0.5f), 100.0f);
			minified = true;
		}
		else
		{
			spr.sprite = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
			minified = false;
		}

	}
}
