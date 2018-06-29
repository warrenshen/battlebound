using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Hand
{
    [SerializeField]
    private List<Card> cards;

    private string name;

    public Hand(string name = "Player")
    {
        this.cards = new List<Card>();
        this.name = name;
    }

    public Hand(string name, List<Card> cards)
    {
        this.name = name;
        this.cards = cards;

        CreateCardObjects(this.cards);
    }

    public Card GetCardById(string cardId)
    {
        foreach (Card card in this.cards)
        {
            if (card.Id == cardId)
            {
                return card;
            }
        }
        return null;
    }


    public Card GetCardByIndex(int index)
    {
        return this.cards.ElementAt(index);
    }

    public int Size()
    {
        return cards.Count;
    }

    public void AddDrawnCard(Card drawnCard)
    {
        this.cards.Add(drawnCard);
        CreateCardObject(drawnCard);
    }

    public void Discard(int count)
    {

    }

    public void RemoveByCardId(string cardId)
    {
        int removeIndex = this.cards.FindIndex(card => card.Id == cardId);
        this.cards.RemoveAt(removeIndex);
        this.RepositionCards();
    }

    private void CreateCardObject(Card card, bool shouldReposition = true)
    {
        GameObject created = new GameObject(card.Name);
        CardObject wrapper = created.AddComponent<CardObject>();
        wrapper.InitializeCard(card);
        created.transform.parent = GameObject.Find(name + " Hand").transform;

        if (shouldReposition)
        {
            RepositionCards();
        }
    }

    private void CreateCardObjects(List<Card> toCreate)
    {
        foreach (Card card in toCreate)
        {
            CreateCardObject(card, false);
        }
        RepositionCards();
    }

    public void RepositionCards()
    {
        HashSet<string> cardIdSet = new HashSet<string>();
        foreach (Card card in this.cards)
        {
            if (cardIdSet.Contains(card.Id))
            {
                Debug.LogError("Duplicate card IDs in hand!");
            }
            else
            {
                cardIdSet.Add(card.Id);
            }
        }

        int size = cards.Count;
        //if no cards, return
        if (size <= 0)
        {
            return;
        }

        float cardWidth = cards[0].wrapper.transform.GetComponent<BoxCollider>().size.x + 0.24f - 0.15f * size;
        float rotation_x = 70f;

        for (int k = 0; k < size; k++)
        {
            int pos = -((size - 1) / 2) + k;
            float vOffset = -0.15f * Mathf.Abs(pos) + Random.Range(-0.05f, 0.05f);
            cards[k].wrapper.Renderer.sortingOrder = 10 + k;

            Vector3 adjustedPos = new Vector3(pos * cardWidth / 1.11f, 0.2f * pos, vOffset);
            float tweenTime = 0.1f;
            LeanTween.moveLocal(cards[k].wrapper.gameObject, adjustedPos, tweenTime);
            LeanTween.rotateLocal(cards[k].wrapper.gameObject, new Vector3(rotation_x, pos * 4f, 0), tweenTime);
        }
    }
}
