using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckPanel : ObjectUI {
    private Deck deck;
    private Collection collection;

	// Use this for initialization
	void Start () {
        base.Initialize();
	}

    public void Initialize(Deck deck, Collection collection) {
        this.deck = deck;
        this.collection = collection;
    }

    private void OnMouseUp()
    {
        collection.RotateToDeck(deck);
    }

}
