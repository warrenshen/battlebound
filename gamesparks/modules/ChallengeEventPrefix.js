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
const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeInstanceId;
const challenge = Spark.getChallenge(challengeId);

const challengeStateData = challenge.getPrivateData("data");
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
