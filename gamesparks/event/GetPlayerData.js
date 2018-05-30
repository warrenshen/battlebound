// ====================================================================================================
//
// Cloud Code for GetPlayerData, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const player = Spark.getPlayer();

Spark.setScriptData("address", player.getPrivateData("address"));
Spark.setScriptData("winStreak", player.getPrivateData("winStreak"));
Spark.setScriptData("level", player.getPrivateData("level"));
// Spark.setScriptData("name", value)
Spark.setScriptData("responseCode", 200);
