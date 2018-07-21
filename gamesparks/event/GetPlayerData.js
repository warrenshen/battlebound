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

const address = player.getPrivateData("address");
var balance = 0;
if (address) {
    balance = fetchBalanceByAddress(address);
}

Spark.setScriptData("playerId", player.getPlayerId());
Spark.setScriptData("address", address);
Spark.setScriptData("balance", balance);
Spark.setScriptData("activeChallengeId", player.getPrivateData("activeChallengeId"));
Spark.setScriptData("winStreak", player.getPrivateData("winStreak"));
Spark.setScriptData("level", player.getPrivateData("level"));
setScriptSuccess();
