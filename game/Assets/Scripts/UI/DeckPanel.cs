using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckPanel : BasicButton
{
    private DeckRaw deckRaw;

    // Use this for initialization
    void Start()
    {
        base.Initialize();
    }

    public void Initialize(DeckRaw deckRaw)
    {
        this.deckRaw = deckRaw;
        this.transform.Find("Deck Info").GetComponent<TextMeshPro>().text = deckRaw.ToString();
    }

    public override void MouseUp()
    {
        CollectionManager.Instance.RotateToDeck(this.deckRaw);
    }

}
