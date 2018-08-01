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
        LeaderboardDataRequest request = new LeaderboardDataRequest();
        request.SetEntryCount(100);
        request.SetLeaderboardShortCode("HIGH_SCORE_LB");
        request.Send(OnGetLeaderboardSuccess, OnGetLeaderboardError);
    }

    private void OnGetLeaderboardSuccess(LeaderboardDataResponse response)
    {
        Debug.Log("FindMatch request success.");
    }

    private void OnGetLeaderboardError(LeaderboardDataResponse response)
    {
        Debug.LogError("FindMatch request error.");
    }
}
