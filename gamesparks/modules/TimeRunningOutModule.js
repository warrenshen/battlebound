// ====================================================================================================
//
// This module sends ChallengeTimeRunningOutMessage messages to the two players
// of a challenge when it is invoked by the Spark scheduler.
//
// ====================================================================================================
const challengeId = Spark.getData().challengeId;
const hasTurnPlayerId = Spark.getData().hasTurnPlayerId;
const opponentId = Spark.getData().opponentId;

const playerId = Spark.getPlayer().playerId;

const message = Spark.message("ChallengeTimeRunningOutMessage");

const data = {};
data.challengeId = challengeId;
data.hasTurnPlayerId = hasTurnPlayerId;

message.setMessageData(data);
message.setPlayerIds([playerId, opponentId]);
message.send();
