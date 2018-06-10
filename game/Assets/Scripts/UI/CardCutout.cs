using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCutout : ObjectUI {
    Collection collection;
    CardObject wrapper;

	// Use this for initialization
    public void Initialize(CardObject wrapper, List<Card> deck, Collection collection) {
        this.collection = collection;
        this.wrapper = wrapper;
        SpriteRenderer sp = gameObject.AddComponent<SpriteRenderer>();
        Texture2D texture = wrapper.Renderer.sprite.texture;
        sp.sprite = Sprite.Create(texture, new Rect(0.0f, texture.height / 2 - 40, texture.width, 40), new Vector2(0.5f, 0.5f), 100.0f);

        //reposition and rotate
        gameObject.transform.parent = GameObject.Find("Build Panel").transform;
        PositionCutout(deck.Count);
        //create collider
        BoxCollider coll = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        base.Initialize();
        scalingFactor = 1.06f;
    }

    public void PositionCutout(int index) {
        gameObject.transform.localPosition = new Vector3((index + 1) * 0.3f, Random.Range(-0.05f, 0.05f), 0f);
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f), Space.Self);
    }

    public void OnMouseDown()
    {
        collection.RemoveFromDeck(wrapper, this);
        Destroy(gameObject);
    }
}
