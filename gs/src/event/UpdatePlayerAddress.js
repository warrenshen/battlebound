// ====================================================================================================
//
// Custom event to update a player's ethereum address.
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const address = Spark.getData().address;
const signature = Spark.getData().signature;

const player = Spark.getPlayer();
const challenge = player.getPrivateData("addressChallenge");

const recoveredAddress = recoverAddressBySignature(challenge, signature);

// If address player says it owns matches the recovered address,
// then the player must have the private key (and own the address).
if (cleanHex(address).toLowerCase() !== cleanHex(recoveredAddress).toLowerCase()) {
    setScriptError("Invalid address ownership proof.");
}

// Set player's private data accordingly.
player.setPrivateData("address", address);

Spark.setScriptData("address", address);
setScriptSuccess();
