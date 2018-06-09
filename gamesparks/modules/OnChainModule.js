// ====================================================================================================
//
// Cloud Code for OnChainModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AddressModule");

const INFURA_URL = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";
const TREASURY_CONTRACT_ADDRESS = "0x60403c022a2661d8218e48892493f5393d985fc4";
const AUCTION_CONTRACT_ADDRESS = "0xEFa6737A7439BFAee477C471241822dAd3558CB2";

const BLOCKCHAIN_EVENT_AUCTION_CREATED = "BLOCKCHAIN_EVENT_AUCTION_CREATED";
const BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL = "BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL";

function fetchTemplateIntByCardInt(cardInt) {
    if (!Number.isInteger(cardInt)) {
        Spark.setScriptError("ERROR", "Invalid cardInt parameter.");
        Spark.exit();
    }
    const formattedCardId = padParameter(cardInt);
    const data = "0xdf84807a" + formattedCardId;
    const json = {
        jsonrpc: "2.0",
        method: "eth_call",
        id: 1,
        params: [
            {
                "data": data,
                "to": TREASURY_CONTRACT_ADDRESS,
            },
            "latest"
        ],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        return convertHexToIntFixedLength(responseJson.result);
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
                "to": TREASURY_CONTRACT_ADDRESS,
            },
            "latest"
        ],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        return convertHexToIntVariableLength(responseJson.result);
    } else {
        return 'Error';
    }
}

/**
 * @return string - latest block number in hex
 **/
function fetchLatestBlockNumber() {
    const json = {
        jsonrpc: "2.0",
        method: "eth_blockNumber",
        id: 83,
        params: [],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        return responseJson.result;
    } else {
        return 'Error';
    }
}

function _getTopicForEvent(event) {
    switch (event) {
        case BLOCKCHAIN_EVENT_AUCTION_CREATED:
            return "0xa9c8dfcda5664a5a124c713e386da27de87432d5b668e79458501eb296389ba7";
        case BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL:
            return "0x4fcc30d90a842164dd58501ab874a101a3749c3d4747139cefe7c876f4ccebd2";
        default:
            Spark.setScriptError("ERROR", "Invalid event.");
            Spark.exit();
    }
}

/**
 * @param rawLogs - array of unparsed logs from JSON RPC call (example below)
 * [
 *   {
 *     "address": "0xefa6737a7439bfaee477c471241822dad3558cb2",
 *     "topics": [
 *       "0xa9c8dfcda5664a5a124c713e386da27de87432d5b668e79458501eb296389ba7"
 *      ],
 *      "data": "0x0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000a000000000000000000000000000000000000000000000000000000000000000a0000000000000000000000000000000000000000000000000000000000000e10",
 *      "blockNumber": "0x24e810",
 *      "transactionHash": "0x88c8373e9b327ea907aa1367d149c8ed60d93197ca7730daca5e1f3ad87d08d1",
 *      "transactionIndex": "0x10",
 *      "blockHash": "0x71e0caf752574fcedcc2852b275e6a835d41b6714d37181ce185585c7d3ee2c4",
 *      "logIndex": "0x1f",
 *      "removed": false
 *    }
 * ]
 **/
function _parseLogs(event, rawLogs) {
    return rawLogs.map(function(rawLog) {
        const dataInts = convertHexToIntFixedLength(rawLog.data);
        
        if (event === BLOCKCHAIN_EVENT_AUCTION_CREATED) {
            return {
                event: event,
                tokenId: dataInts[0],
                // data: {
                //     tokenId: dataInts[0],
                //     startingPrice: dataInts[1],
                //     endingPrice: dataInts[2],
                //     duration: dataInts[3],
                // },
                blockNumber: rawLog.blockNumber,
                transactionHash: rawLog.transactionHash,
                transactionIndex: rawLog.transactionIndex,
                logIndex: rawLog.logIndex,
            };
        } else if (event === BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL) {
            return {
                event: event,
                tokenId: dataInts[0],
                // data: {
                //     tokenId: dataInts[0],
                // },
                blockNumber: rawLog.blockNumber,
                transactionHash: rawLog.transactionHash,
                transactionIndex: rawLog.transactionIndex,
                logIndex: rawLog.logIndex,
            };
        } else {
            Spark.setScriptError("ERROR", "Invalid event.");
            Spark.exit();
        }
    });
}

function fetchLogsByBlockRange(event, fromBlock, toBlock) {
    const json = {
        jsonrpc: "2.0",
        method: "eth_getLogs",
        id: 74,
        params: [
            {
                "address": AUCTION_CONTRACT_ADDRESS,
                "fromBlock": fromBlock,
                "toBlock": toBlock,
                "topics": [_getTopicForEvent(event)],
            }
        ],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const logs = _parseLogs(event, responseJson.result);
        const z = "z";
        return logs;
    } else {
        return 'Error';
    }
}
        
function fetchAuctionByCardInt(cardInt) {
    if (!Number.isInteger(cardInt)) {
        Spark.setScriptError("ERROR", "Invalid cardInt parameter.");
        Spark.exit();
    }
    const formattedCardId = padParameter(cardInt);
    const data = "0x78bd7935" + formattedCardId;
    const json = {
        jsonrpc: "2.0",
        method: "eth_call",
        id: 1,
        params: [
            {
                "data": data,
                "to": AUCTION_CONTRACT_ADDRESS,
            },
            "latest"
        ],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const result = responseJson.result;
        // The first 32-bit word argument in the result contains the seller's address.
        // We use a helper method to parse it out and get a "0x"-prefixed address.
        const seller = parseAddress(result.substring(0, 66));
        const dataInts = convertHexToIntFixedLength(result);
        return {
            cardInt: cardInt,
            seller: seller,
            startingPrice: dataInts[1],
            endingPrice: dataInts[2],
            duration: dataInts[3],
            startedAt: dataInts[4],
        };
    } else {
        return 'Error';
    }
}
