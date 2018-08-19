﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    private string name;
    public string Name => name;

    private List<Card> cards;
    public List<Card> Cards => cards;

    public Deck(string name, List<Card> cards)
    {
        this.name = name;
        this.cards = cards;
    }

    public void SetCards(List<Card> cards)
    {
        this.cards = cards;
    }

    public void AddCard(Card card)
    {
        this.cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        this.cards.Remove(card);
    }

    public int Size()
    {
        return cards.Count;
    }

    public override string ToString()
    {
        string output = "";
        foreach (Card card in cards)
        {
            output += card.Name + ", ";
        }
        return output.Substring(0, output.Length - 1);
    }
}
