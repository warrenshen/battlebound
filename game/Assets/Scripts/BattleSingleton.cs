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
		Debug.Log("ChallengeTurnTakenMessage received.");
		GSData scriptData = message.ScriptData;
		ProcessChallengeScriptData(scriptData);
	}

	private void ChallengeWonMessageHandler(ChallengeWonMessage message)
	{
		Debug.Log("ChallengeWonMessage received.");
	}

	private void ChallengeLostMessageHandler(ChallengeLostMessage message)
	{
		Debug.Log("ChallengeLostMessage received.");
	}

	private void ChallengeTimeRunningOutMessageHandler(ScriptMessage_ChallengeTimeRunningOutMessage message)
	{
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

		string playerJSON = scriptData.GetGSData("playerState").JSON;
        string opponentJSON = scriptData.GetGSData("opponentState").JSON;
		this.playerState = JsonUtility.FromJson<PlayerState>(playerJSON);
		this.opponentState = JsonUtility.FromJson<PlayerState>(opponentJSON);

		Debug.Log(this.playerState.ToString());
		Debug.Log(this.opponentState.ToString());
        // Call some function in BattleManager so Nick can react to events.
	}

	public void SendChallengeEndTurnRequest()
    {
		if (this.challengeId == null)
		{
			Debug.LogError("Cannot send ChallengeEndTurn request without challengeId set.");
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

	public void SendChallengePlayCardRequest(
		string cardId,
		PlayCardAttributes attributes
	)
	{
		if (this.challengeId == null)
        {
			Debug.LogError("Cannot send ChallengeEndTurn request without challengeId set.");
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
			Debug.LogError("Cannot send ChallengeCardAttack request without challengeId set.");
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
