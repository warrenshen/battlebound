// ====================================================================================================
//
// This module contains inline code to generate challenge user message
// response to the players. It fetches the ChallengeState associated
// with the challenge ID and sets two scriptData fields:
// "playerState" and "opponentState".
// 
// Note this is in-line with the design pattern where "global messages"
// change the ChallengeState and "user messages" just read from it.
//
// ====================================================================================================
require("ChallengeStateModule");

const playerId = Spark.getPlayer().getPlayerId();
const challengeId = Spark.getData().challenge.challengeId;

getChallengeStateForPlayer(playerId, challengeId);
