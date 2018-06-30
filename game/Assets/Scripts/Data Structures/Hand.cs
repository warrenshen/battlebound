using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Hand
{
    [SerializeField]
    private List<CardObject> cardObjects;

    private string name;

    public Hand(Player player)
    {
        this.name = player.Name;
        this.cardObjects = new List<CardObject>();
    }

    public int Size()
    {
        return cardObjects.Count;
    }

    public CardObject GetCardObjectByCardId(string cardId)
    {
        foreach (CardObject cardObject in this.cardObjects)
        {
            if (cardObject.Card.Id == cardId)
            {
                return cardObject;
            }
        }

        return null;
    }

    public CardObject GetCardObjectByIndex(int index)
    {
        return this.cardObjects.ElementAt(index);
    }

    public void AddCardObject(CardObject cardObject)
    {
        this.cardObjects.Add(cardObject);
        this.RepositionCards();
    }

    public void Discard(int count)
    {

    }

    public void RemoveByCardId(string cardId)
    {
        int removeIndex = this.cardObjects.FindIndex(cardObject => cardObject.Card.Id == cardId);
        this.cardObjects.RemoveAt(removeIndex);
        this.RepositionCards();
    }

    public void RepositionCards(float verticalShift = 0)
    {
        int size = this.cardObjects.Count;
        //if no cards, return
        if (size <= 0)
        {
            return;
        }

        HashSet<string> cardIdSet = new HashSet<string>();
        foreach (CardObject cardObject in this.cardObjects)
        {
            if (cardObject.Card.Id != "HIDDEN" && cardIdSet.Contains(cardObject.Card.Id))
            {
                Debug.LogError("Duplicate card IDs in hand!");
            }
            else
            {
                cardIdSet.Add(cardObject.Card.Id);
            }
        }

        float cardWidth = this.cardObjects[0].transform.GetComponent<BoxCollider>().size.x + 0.24f - 0.15f * size;
        float rotation_x = 70f;

        for (int k = 0; k < size; k++)
        {
            float pos = -((size - 1) / 2.0f) + k;
            float vertical = -0.15f * Mathf.Abs(pos) + Random.Range(-0.05f, 0.05f);

            CardObject cardObject = this.cardObjects[k];
            //cardObject.Renderer.sortingOrder = 10 + k;

            Vector3 adjustedPos = new Vector3(pos * cardWidth / 1.11f, 0.2f * pos, vertical) + verticalShift * cardObject.transform.forward;
            float tweenTime = 0.1f;
            LeanTween.moveLocal(cardObject.gameObject, adjustedPos, tweenTime);
            LeanTween.rotateLocal(cardObject.gameObject, new Vector3(rotation_x, pos * 4f, 0), tweenTime);
        }
    }

    public void RecedeCards()
    {
        RepositionCards(-0.8f);
    }
}
