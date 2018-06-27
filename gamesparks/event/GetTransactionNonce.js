// ====================================================================================================
//
// Cloud Code for GetTransactionNonce, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const player = Spark.getPlayer();
const address = player.getPrivateData("address");

if (!address) {
    setScriptError("Player does not have an address.");
}

const nonce = fetchNonceByAddress(address);

Spark.setScriptData("nonce", nonce);
setScriptSuccess();
