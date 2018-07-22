using System;
using System.Linq;
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

    private Dictionary<string, Card> cardIdToCard;
    public Dictionary<string, Card> CardIdToCard => cardIdToCard;

    private Dictionary<string, List<string>> deckNameToDeck;
    public Dictionary<string, List<string>> DeckNameToDeck => deckNameToDeck;

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
        this.cardIdToCard = new Dictionary<string, Card>();
        this.deckNameToDeck = new Dictionary<string, List<string>>();
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

    public List<Card> GetCards()
    {
        return new List<Card>(this.cardIdToCard.Values);
    }

    public List<string> GetDeckNames()
    {
        return new List<string>(this.deckNameToDeck.Keys);
    }

    public List<Card> GetCardsByDeckName(string deckName)
    {
        if (!this.deckNameToDeck.ContainsKey(deckName))
        {
            Debug.LogError("Invalid deck name paramter - deck does not exist.");
            return new List<Card>();
        }

        List<string> cardIds = this.deckNameToDeck[deckName];
        List<Card> deckCards = new List<Card>();

        foreach (string cardId in cardIds)
        {
            deckCards.Add(this.cardIdToCard[cardId]);
        }

        return deckCards;
    }

    public List<string> GetCardIdsByDeckName(string deckName)
    {
        if (!this.deckNameToDeck.ContainsKey(deckName))
        {
            Debug.LogError("Invalid deck name paramter - deck does not exist.");
            return new List<string>();
        }

        return new List<string>(this.deckNameToDeck[deckName]);
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

    public void CreateUpdatePlayerDeckWithCallback(
        List<string> cardIds,
        string previousName,
        string name,
        UnityAction callback
    )
    {
        this.awaitingAction = callback;

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("CreateUpdatePlayerDeck");
        request.SetEventAttribute("cardIds", cardIds);
        request.SetEventAttribute("previousName", previousName);
        request.SetEventAttribute("name", name);
        request.Send(OnSaveCollectionSuccess, OnSaveCollectionError);
    }

    private void OnSaveCollectionSuccess(LogEventResponse response)
    {
        ResetStore();

        ParseScriptData(response.ScriptData);

        InvokeAwaitingAction();
    }

    private void OnSaveCollectionError(LogEventResponse response)
    {
        Debug.Log("Gamesparks Request Error!");
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

        ParseScriptData(response.ScriptData);

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

    private void ParseScriptData(GSData scriptData)
    {
        GSData decksData = scriptData.GetGSData("decks");
        List<GSData> cardsData = scriptData.GetGSDataList("cards");

        List<Card> cards = ParseCardsFromScriptData(cardsData);
        foreach (Card card in cards)
        {
            this.cardIdToCard.Add(card.Id, card);
        }

        foreach (string deckName in new List<string>(decksData.BaseData.Keys))
        {
            List<string> cardIds = decksData.GetStringList(deckName);
            this.deckNameToDeck.Add(deckName, cardIds);
        }

        this.containsData = true;
    }

    public List<Card> ParseCardsFromScriptData(List<GSData> cardsData)
    {
        List<Card> cards = new List<Card>();

        foreach (GSData cardData in cardsData)
        {
            int category = (int)cardData.GetInt("category");

            Card card;
            switch (category)
            {
                case (int)Card.CardType.Creature: //creature
                    card = CreatureCard.GetFromJson(cardData.JSON);
                    break;
                case (int)Card.CardType.Spell:  //spell
                    card = SpellCard.GetFromJson(cardData.JSON);
                    break;
                case (int)Card.CardType.Weapon:  //weapon
                    card = WeaponCard.GetFromJson(cardData.JSON);
                    break;
                case (int)Card.CardType.Structure:  //structure
                    card = StructureCard.GetFromJson(cardData.JSON);
                    break;
                default:
                    Debug.LogError("Card has no category/type field!");
                    return cards;
            }

            cards.Add(card);
        }

        return cards;
    }
}
