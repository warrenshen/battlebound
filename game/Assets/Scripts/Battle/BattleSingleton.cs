using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Api.Messages;

public class BattleSingleton : Singleton<BattleSingleton>
{
    public const int ENVIRONMENT_NORMAL = 0;
    public const int ENVIRONMENT_TEST = 1;

    private int environment;
    public int Environment => environment;

    private PlayerState playerState;
    public PlayerState PlayerState => playerState;

    private PlayerState opponentState;
    public PlayerState OpponentState => opponentState;

    private int nonce = -1;

    private string challengeId;
    public string ChallengeId => challengeId;

    private bool challengeStarted;
    public bool ChallengeStarted => challengeStarted;

    private int moveCount;
    public int MoveCount => moveCount;

    private int spawnCount;
    public int SpawnCount => spawnCount;

    private List<ChallengeMove> initServerMoves;
    public List<ChallengeMove> InitServerMoves => initServerMoves;

    private List<ChallengeCard> initDeadCards;
    public List<ChallengeCard> InitDeadCards => initDeadCards;

    // List of moves waiting to be sent to BattleManager.
    private List<ChallengeMove> moveQueue;

    private void Awake()
    {
        base.Awake();

        if (this.isDestroyed)
        {
            return;
        }

        this.environment = ENVIRONMENT_NORMAL;

        this.challengeStarted = false;

        this.moveCount = 0;
        this.spawnCount = 0;

        this.moveQueue = new List<ChallengeMove>();

        ChallengeIssuedMessage.Listener = ChallengeIssuedMessageHandler;
        ChallengeStartedMessage.Listener = ChallengeStartedMessageHandler;
        ChallengeTurnTakenMessage.Listener = ChallengeTurnTakenMessageHandler;
        ChallengeWonMessage.Listener = ChallengeWonMessageHandler;
        ChallengeLostMessage.Listener = ChallengeLostMessageHandler;
        ScriptMessage_ChallengePlayMulliganMessage.Listener = ChallengePlayMulliganMessageHandler;
        ScriptMessage_ChallengeTimeRunningOutMessage.Listener = ChallengeTimeRunningOutMessageHandler;
        ScriptMessage_ChallengeSendChatMessage.Listener = ChallengeSendChatMessageHandler;
    }

    private void Update()
    {
        if (this.moveQueue.Count > 0)
        {
            EmitChallengeMove();
        }
    }

    public void SetEnvironmentTest()
    {
        this.environment = ENVIRONMENT_TEST;
    }

    public bool IsEnvironmentTest()
    {
        return this.environment == ENVIRONMENT_TEST;
    }

    private void ChallengeIssuedMessageHandler(ChallengeIssuedMessage message)
    {
        Debug.Log("ChallengeIssuedMessage received.");
    }

    private void ChallengeStartedMessageHandler(ChallengeStartedMessage message)
    {
        Debug.Log("ChallengeStartedMessage received.");

        GSData scriptData = message.ScriptData;
        InitializeChallenge(scriptData);

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
        GSData scriptData = message.ScriptData;
        if (IsMessageChallengeIdValid(scriptData))
        {
            ProcessChallengeScriptData(scriptData);

            string challengeEndStateJson = scriptData.GetGSData("challengeEndState").JSON;
            ChallengeEndState challengeEndState = JsonUtility.FromJson<ChallengeEndState>(challengeEndStateJson);
            BattleState.Instance().SetChallengeEndState(challengeEndState);
        }
    }

    private void ChallengeLostMessageHandler(ChallengeLostMessage message)
    {
        GSData scriptData = message.ScriptData;
        if (IsMessageChallengeIdValid(scriptData))
        {
            ProcessChallengeScriptData(scriptData);

            string challengeEndStateJson = scriptData.GetGSData("challengeEndState").JSON;
            ChallengeEndState challengeEndState = JsonUtility.FromJson<ChallengeEndState>(challengeEndStateJson);
            BattleState.Instance().SetChallengeEndState(challengeEndState);
        }
    }

    private void ChallengePlayMulliganMessageHandler(ScriptMessage_ChallengePlayMulliganMessage message)
    {
        GSData scriptData = message.Data;
        if (IsMessageChallengeIdValid(scriptData))
        {
            ProcessChallengeScriptData(scriptData);
        }
    }

    private void ChallengeTimeRunningOutMessageHandler(ScriptMessage_ChallengeTimeRunningOutMessage message)
    {
        // Call some function in BattleManager so Nick can react to event.
        Debug.Log("ChallengeTimeRunningOutMessage received.");
    }

