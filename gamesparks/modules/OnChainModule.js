// ====================================================================================================
//
// Cloud Code for OnChainModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AddressModule");

const INFURA_URL = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";
// const CONTRACT_ADDRESS = "0x5Cd6BaEF2B5bd80CD699A702649591DC1fE6bf33";
const CONTRACT_ADDRESS = "0x60403c022a2661d8218e48892493f5393d985fc4";

function fetchTemplateIdByCardId(cardId) {
    const formattedCardId = padParameter(cardId);
    const data = "0xdf84807a" + formattedCardId;
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
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        return convertHexToInt(responseJson.result);
    } else {
        return 'Error';
    }
}

function fetchCardIdsByAddress(address) {
    const formattedAddress = padParameter(cleanAddress(address));
    const data = "0x8462151c" + formattedAddress;
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
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        return convertHexToInt(responseJson.result);
    } else {
        return 'Error';
    }
}
