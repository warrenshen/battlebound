// ====================================================================================================
//
// Cloud Code for ProcessRankModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function processRank(winnerPlayerId, loserPlayerId) {
    const winnerRankScore = getLeaderboardsScore(winnerPlayerId);
    const loserRankScore = getLeaderboardsScore(loserPlayerId);
    
    const newWinnerRankScore = computeRank(winnerRankScore, loserRankScore, 1);
    const newLoserRankScore = computeRank(loserRankScore, winnerRankScore, 0);
    
    var results = {}
    results[winnerPlayerId] = newWinnerRankScore;
    results[loserPlayerId] = newLoserRankScore;
    return results;
}

function computeRank(rankScore1, rankScore2, score) {
    var q1 = Math.pow(10, rankScore1 / 400);
    var q2 = Math.pow(10, rankScore2 / 400);
    var e = q1 / (q1 + q2);
    
    return rankScore1 + 40 * (score - e);
}

function getLeaderboardsScore(playerId) {
    var request = new SparkRequests.GetLeaderboardEntriesRequest();
    request.leaderboards = ["HIGH_SCORE_LB"];
    request.player = playerId;
    var response = request.Send();
    return response.HIGH_SCORE_LB.SCORE_ATTR;
}

function setLeaderboardsScore(score) {
    var request = new SparkRequests.LogEventRequest();
    request.eventKey = "SCORE_EVENT";
    request.SCORE_ATTR = score;
    var response = request.Send();
}

