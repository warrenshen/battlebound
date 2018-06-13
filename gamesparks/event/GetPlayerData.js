// ====================================================================================================
//
// Cloud Code for GetPlayerData, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("OnChainModule");

const player = Spark.getPlayer();

const address = player.getPrivateData("address");
var balance = null;
if (address) {
    balance = fetchBalanceByAddress(address);
}

Spark.setScriptData("address", address);
Spark.setScriptData("balance", balance);
Spark.setScriptData("winStreak", player.getPrivateData("winStreak"));
Spark.setScriptData("level", player.getPrivateData("level"));
Spark.setScriptData("responseCode", 200);
