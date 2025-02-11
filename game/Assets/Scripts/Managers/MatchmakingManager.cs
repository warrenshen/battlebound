﻿using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;

using TMPro;

public class MatchmakingManager : MonoBehaviour
{
    public const string MATCH_TYPE_CASUAL = "CasualMatch";
    public const string MATCH_TYPE_RANKED = "RankedMatch";

    private string matchType = MATCH_TYPE_CASUAL;
    private string matchDeckName;

    [SerializeField]
    private Button casualMatchButton;
    [SerializeField]
    private Button rankedMatchButton;
    [SerializeField]
    private Button findMatchButton;

    [SerializeField]
    private GameObject deckScrollViewContent;
    [SerializeField]
    private GameObject deckButton;
    [SerializeField]
    private List<CardObject> renderedCardObjects;

    public void Awake()
    {
        MatchFoundMessage.Listener = MatchFoundMessageHandler;
        MatchNotFoundMessage.Listener = MatchNotFoundMessageHandler;

        casualMatchButton.onClick.AddListener(SelectCasualMatch);
        rankedMatchButton.onClick.AddListener(SelectRankedMatch);
        findMatchButton.onClick.AddListener(FindMatch);

        renderedCardObjects = new List<CardObject>();
    }

    public void Start()
    {
        DeckStore.Instance().GetDecksWithCallback(Callback);
    }

    private void Callback()
    {
        List<string> deckNames = DeckStore.Instance().GetDeckNames();
        if (deckNames.Count <= 0)
        {
            Debug.LogError("No decks!");
            return;
        }
        RenderDecks(deckNames);

        this.matchDeckName = deckNames[0];
        ChangeSelectedDeck(this.matchDeckName);
    }

    private void ChangeSelectedDeck(string deckName)
    {
        List<Card> cards = DeckStore.Instance().GetCardsByDeckName(deckName);
        this.matchDeckName = deckName;
        RenderCards(cards);
    }

    private void RenderDecks(List<string> deckNames)
    {
        foreach (string deckName in deckNames)
        {
            GameObject created = Instantiate(deckButton, Vector3.zero, Quaternion.identity);
            created.transform.SetParent(deckScrollViewContent.transform);
            created.GetComponentInChildren<TextMeshProUGUI>().text = deckName;
            created.GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => ChangeSelectedDeck(deckName)));
        }
    }

    private void RenderCards(List<Card> cards)
    {
        foreach (CardObject rendered in this.renderedCardObjects)
        {
            rendered.Recycle();
        }

        this.renderedCardObjects = new List<CardObject>();

        foreach (Card card in cards)
        {
            CardObject cardObject = CardObject.Create(card);
            this.renderedCardObjects.Add(cardObject);
        }

        PositionCards();
    }

    private void PositionCards()
    {
        Transform pivot = GameObject.Find("Deck Pivot").transform;
        for (int index = 0; index < renderedCardObjects.Count; index++)
        {
            CardObject chosen = renderedCardObjects[index];
            Vector3 offset = (index % 6) * Vector3.right * 1.8f + (index / 6) * Vector3.down * 3.6f + (index % 6) * Vector3.forward * 0.05f;
            CardTween.move(chosen, pivot.position + offset, CardTween.TWEEN_DURATION);
        }
    }

    private void MatchFoundMessageHandler(MatchFoundMessage message)
    {
        Debug.Log("MatchFoundMessage received.");
    }

    private void MatchNotFoundMessageHandler(MatchNotFoundMessage message)
    {
        Debug.Log("MatchNotFoundMessage received.");
    }

    private void SelectCasualMatch()
    {
        this.matchType = MATCH_TYPE_CASUAL;
        FindMatch();
    }

    private void SelectRankedMatch()
    {
        this.matchType = MATCH_TYPE_RANKED;
        FindMatch();
    }

    private void FindMatch()
    {
        if (!FlagHelper.IsServerEnabled())
        {
            Application.LoadLevel("Battle");
        }

        if (this.matchDeckName == null)
        {
            Debug.LogError("Cannot send FindMatch request without deck name.");
            return;
        }

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("FindMatch");
        request.SetEventAttribute("playerDeck", this.matchDeckName);
        request.SetEventAttribute("matchShortCode", this.matchType);
        request.Send(
            OnFindMatchSuccess,
            OnFindMatchError
        );
    }

    private void OnFindMatchSuccess(LogEventResponse response)
    {
        Debug.Log("FindMatch request success.");
    }

    private void OnFindMatchError(LogEventResponse response)
    {
        Debug.LogError("FindMatch request error.");
        SendForfeitActiveChallengeRequest();
    }

    public void SendForfeitActiveChallengeRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("ForfeitActiveChallenge");

        request.Send(
            OnForfeitActiveChallengeSuccess,
            OnForfeitActiveChallengeError
        );
    }

    private void OnForfeitActiveChallengeSuccess(LogEventResponse response)
    {
        Debug.Log("ForfeitActiveChallenge request success.");
        FindMatch();
    }

    private void OnForfeitActiveChallengeError(LogEventResponse response)
    {
        Debug.LogError("ForfeitActiveChallenge request error.");
    }
}
