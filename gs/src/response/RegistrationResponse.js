// ====================================================================================================
//
// Cloud Code for RegistrationResponse, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("InitializePlayerModule");

const player = Spark.getPlayer();

if (Spark.hasScriptErrors() || player == null) {
    setScriptError("Something went wrong with registration.");
}

initializePlayerByPlayerId(player.getPlayerId());
setScriptSuccess();
