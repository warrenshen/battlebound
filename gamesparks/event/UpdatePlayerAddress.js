// ====================================================================================================
//
// Custom event to update a player's ethereum address.
//
// ====================================================================================================
require("AddressModule");

const INFURA_URL = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";
const CONTRACT_ADDRESS = "0xF17e2999b7eF0F42E454c5093ECC54d6e2d5c134";

const address = Spark.getData().address;
const signature = Spark.getData().signature;

const player = Spark.getPlayer();
const h = player.getPrivateData("addressChallenge");

const cleanSignature = cleanHex(signature);
const r = padParameter(cleanSignature.slice(0, 64));
const s = padParameter(cleanSignature.slice(64, 128));
const v = padParameter(cleanSignature.slice(128, 130));
const data = "0x8428cf83" + h + v + r + s;
    
const json = {
    jsonrpc: "2.0",
    method: "eth_call",
    id: 1,
    params: [
        {
            "data": data,
            "to": CONTRACT_ADDRESS
        },
        "latest"
    ]
};
const jsonString = JSON.stringify(json);
const response = Spark.getHttp(INFURA_URL).postString(jsonString);

const responseCode = response.getResponseCode();

if (responseCode === 200) {
    const responseJson = response.getResponseJson();
    const result = responseJson.result;
    const recoveredAddress = cleanHex(result).substring(24);
    
    // If address player says it owns matches the recovered address,
    // then the player must have the private key (and own the address).
    if (cleanHex(address).toLowerCase() === cleanHex(recoveredAddress).toLowerCase()) {
        // Set player's private data accordingly.
        player.setPrivateData("address", address);
        
        Spark.setScriptData("address", address);
        Spark.setScriptData("statusCode", 200);
    } else {
        Spark.setScriptError("ERROR", "Invalid address ownership proof.");
        Spark.exit();
    }
} else {
    Spark.setScriptError("ERROR", "JSON RPC call failed.");
    Spark.exit();
}
