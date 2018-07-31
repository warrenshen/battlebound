// ====================================================================================================
//
// Cloud Code for GetPlayerData, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const address = player.getPrivateData("address");
var balance = 0;
if (address) {
    balance = fetchBalanceByAddress(address);
}

const scoreRequest = new SparkRequests.GetLeaderboardEntriesRequest();
scoreRequest.leaderboards = ["HIGH_SCORE_LB"];
scoreRequest.player = playerId;
const scoreResponse = scoreRequest.Send();
const playerRank = scoreResponse.HIGH_SCORE_LB.SCORE_ATTR;
    
Spark.setScriptData("playerId", playerId);
Spark.setScriptData("displayName", player.getDisplayName());
Spark.setScriptData("address", address);
Spark.setScriptData("balance", balance);
Spark.setScriptData("activeChallengeId", player.getPrivateData("activeChallengeId"));
Spark.setScriptData("infoByMatchType", player.getPrivateData("infoByMatchType"));
Spark.setScriptData("level", player.getPrivateData("level"));
Spark.setScriptData("rank", playerRank);

setScriptSuccess();
