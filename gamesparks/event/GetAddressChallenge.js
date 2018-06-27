// ====================================================================================================
//
// Cloud Code for GetAddressChallenge, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AddressModule");

const addressChallenge = resetPlayerAddressChallenge();

Spark.setScriptData("addressChallenge", addressChallenge);
setScriptSuccess();
