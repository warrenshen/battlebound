// ====================================================================================================
//
// Cloud Code for OnChainModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AddressModule");

const INFURA_URL = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";

const RECOVER_CONTRACT_ADDRESS = "0xF17e2999b7eF0F42E454c5093ECC54d6e2d5c134";
const TREASURY_CONTRACT_ADDRESS = "0xed0f506bc7f9738d0d607e75b612239fe1f405f6";
const AUCTION_CONTRACT_ADDRESS = "0xEFa6737A7439BFAee477C471241822dAd3558CB2";

const BLOCKCHAIN_EVENT_AUCTION_CREATED = "BLOCKCHAIN_EVENT_AUCTION_CREATED";
const BLOCKCHAIN_EVENT_AUCTION_CANCELED = "BLOCKCHAIN_EVENT_AUCTION_CANCELED";
const BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL = "BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL";

function recoverAddressBySignature(challenge, signature) {
    const cleanSignature = cleanHex(signature);
    const h = padParameter(challenge);
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
        return recoveredAddress;
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

function fetchBalanceByAddress(address) {
    const formattedAddress = padParameter(cleanAddress(address));
    const json = {
        jsonrpc: "2.0",
        method: "eth_getBalance",
        id: 1,
        params: [address, "latest"],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const z = 'z';
        return parseInt(responseJson.result, 16);
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

function fetchNonceByAddress(address) {
    const json = {
        jsonrpc: "2.0",
        method: "eth_getTransactionCount",
        id: 1,
        params: [address, "latest"],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const z = 'z';
        return parseInt(responseJson.result, 16);
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

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
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
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
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
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
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

function _getTopicForEvent(event) {
    switch (event) {
        case BLOCKCHAIN_EVENT_AUCTION_CREATED:
            return "0xa9c8dfcda5664a5a124c713e386da27de87432d5b668e79458501eb296389ba7";
        case BLOCKCHAIN_EVENT_AUCTION_CANCELED:
            return "0x2809c7e17bf978fbc7194c0a694b638c4215e9140cacc6c38ca36010b45697df";
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
        const data = convertHexToIntFixedLength(rawLog.data);
        
        if (event === BLOCKCHAIN_EVENT_AUCTION_CREATED) {
            return {
                event: event,
                tokenId: data[0], // data is an array with token ID as first element.
                blockNumber: rawLog.blockNumber,
                transactionHash: rawLog.transactionHash,
                transactionIndex: rawLog.transactionIndex,
                logIndex: rawLog.logIndex,
            };
        } else if (event === BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL) {
            return {
                event: event,
                tokenId: data[0], // data is an array with token ID as first element.
                blockNumber: rawLog.blockNumber,
                transactionHash: rawLog.transactionHash,
                transactionIndex: rawLog.transactionIndex,
                logIndex: rawLog.logIndex,
            };
        } else if (event === BLOCKCHAIN_EVENT_AUCTION_CANCELED) {
            return {
                event: event,
                tokenId: data, // data is the token ID.
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
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}
        
function fetchAuctionByCardInt(cardInt) {
    if (!Number.isInteger(cardInt)) {
        Spark.setScriptError("ERROR", "Invalid cardInt parameter.");
        Spark.exit();
    }
    const formattedTokenId = padParameter(cardInt);
    const data = "0x78bd7935" + formattedTokenId;
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
            seller: seller.toLowerCase(),
            startingPrice: dataInts[1],
            endingPrice: dataInts[2],
            duration: dataInts[3],
            startedAt: dataInts[4],
        };
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

function fetchAuctionCurrentPriceByTokenId(cardInt) {
    if (!Number.isInteger(cardInt)) {
        Spark.setScriptError("ERROR", "Invalid cardInt parameter.");
        Spark.exit();
    }
    const formattedTokenId = padParameter(cardInt);
    const data = "0xc55d0f56" + formattedTokenId;
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
        return convertHexToIntFixedLength(result);
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

/**
 * @param string signedTx - signed transaction
 * @return string - transaction hash of submitted transaction
 **/
function submitCreateAuctionTransaction(signedTx) {
    const formattedSignedTx = prefixHex(signedTx);
    const json = {
        jsonrpc: "2.0",
        method: "eth_sendRawTransaction",
        id: 1,
        params: [formattedSignedTx],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const error = responseJson.error;
        if (error && error.code) {
            Spark.setScriptError("ERROR", "Insufficient funds to submit transaction.");
            Spark.exit();
        }
        
        return responseJson.result;
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

/**
 * @param string signedTx - signed transaction
 * @return string - transaction hash of submitted transaction
 **/
function submitBidAuctionTransaction(signedTx) {
    const formattedSignedTx = prefixHex(signedTx);
    const json = {
        jsonrpc: "2.0",
        method: "eth_sendRawTransaction",
        id: 1,
        params: [formattedSignedTx],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const error = responseJson.error;
        if (error && error.code) {
            Spark.setScriptError("ERROR", "Insufficient funds to submit transaction.");
            Spark.exit();
        }
        
        return responseJson.result;
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}

/**
 * @param string signedTx - signed transaction
 * @return string - transaction hash of submitted transaction
 **/
function submitCancelAuctionTransaction(signedTx) {
    const formattedSignedTx = prefixHex(signedTx);
    const json = {
        jsonrpc: "2.0",
        method: "eth_sendRawTransaction",
        id: 1,
        params: [formattedSignedTx],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseCode === 200) {
        const error = responseJson.error;
        if (error && error.code) {
            Spark.setScriptError("ERROR", "Insufficient funds to submit transaction.");
            Spark.exit();
        }
        
        return responseJson.result;
    } else {
        Spark.setScriptError("ERROR", "JSON RPC request error.");
        Spark.exit();
    }
}
