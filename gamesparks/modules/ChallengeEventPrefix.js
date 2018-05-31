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
const API = Spark.getGameDataService();

const player = Spark.getPlayer()
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeInstanceId;
const challenge = Spark.getChallenge(challengeId);

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "Challenge state does not exist.");
    Spark.exit();
}

const challengeStateData = challengeStateDataItem.getData();
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
