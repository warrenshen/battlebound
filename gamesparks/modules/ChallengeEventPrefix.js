// ====================================================================================================
//
// This module contains inline code for "challenge events" that fetches the ChallengeState associated
// with the challenge ID. It defines some helpful variables, namely the following:
// - challengeStateDataItem
// - challengeStateData
// - challenge
// - challengeId
// - playerId
// - opponentId
//
// Note that "challenge events" and "global/user messages" are NOT the same. This is for the former.
//
// ====================================================================================================
var API = Spark.getGameDataService();

var playerId = Spark.getPlayer().getPlayerId();
var challengeId = Spark.getData().challengeInstanceId;
var challenge = Spark.getChallenge(challengeId);

var challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "Challenge state does not exist.");
    Spark.exit();
}

var challengeStateData = challengeStateDataItem.getData();
var opponentId = challengeStateData.opponentIdByPlayerId[playerId];
