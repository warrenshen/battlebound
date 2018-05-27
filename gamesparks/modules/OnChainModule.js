// ====================================================================================================
//
// Cloud Code for OnChainModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AddressModule");

const INFURA_URL = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";
const CONTRACT_ADDRESS = "0x5Cd6BaEF2B5bd80CD699A702649591DC1fE6bf33";

// {
//       "to": "0x06012c8cf97BEaD5deAe237070F9587f8E7A266d",
//       "data": "0x6352211e000000000000000000000000000000000000000000000000000000000000000a"
//     },

function fetchCardsOnChainByAddress(address) {
    var formattedAddress = "000000000000000000000000" + cleanAddress(address);
    var data = "0x8462151c" + formattedAddress;
    
    var json = {
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
    var jsonString = JSON.stringify(json);
    var response = Spark.getHttp(INFURA_URL).postString(jsonString);

    var responseCode = response.getResponseCode();
    var responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        var cards = convertHexToInt(responseJson.result);
        return cards;
    } else {
        return 'Error';
    }
}