    private void ChallengeSendChatMessageHandler(ScriptMessage_ChallengeSendChatMessage message)
    {
        GSData scriptData = message.Data;
        if (IsMessageChallengeIdValid(scriptData))
        {
            if (scriptData.GetInt("chatId") != null)
            {
                int chatId = (int)scriptData.GetInt("chatId");
                BattleManager.Instance.ShowOpponentChat(chatId);
            }
        }
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
        bool shouldQueueMoves = true
    )
    {
        int messageNonce = (int)scriptData.GetInt("nonce");
        if (messageNonce <= this.nonce)
        {
            return;
        }

        this.nonce = messageNonce;

        string playerJson = scriptData.GetGSData("playerState").JSON;
        string opponentJson = scriptData.GetGSData("opponentState").JSON;
        this.playerState = JsonUtility.FromJson<PlayerState>(playerJson);
        this.opponentState = JsonUtility.FromJson<PlayerState>(opponentJson);

        if (scriptData.GetInt("spawnCount") == null)
        {
            this.spawnCount = 0;
        }
        else
        {
            this.spawnCount = (int)scriptData.GetInt("spawnCount");
        }

        if (scriptData.GetInt("moveCount") == null)
        {
            this.moveCount = 0;
        }
        else
        {
            this.moveCount = (int)scriptData.GetInt("moveCount");
        }

        if (BattleManager.Instance == null)
        {
            return;
        }

        // Logic to load in existing moves for resume challenge case.
        List<GSData> movesData = scriptData.GetGSDataList("moves");
        if (movesData != null)
        {
            List<ChallengeMove> challengeMoves = new List<ChallengeMove>();
            foreach (GSData moveData in movesData)
            {
                challengeMoves.Add(JsonUtility.FromJson<ChallengeMove>(moveData.JSON));
            }
            this.initServerMoves = challengeMoves;
        }
        else
        {
            this.initServerMoves = new List<ChallengeMove>();
        }

        // Logic to load in existing dead cards for resume challenge case.
        List<GSData> deadCardsData = scriptData.GetGSDataList("deadCards");
        if (deadCardsData != null)
        {
            List<ChallengeCard> deadCards = new List<ChallengeCard>();
            foreach (GSData deadCardData in deadCardsData)
            {
                deadCards.Add(JsonUtility.FromJson<ChallengeCard>(deadCardData.JSON));
            }
            this.initDeadCards = deadCards;
        }
        else
        {
            this.initDeadCards = new List<ChallengeCard>();
        }

        List<GSData> newMovesData = scriptData.GetGSDataList("newMoves");
        List<ChallengeMove> newChallengeMoves = new List<ChallengeMove>();
        foreach (GSData moveData in newMovesData)
        {
            newChallengeMoves.Add(JsonUtility.FromJson<ChallengeMove>(moveData.JSON));
        }

        if (shouldQueueMoves)
        {
            QueueChallengeMoves(newChallengeMoves);
        }
    }

    public List<Card> GetMulliganCards(string playerId)
    {
        if (this.playerState.Id == playerId)
        {
            return this.playerState.GetCardsMulligan();
        }
        else
        {
            return this.opponentState.GetCardsMulligan();
        }
    }

    public void EmitChallengeMove()
    {
        if (BattleState.Instance().CanReceiveChallengeMove())
        {
            ChallengeMove challengeMove = this.moveQueue[0];
            this.moveQueue.RemoveAt(0);
            BattleState.Instance().ReceiveChallengeMove(challengeMove);
        }
    }

    public void QueueChallengeMoves(List<ChallengeMove> challengeMoves)
    {
        this.moveQueue.AddRange(challengeMoves);
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
        OnChallengeRequestError(response, "ChallengeEndTurn");
    }

