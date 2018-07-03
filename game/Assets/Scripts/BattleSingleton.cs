﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;

public class BattleSingleton : Singleton<BattleSingleton>
{
    private PlayerState playerState;
    public PlayerState PlayerState => playerState;
    private PlayerState opponentState;
    public PlayerState OpponentState => opponentState;

    private int nonce = -1;

    private string challengeId;
    public string ChallengeId => challengeId;

    private bool challengeStarted;
    public bool ChallengeStarted => challengeStarted;

    private void Awake()
    {
        base.Awake();

        if (this.isDestroyed)
        {
            return;
        }

        this.challengeStarted = false;

        ChallengeIssuedMessage.Listener = ChallengeIssuedMessageHandler;
        ChallengeStartedMessage.Listener = ChallengeStartedMessageHandler;
        ChallengeTurnTakenMessage.Listener = ChallengeTurnTakenMessageHandler;
        ChallengeWonMessage.Listener = ChallengeWonMessageHandler;
        ChallengeLostMessage.Listener = ChallengeLostMessageHandler;
        ScriptMessage_ChallengeTimeRunningOutMessage.Listener = ChallengeTimeRunningOutMessageHandler;
    }

    private void ChallengeIssuedMessageHandler(ChallengeIssuedMessage message)
    {
        Debug.Log("ChallengeIssuedMessage received.");
    }

    private void ChallengeStartedMessageHandler(ChallengeStartedMessage message)
    {
        Debug.Log("ChallengeStartedMessage received.");

        GSData scriptData = message.ScriptData;
        this.challengeId = scriptData.GetString("challengeId");
        Debug.Log("Setting challengeId to: " + this.challengeId);
        ProcessChallengeScriptData(scriptData);

        this.challengeStarted = true;

        SceneManager.LoadScene("Battle");
    }

    private void ChallengeTurnTakenMessageHandler(ChallengeTurnTakenMessage message)
    {
        GSData scriptData = message.ScriptData;
        if (IsMessageChallengeIdValid(scriptData))
        {
            ProcessChallengeScriptData(scriptData);
        }
    }

    private void ChallengeWonMessageHandler(ChallengeWonMessage message)
    {
        Debug.Log("ChallengeWonMessage received.");
        GSData scriptData = message.ScriptData;
        if (IsMessageChallengeIdValid(scriptData))
        {
            Debug.Log("Challenge won!");

            string challengeEndStateJson = scriptData.GetString("challengeEndState");
            ChallengeEndState challengeEndState = JsonUtility.FromJson<ChallengeEndState>(challengeEndStateJson);
            BattleManager.Instance.ReceiveChallengeEndState(challengeEndState);
        }
    }

    private void ChallengeLostMessageHandler(ChallengeLostMessage message)
    {
        Debug.Log("ChallengeLostMessage received.");
        GSData scriptData = message.ScriptData;
        if (IsMessageChallengeIdValid(scriptData))
        {
            Debug.Log("Challenge lost...");

            string challengeEndStateJson = scriptData.GetString("challengeEndState");
            ChallengeEndState challengeEndState = JsonUtility.FromJson<ChallengeEndState>(challengeEndStateJson);
            BattleManager.Instance.ReceiveChallengeEndState(challengeEndState);
        }
    }

    private void ChallengeTimeRunningOutMessageHandler(ScriptMessage_ChallengeTimeRunningOutMessage message)
    {
        // Call some function in BattleManager so Nick can react to event.
        Debug.Log("ChallengeTimeRunningOutMessage received.");
    }

    private bool IsMessageChallengeIdValid(GSData scriptData)
    {
        string messageChallengeId = scriptData.GetString("challengeId");

        if (!messageChallengeId.Equals(BattleSingleton.Instance.ChallengeId))
        {
            Debug.Log("Got message with different challenge ID than expected: " + BattleSingleton.Instance.ChallengeId + " vs " + messageChallengeId + ".");
            return false;
        }

        return true;
    }

    private void ProcessChallengeScriptData(
        GSData scriptData,
        bool shouldEmitMoves = true
    )
    {
        int messageNonce = (int)scriptData.GetInt("nonce");
        Debug.Log("Got message with nonce: " + messageNonce.ToString());
        if (messageNonce <= this.nonce)
        {
            return;
        }

        Debug.Log("Updating nonce and player states.");
        this.nonce = messageNonce;

        string playerJson = scriptData.GetGSData("playerState").JSON;
        string opponentJson = scriptData.GetGSData("opponentState").JSON;
        this.playerState = JsonUtility.FromJson<PlayerState>(playerJson);
        this.opponentState = JsonUtility.FromJson<PlayerState>(opponentJson);

        if (BattleManager.Instance == null)
        {
            return;
        }

        List<GSData> movesData = scriptData.GetGSDataList("newMoves");
        List<ChallengeMove> challengeMoves = new List<ChallengeMove>();
        foreach (GSData moveData in movesData)
        {
            challengeMoves.Add(JsonUtility.FromJson<ChallengeMove>(moveData.JSON));
        }

        if (shouldEmitMoves)
        {
            EmitChallengeMoves(challengeMoves);
        }
    }

