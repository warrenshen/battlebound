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
	private PlayerState opponentState;

	private string challengeId;
	private int nonce = -1;

	private new void Awake()
	{
		base.Awake();

        ChallengeIssuedMessage.Listener += ChallengeIssuedMessageHandler;
		ChallengeStartedMessage.Listener += ChallengeStartedMessageHandler;
		ChallengeTurnTakenMessage.Listener += ChallengeTurnTakenMessageHandler;
		ChallengeWonMessage.Listener += ChallengeWonMessageHandler;
		ChallengeLostMessage.Listener += ChallengeLostMessageHandler;
		ScriptMessage_ChallengeTimeRunningOutMessage.Listener += ChallengeTimeRunningOutMessageHandler;
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
		ProcessChallengeScriptData(scriptData);

		SceneManager.LoadScene("Battle");
	}

	private void ChallengeTurnTakenMessageHandler(ChallengeTurnTakenMessage message)
	{
		// Call some function in BattleManager so Nick can react to event.
		Debug.Log("ChallengeTurnTakenMessage received.");
		GSData scriptData = message.ScriptData;
		ProcessChallengeScriptData(scriptData);
	}

	private void ChallengeWonMessageHandler(ChallengeWonMessage message)
	{
		// Call some function in BattleManager so Nick can react to event.
		Debug.Log("ChallengeWonMessage received.");
	}

	private void ChallengeLostMessageHandler(ChallengeLostMessage message)
	{
		// Call some function in BattleManager so Nick can react to event.
		Debug.Log("ChallengeLostMessage received.");
	}

	private void ChallengeTimeRunningOutMessageHandler(ScriptMessage_ChallengeTimeRunningOutMessage message)
	{
		// Call some function in BattleManager so Nick can react to event.
		Debug.Log("ChallengeTimeRunningOutMessage received.");
	}

	private void ProcessChallengeScriptData(GSData scriptData)
	{
		string messageChallengeId = scriptData.GetString("challengeId");

		if (!messageChallengeId.Equals(this.challengeId))
		{
			Debug.Log("Got message with different challenge ID than expected.");
			return;
		}

		int messageNonce = (int) scriptData.GetInt("nonce");
		Debug.Log("Got message with nonce: " + messageNonce.ToString());
        if (messageNonce > this.nonce)
        {
			this.nonce = messageNonce;
			Debug.Log("Updating nonce and player states.");
        }
		else
		{
			return;
		}

		string playerJson = scriptData.GetGSData("playerState").JSON;
        string opponentJson = scriptData.GetGSData("opponentState").JSON;
		this.playerState = JsonUtility.FromJson<PlayerState>(playerJson);
		this.opponentState = JsonUtility.FromJson<PlayerState>(opponentJson);

		if (BattleManager.Instance == null)
		{
			return;
		}

		PlayerState devicePlayerState = BattleManager.Instance.GetPlayerState();
		PlayerState deviceOpponentState = BattleManager.Instance.GetOpponentState();

		if (!this.playerState.Equals(devicePlayerState))
		{
			Debug.LogWarning("Server vs device player state mismatch.");
			Debug.LogWarning("Server: " + JsonUtility.ToJson(this.playerState));
			Debug.LogWarning("Device: " + JsonUtility.ToJson(devicePlayerState));
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
		}
		else
		{
			Debug.Log("Server vs device opponent state match.");
			Debug.Log("State: " + JsonUtility.ToJson(this.opponentState));
		}

		List<GSData> movesData = scriptData.GetGSDataList("moves");
		List<ChallengeMove> challengeMoves = new List<ChallengeMove>();
		foreach (GSData moveData in movesData)
		{
			challengeMoves.Add(JsonUtility.FromJson<ChallengeMove>(moveData.JSON));
		}

		EmitChallengeMoves(challengeMoves);
	}

	public void EmitChallengeMoves(List<ChallengeMove> challengeMoves)
	{
		Debug.Log("BattleSingleton.EmitChallengeMoves called.");
		foreach (ChallengeMove challengeMove in challengeMoves)
		{
			Debug.Log(JsonUtility.ToJson(challengeMove));
			if (challengeMove.Category == ChallengeMove.CATEGORY_PLAY_CARD)
			{
				BattleManager.Instance.ReceivePlayCardMove(
					null,
					challengeMove.Attributes.CardId,
					challengeMove.Attributes.FieldIndex
				);
			}
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
			Debug.LogWarning("Cannot send ChallengeEndTurn request without challengeId set.");
            return;
        }

		LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengeEndTurn");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
		request.SetEventAttribute("cardId", cardId);
		request.SetEventAttribute("attributes", JsonUtility.ToJson(attributes));
		request.Send(OnChallengePlayCardSuccess, OnChallengePlayCardError);
	}

	private void OnChallengePlayCardSuccess(LogChallengeEventResponse response)
    {
		Debug.Log("ChallengeEndTurn request success.");
    }

	private void OnChallengePlayCardError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeEndTurn request error.");
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
        request.SetEventAttribute("attributes", JsonUtility.ToJson(attributes));
		request.Send(OnChallengeCardAttackSuccess, OnChallengeCardAttackError);
    }

	private void OnChallengeCardAttackSuccess(LogChallengeEventResponse response)
    {
		Debug.Log("ChallengeCardAttack request success.");
    }

	private void OnChallengeCardAttackError(LogChallengeEventResponse response)
    {
		Debug.LogError("ChallengeCardAttack request error.");
    }
}