    public void SendChallengePlayMulliganRequest(List<string> cardIds)
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send ChallengeSurrender request without challengeId set.");
            return;
        }

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("ChallengePlayMulligan");
        request.SetEventAttribute("challengeId", this.challengeId);
        request.SetEventAttribute("cardIds", cardIds);
        request.Send(OnChallengePlayMulliganSuccess, OnChallengePlayMulliganError);
    }

    private void OnChallengePlayMulliganSuccess(LogEventResponse response)
    {
        Debug.Log("ChallengePlayMulligan request success.");
    }

    private void OnChallengePlayMulliganError(LogEventResponse response)
    {
        Debug.Log("ChallengePlayMulligan request error.");
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
        OnChallengeRequestError(response, "ChallengePlayCard");
    }

    public void SendChallengePlaySpellTargetedRequest(
        string cardId,
        PlaySpellTargetedAttributes attributes
    )
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send SendChallengePlaySpellTargeted request without challengeId set.");
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
        OnChallengeRequestError(response, "ChallengePlaySpellTargeted");
    }

    public void SendChallengePlaySpellUntargetedRequest(string cardId)
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send SendChallengePlaySpellUntargeted request without challengeId set.");
            return;
        }

        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengePlaySpellUntargeted");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.SetEventAttribute("cardId", cardId);
        request.Send(OnChallengePlaySpellTargetedSuccess, OnChallengePlaySpellTargetedError);
    }

    private void OnChallengePlaySpellUntargetedSuccess(LogChallengeEventResponse response)
    {
        Debug.Log("ChallengePlaySpellUntargeted request success.");
    }

    private void OnChallengePlaySpellUntargetedError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengePlaySpellUntargeted request error.");
        OnChallengeRequestError(response, "ChallengePlaySpell");
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
        OnChallengeRequestError(response, "ChallengeCardAttack");
    }

    private void SendChallengeSendChatRequest(int chatId)
    {
        if (this.challengeId == null)
        {
            Debug.LogWarning("Cannot send ChallengeSendChat request without challengeId set.");
            return;
        }

        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengeSendChat");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.SetEventAttribute("chatId", chatId);
        request.Send(OnChallengeSendChatSuccess, OnChallengeSendChatError);
    }

    private void OnChallengeSendChatSuccess(LogChallengeEventResponse response)
    {
        Debug.Log("ChallengeSendChat request success.");
    }

    private void OnChallengeSendChatError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeSendChat request error.");
    }

    private void OnChallengeRequestError(LogChallengeEventResponse response, string requestName)
    {
        string devicePlayerStateString = JsonUtility.ToJson(BattleState.Instance().GetPlayerState());
        string deviceOpponentStateString = JsonUtility.ToJson(BattleState.Instance().GetOpponentState());

        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("ChallengeLogDeviceError");
        request.SetEventAttribute("challengeId", this.challengeId);
        request.SetEventAttribute("requestName", requestName);
        request.SetEventAttribute("errorMessage", response.Errors.GetString("errorMessage"));
        request.SetEventAttribute("stackTrace", string.Join(",", response.Errors.GetStringList("stackTrace")));
        request.SetEventAttribute("devicePlayerState", devicePlayerStateString);
        request.SetEventAttribute("deviceOpponentState", deviceOpponentStateString);
        request.Send(
            OnChallengeLogDeviceErrorSuccess,
            OnChallengeLogDeviceErrorError
        );

        // TODO: show something to user before doing this.
        SceneManager.LoadScene("Battle");
    }

    private void OnChallengeLogDeviceErrorSuccess(LogEventResponse response)
    {

    }

    private void OnChallengeLogDeviceErrorError(LogEventResponse response)
    {

    }

    public void SendFindMatchRequest(string matchCode, string deckName)
    {
        LogEventRequest request = new LogEventRequest();
        request.SetEventKey("FindMatch");
        request.SetEventAttribute("matchShortCode", matchCode);
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
        InitializeChallenge(scriptData);

        SceneManager.LoadScene("Battle");
    }

    private void OnGetActiveChallengeError(LogEventResponse response)
    {
        Debug.LogError("GetActiveChallenge request error.");
        // Show prompt to user and ask what they want to do.
    }

    public bool ComparePlayerStates(
        PlayerState devicePlayerState,
        PlayerState deviceOpponentState,
        int deviceMoveCount
    )
    {
        if (this.moveCount != deviceMoveCount)
        {
            return true;
        }

        bool doesMatch = true;

        PlayerState serverPlayerState = this.playerState;
        PlayerState serverOpponentState = this.opponentState;

        if (!serverPlayerState.Equals(devicePlayerState))
        {
            Debug.LogWarning("Server vs device player state mismatch.");
            Debug.LogWarning("Server: " + JsonUtility.ToJson(serverPlayerState));
            Debug.LogWarning("Device: " + JsonUtility.ToJson(devicePlayerState));
            Debug.LogWarning("First diff: " + serverPlayerState.FirstDiff(devicePlayerState));
            doesMatch = false;
        }
        else
        {
            if (FlagHelper.IsLogVerbose())
            {
                Debug.Log("Server vs device player state match.");
                Debug.Log("State: " + JsonUtility.ToJson(serverPlayerState));
            }
        }

        if (!serverOpponentState.Equals(deviceOpponentState))
        {
            Debug.LogWarning("Server vs device opponent state mismatch.");
            Debug.LogWarning("Server: " + JsonUtility.ToJson(serverOpponentState));
            Debug.LogWarning("Device: " + JsonUtility.ToJson(deviceOpponentState));
            Debug.LogWarning("First diff: " + serverOpponentState.FirstDiff(deviceOpponentState));
            doesMatch = false;
        }
        else
        {
            if (FlagHelper.IsLogVerbose())
            {
                Debug.Log("Server vs device opponent state match.");
                Debug.Log("State: " + JsonUtility.ToJson(serverOpponentState));
            }
        }

        return doesMatch;
    }

    private void InitializeChallenge(GSData scriptData)
    {
        this.challengeId = scriptData.GetString("challengeId");
        Debug.Log("Setting challengeId to: " + this.challengeId);

        ProcessChallengeScriptData(scriptData, false);

        this.challengeStarted = true;
    }
}
