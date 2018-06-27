// ====================================================================================================
//
// Cloud Code for RegistrationResponse, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("InitializePlayerModule");

if (!Spark.hasScriptErrors() && Spark.getPlayer() !== null) {
    initializePlayer();
    setScriptSuccess();
} else {
    setScriptError("Something went wrong with registration.");
}
