using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;

public class MatchmakingManager : MonoBehaviour
{
    public const string MATCH_TYPE_CASUAL = "MATCH_TYPE_CASUAL";
    public const string MATCH_TYPE_RANKED = "MATCH_TYPE_RANKED";

    private string matchType = MATCH_TYPE_CASUAL;
    private string matchDeckName;

    [SerializeField]
    private Button casualMatchButton;
    [SerializeField]
    private Button rankedMatchButton;
    [SerializeField]
    private Button findMatchButton;

    public void Awake()
    {
        MatchFoundMessage.Listener += MatchFoundMessageHandler;
        MatchNotFoundMessage.Listener += MatchNotFoundMessageHandler;

        casualMatchButton.onClick.AddListener(SelectCasualMatch);
        rankedMatchButton.onClick.AddListener(SelectRankedMatch);
        findMatchButton.onClick.AddListener(FindMatch);
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
        // TODO: render decks in left column.
        this.matchDeckName = deckNames[0];
    }

    private void ChangeSelectedDeck(string deckName)
    {
        List<CardRaw> cards = DeckStore.Instance().GetCardsByDeckName(deckName);
        // TODO: render cards in middle column.
        this.matchDeckName = deckName;
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
        Debug.Log("Selectin match type casual.");
        this.matchType = MATCH_TYPE_CASUAL;
    }

    private void SelectRankedMatch()
    {
        Debug.Log("Selecting match type ranked.");
        this.matchType = MATCH_TYPE_RANKED;
    }

    private void FindMatch()
    {
        if (this.matchDeckName == null)
        {
            Debug.LogError("Cannot send FindMatch request without deck name.");
            return;
        }

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("FindMatch");
        request.SetEventAttribute("playerDeck", this.matchDeckName);

        switch (this.matchType)
        {
            case MATCH_TYPE_RANKED:
                request.SetEventAttribute("matchShortCode", "RankedMatch");
                break;
            case MATCH_TYPE_CASUAL:
            default:
                request.SetEventAttribute("matchShortCode", "CasualMatch");
                break;
        }

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
    }
}
