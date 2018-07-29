using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class LeaderboardManager : MonoBehaviour
{

    public void Awake()
    {

    }

    private void SendGetLeaderboardRequest()
    {

    }

    private void OnGetLeaderboardSuccess(LogEventResponse response)
    {
        Debug.Log("FindMatch request success.");
    }

    private void OnGetLeaderboardError(LogEventResponse response)
    {
        Debug.LogError("FindMatch request error.");
    }
}
