using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCutout : MonoBehaviour {
    Collection collection;
    CardObject wrapper;

	// Use this for initialization
    public void Initialize(CardObject wrapper, List<Card> deck, Collection collection) {
        this.collection = collection;
        this.wrapper = wrapper;

        SpriteRenderer sp = gameObject.AddComponent<SpriteRenderer>();
        sp.sprite = wrapper.Renderer.sprite;

        //reposition and rotate
        gameObject.transform.parent = GameObject.Find("Panel").transform;
        PositionCutout(deck.Count);
        //create collider
        BoxCollider coll = gameObject.AddComponent<BoxCollider>() as BoxCollider;
    }

    public void PositionCutout(int index) {
        gameObject.transform.localPosition = new Vector3(-0.25f + index * 0.35f, Random.Range(-0.05f, 0.05f), 0f);
        gameObject.transform.localRotation = Quaternion.identity;
        gameObject.transform.Rotate(new Vector3(0f, 0f, 90f), Space.Self);
    }

    public void OnMouseDown()
    {
        collection.RemoveFromDeck(wrapper, this);
        Destroy(gameObject);
    }
}
