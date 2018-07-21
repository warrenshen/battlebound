// ====================================================================================================
//
// This module sends ChallengeTimeRunningOutMessage messages to the two players
// of a challenge when it is invoked by the Spark scheduler.
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeMovesModule");

const category = Spark.getData().category;
const challengeId = Spark.getData().challengeId;

const message = Spark.message("ChallengeTimeRunningOutMessage");
const data = {};

if (category === TIME_LIMIT_CATEGORY_NORMAL)
{
    const playerId = Spark.getPlayer().playerId;
    
    const hasTurnPlayerId = Spark.getData().hasTurnPlayerId;
    const opponentId = Spark.getData().opponentId;
    
    data.category = category;
    data.challengeId = challengeId;
    data.hasTurnPlayerId = hasTurnPlayerId;
    
    message.setMessageData(data);
    message.setPlayerIds([playerId, opponentId]);
    message.send();
} else if (category === TIME_LIMIT_CATEGORY_MULLIGAN) {
    const playerIds = Spark.getData().playerIds;
    
    data.category = category;
    data.challengeId = challengeId;
    
    message.setMessageData(data);
    message.setPlayerIds(playerIds);
    message.send();
} else {
    setScriptError("Unknown time running out category.");
}
