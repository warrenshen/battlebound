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
		GSData scriptData = message.ScriptData;
		ProcessChallengeScriptData(scriptData);
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
        LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengeEndTurn");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
        request.Send(OnChallengeEndTurnSuccess, OnChallengeEndTurnError);
    }

    private void OnChallengeEndTurnSuccess(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeEndTurn request success.");
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
		LogChallengeEventRequest request = new LogChallengeEventRequest();
        request.SetEventKey("ChallengeEndTurn");
        request.SetEventAttribute("challengeInstanceId", this.challengeId);
		request.SetEventAttribute("cardId", cardId);
		request.SetEventAttribute("attributes", JsonUtility.ToJson(attributes));
		request.Send(OnChallengePlayCardSuccess, OnChallengePlayCardError);
	}

	private void OnChallengePlayCardSuccess(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeEndTurn request success.");
    }

	private void OnChallengePlayCardError(LogChallengeEventResponse response)
    {
        Debug.LogError("ChallengeEndTurn request error.");
    }
}
