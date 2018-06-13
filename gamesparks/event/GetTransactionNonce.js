// ====================================================================================================
//
// Cloud Code for GetTransactionNonce, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("OnChainModule");

const player = Spark.getPlayer();
const address = player.getPrivateData("address");

if (!address) {
    Spark.setScriptError("ERROR", "Player does not have an address.");
    Spark.exit(); 
}

const nonce = fetchNonceByAddress(address);

Spark.setScriptData("nonce", nonce);
