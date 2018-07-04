using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class DeckStore
{
    private bool containsData;
    private UnityAction awaitingAction;

    private Dictionary<string, CardRaw> cardIdToCard;
    public Dictionary<string, CardRaw> CardIdToCard => cardIdToCard;

    private Dictionary<string, List<CardRaw>> deckNameToDeck;
    public Dictionary<string, List<CardRaw>> DeckNameToDeck => deckNameToDeck;

    private static DeckStore instance;

    public static DeckStore Instance()
    {
        if (instance == null)
        {
            instance = new DeckStore();
        }
        return instance;
    }

    public DeckStore()
    {
        ResetStore();
    }

    private void ResetStore()
    {
        this.containsData = false;
        this.cardIdToCard = new Dictionary<string, CardRaw>();
        this.deckNameToDeck = new Dictionary<string, List<CardRaw>>();
    }

    public void GetDecksWithCallback(UnityAction callback)
    {
        if (this.containsData)
        {
            callback.Invoke();
        }
        else
        {
            this.awaitingAction = callback;
            SendGetCollectionRequest();
        }
    }

    //public List<string> GetDeckMetadatas()
    //{
    //    List<string> deckNames = new List<string>(this.deckNameToDeck.Keys);
    //    foreach (string deckName in deckNames)
    //    {
    //        List<CardRaw> cards = this.deckNameToDeck[deckName];
    //        int minionCount = 0;
    //        int spellCount = 0;
    //        int weaponCount = 0;
    //        int structureCount = 0;

    //        foreach (CardRaw card in cards)
    //        {
    //            if (card.Category == CardRaw.CardType.Creature)
    //            {
    //                minionCount += 1;
    //            }
    //            else if (card.Category == CardRaw.CardType.Spell)
    //            {
    //                spellCount += 1;
    //            }
    //            else if (card.Category == CardRaw.CardType.Weapon)
    //            {
    //                weaponCount += 1;
    //            }
    //            else if (card.Category == CardRaw.CardType.Structure)
    //            {
    //                structureCount += 1;
    //            }
    //        }
    //    }
    //}

    public List<CardRaw> GetDeckByDeckName(string deckName)
    {
        if (!this.deckNameToDeck.ContainsKey(deckName))
        {
            Debug.LogError("Invalid deck name paramter - deck does not exist.");
            return new List<CardRaw>();
        }
        return this.deckNameToDeck[deckName];
    }

    private void SendGetCollectionRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetPlayerCards");
        request.Send(OnGetCollectionSuccess, OnGetCollectionError);
    }

    private void OnGetCollectionSuccess(LogEventResponse response)
    {
        ResetStore();

        GSData decksData = response.ScriptData.GetGSData("decks");

        List<GSData> cardsData = response.ScriptData.GetGSDataList("cards");
        foreach (GSData cardData in cardsData)
        {
            CardRaw card = JsonUtility.FromJson<CardRaw>(cardData.JSON);
            this.cardIdToCard.Add(card.Id, card);
        }

        foreach (string deckName in decksData.BaseData.Keys)
        {
            List<string> cardIds = decksData.GetStringList(deckName);
            List<CardRaw> deckCards = new List<CardRaw>();

            foreach (string cardId in cardIds)
            {
                deckCards.Add(this.cardIdToCard[cardId]);
            }

            this.deckNameToDeck.Add(deckName, deckCards);
        }

        this.containsData = true;
        InvokeAwaitingAction();
    }

    private void OnGetCollectionError(LogEventResponse response)
    {
        Debug.Log("Gamesparks Request Error!");
    }

    private void InvokeAwaitingAction()
    {
        if (this.awaitingAction != null)
        {
            this.awaitingAction.Invoke();
            this.awaitingAction = null;
        }
    }
}