    public void EmitChallengeMoves(List<ChallengeMove> challengeMoves)
    {
        foreach (ChallengeMove challengeMove in challengeMoves)
        {
            Debug.Log(JsonUtility.ToJson(challengeMove));

            if (challengeMove.Category == ChallengeMove.MOVE_CATEGORY_END_TURN)
            {
                BattleManager.Instance.ReceiveMoveEndTurn(
                    challengeMove.PlayerId
                );
            }
            else if (challengeMove.Category == ChallengeMove.MOVE_CATEGORY_DRAW_CARD)
            {
                Player owner = BattleManager.Instance.PlayerIdToPlayer[challengeMove.PlayerId];
                Card card = challengeMove.Attributes.Card.GetCard();

                BattleManager.Instance.ReceiveMoveDrawCard(
                    challengeMove.PlayerId,
                    card
                );
            }
            else if (challengeMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_MINION)
            {
                Player owner = BattleManager.Instance.PlayerIdToPlayer[challengeMove.PlayerId];
                Card card = challengeMove.Attributes.Card.GetCard();

                if (card.GetType() == typeof(CreatureCard))
                {
                    BattleManager.Instance.ReceiveMovePlayMinion(
                        challengeMove.PlayerId,
                        challengeMove.Attributes.CardId,
                        card as CreatureCard,
                        challengeMove.Attributes.HandIndex,
                        challengeMove.Attributes.FieldIndex
                    );
                }
                else
                {
                    Debug.LogError("Invalid card category for play minion move");
                }
            }
            else if (challengeMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_UNTARGETED)
            {
                Player owner = BattleManager.Instance.PlayerIdToPlayer[challengeMove.PlayerId];
                Card card = challengeMove.Attributes.Card.GetCard();

                if (card.GetType() == typeof(SpellCard))
                {
                    BattleManager.Instance.ReceiveMovePlaySpellUntargeted(
                        challengeMove.PlayerId,
                        challengeMove.Attributes.CardId,
                        card as SpellCard,
                        challengeMove.Attributes.HandIndex
                    );
                }
                else
                {
                    Debug.LogError("Invalid card category for play spell general move");
                }
            }
            else if (challengeMove.Category == ChallengeMove.MOVE_CATEGORY_PLAY_SPELL_TARGETED)
            {
                Player owner = BattleManager.Instance.PlayerIdToPlayer[challengeMove.PlayerId];
                Card card = challengeMove.Attributes.Card.GetCard();

                if (card.GetType() == typeof(SpellCard))
                {
                    BattleManager.Instance.ReceiveMovePlaySpellTargeted(
                        challengeMove.PlayerId,
                        challengeMove.Attributes.CardId,
                        card as SpellCard,
                        challengeMove.Attributes.HandIndex,
                        challengeMove.Attributes.FieldId,
                        challengeMove.Attributes.TargetId
                    );
                }
                else
                {
                    Debug.LogError("Invalid card category for play spell targeted move");
                }
            }
            else if (challengeMove.Category == ChallengeMove.MOVE_CATEGORY_CARD_ATTACK)
            {
                BattleManager.Instance.ReceiveMoveCardAttack(
                    challengeMove.PlayerId,
                    challengeMove.Attributes.CardId,
                    challengeMove.Attributes.FieldId,
                    challengeMove.Attributes.TargetId
                );
            }
        }

        if (!BattleManager.Instance.Initialized)
        {
            return;
        }

        PlayerState devicePlayerState = BattleManager.Instance.GetPlayerState();
        PlayerState deviceOpponentState = BattleManager.Instance.GetOpponentState();

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            if (!this.playerState.Equals(devicePlayerState))
            {
                Debug.LogWarning("Server vs device player state mismatch.");
                Debug.LogWarning("Server: " + JsonUtility.ToJson(this.playerState));
                Debug.LogWarning("Device: " + JsonUtility.ToJson(devicePlayerState));
                Debug.LogWarning("First diff: " + this.playerState.FirstDiff(devicePlayerState));
            }
            else
            {
                Debug.Log("Server vs device player state match.");
                Debug.Log("State: " + JsonUtility.ToJson(this.playerState));
            }

            if (!this.opponentState.Equals(deviceOpponentState))
            {
                Debug.LogWarning("Server vs device opponent state mismatch.");
                Debug.LogWarning("Server: " + JsonUtility.ToJson(this.opponentState));
                Debug.LogWarning("Device: " + JsonUtility.ToJson(deviceOpponentState));
                Debug.LogWarning("First diff: " + this.opponentState.FirstDiff(deviceOpponentState));
            }
            else
            {
                Debug.Log("Server vs device opponent state match.");
                Debug.Log("State: " + JsonUtility.ToJson(this.opponentState));
            }
        }
        else
        {
            Debug.Log("Player state: " + JsonUtility.ToJson(devicePlayerState));
            Debug.Log("Opponent state: " + JsonUtility.ToJson(deviceOpponentState));
        }
    }

    public void SendChallengeEndTurnRequest()
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send ChallengeEndTurn request without challengeId set.");
            return;
        }

        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengeEndTurn");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.Send(OnChallengeEndTurnSuccess, OnChallengeEndTurnError);
    }

    private void OnChallengeEndTurnSuccess(LogChallengeEventResponse response)
    {
        Debug.Log("ChallengeEndTurn request success.");
    }

    private void OnChallengeEndTurnError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeEndTurn request error.");
        OnChallengeRequestError();
    }

    public void SendChallengeSurrenderRequest()
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send ChallengeSurrender request without challengeId set.");
            return;
        }

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("ChallengeSurrender");
        request.SetEventAttribute("challengeId", this.challengeId);
        request.Send(OnChallengeSurrenderSuccess, OnChallengeSurrenderError);
    }

    private void OnChallengeSurrenderSuccess(LogEventResponse response)
    {
        Debug.Log("ChallengeSurrender request success.");
    }

    private void OnChallengeSurrenderError(LogEventResponse response)
    {
        Debug.Log("ChallengeSurrender request error.");
    }

    public void SendChallengePlayCardRequest(
        string cardId,
        PlayCardAttributes attributes
    )
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send ChallengePlayCard request without challengeId set.");
            return;
        }

        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengePlayCard");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.SetEventAttribute("cardId", cardId);
        request.SetEventAttribute("attributesString", JsonUtility.ToJson(attributes));
        request.Send(OnChallengePlayCardSuccess, OnChallengePlayCardError);
    }

    private void OnChallengePlayCardSuccess(LogChallengeEventResponse response)
    {
        Debug.Log("ChallengePlayCard request success.");
    }

    private void OnChallengePlayCardError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengePlayCard request error.");
        OnChallengeRequestError();
    }

    public void SendChallengePlaySpellTargetedRequest(
        string cardId,
        PlaySpellTargetedAttributes attributes
    )
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send SendChallengePlaySpellTargeted( request without challengeId set.");
            return;
        }

        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengePlaySpellTargeted");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.SetEventAttribute("cardId", cardId);
        request.SetEventAttribute("attributesString", JsonUtility.ToJson(attributes));
        request.Send(OnChallengePlaySpellTargetedSuccess, OnChallengePlaySpellTargetedError);
    }

    private void OnChallengePlaySpellTargetedSuccess(LogChallengeEventResponse response)
    {
        Debug.Log("ChallengePlaySpellTargeted request success.");
    }

    private void OnChallengePlaySpellTargetedError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengePlaySpellTargeted request error.");
        OnChallengeRequestError();
    }

    public void SendChallengeCardAttackRequest(
        string cardId,
        CardAttackAttributes attributes
    )
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send ChallengeCardAttack request without challengeId set.");
            return;
        }

        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengeCardAttack");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.SetEventAttribute("cardId", cardId);
        request.SetEventAttribute("attributesString", JsonUtility.ToJson(attributes));
        request.Send(OnChallengeCardAttackSuccess, OnChallengeCardAttackError);
    }

    private void OnChallengeCardAttackSuccess(LogChallengeEventResponse response)
    {
        Debug.Log("ChallengeCardAttack request success.");
    }

    private void OnChallengeCardAttackError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeCardAttack request error.");
        OnChallengeRequestError();
    }

    private void OnChallengeRequestError()
    {
        SceneManager.LoadScene("Battle");
    }

    public void SendFindMatchRequest(string matchCode, string deckName)
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("FindMatch");
        request.SetEventAttribute("matchShortCode", "CasualMatch");
        request.SetEventAttribute("playerDeck", deckName);

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
        SendGetActiveChallengeRequest();
    }

    public void SendGetActiveChallengeRequest()
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("GetActiveChallenge");

        request.Send(
            OnGetActiveChallengeSuccess,
            OnGetActiveChallengeError
        );
    }

    private void OnGetActiveChallengeSuccess(LogEventResponse response)
    {
        Debug.Log("GetActiveChallenge request success.");

        GSData scriptData = response.ScriptData;
        this.challengeId = scriptData.GetString("challengeId");
        Debug.Log("Setting challengeId to: " + this.challengeId);
        ProcessChallengeScriptData(scriptData, false);

        this.challengeStarted = true;

        SceneManager.LoadScene("Battle");
    }

    private void OnGetActiveChallengeError(LogEventResponse response)
    {
        Debug.LogError("GetActiveChallenge request error.");
    }
}
