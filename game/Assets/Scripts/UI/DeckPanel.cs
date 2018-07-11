using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckPanel : BasicButton
{
    private Deck deck;

    // Use this for initialization
    void Start()
    {
        base.Initialize();
    }

    public void Initialize(Deck deck)
    {
        this.deck = deck;
        this.transform.Find("Deck Info").GetComponent<TextMeshPro>().text = deck.ToString();
    }

    public override void MouseUp()
    {
        CollectionManager.Instance.RotateToDeck(deck);
    }

}
