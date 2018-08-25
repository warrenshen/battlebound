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
const TREASURY_CONTRACT_ADDRESS = "0x7525106192a90D039Bfe79b39dAFF86123C5850b";
const AUCTION_CONTRACT_ADDRESS = "0x673188286d4da734a23738cad9339e5fc3bb4947";
const LOOT_BOX_CONTRACT_ADDRESS = "0xbca850e75f5e5dc02Dcb345461bE392af906F7C3";

const BLOCKCHAIN_EVENT_AUCTION_CREATED = "BLOCKCHAIN_EVENT_AUCTION_CREATED";
const BLOCKCHAIN_EVENT_AUCTION_CANCELED = "BLOCKCHAIN_EVENT_AUCTION_CANCELED";
const BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL = "BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL";

function fetchStatusByTransactionHash(txHash, latestBlockNumber) {
    const formattedTxHash = prefixHex(txHash);
    const json = {
        jsonrpc: "2.0",
        method: "eth_getTransactionReceipt",
        id: 1,
        params: [
            formattedTxHash
        ]
    };

    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseJson.error) {
        setScriptError("JSON RPC request error: " + responseJson.error);
    } else if (responseCode === 200) {
        const result = responseJson.result;
        const z = "z";
        
        if (result == null) {
            return 2;
        } else if (result.status === "0x0") {
            return 0;
        } else if (result.status === "0x1") {
            const txBlockNumber = result.blockNumber;
            if (parseInt(latestBlockNumber, 16)  - parseInt(txBlockNumber, 16) > 4) {
                return 1;
            } else {
                return 2;
            }
        } else {
            return 2;
        }
    } else {
        setScriptError("JSON RPC request error - recoverAddressBySignature.");
    }
}

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
                "to": RECOVER_CONTRACT_ADDRESS
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
        setScriptError("JSON RPC request error - recoverAddressBySignature.");
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
        setScriptError("JSON RPC request error - fetchBalanceByAddress.");
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
        setScriptError("JSON RPC request error - fetchNonceByAddress.");
    }
}

function fetchTemplateIntByCardInt(cardInt) {
    if (!Number.isInteger(cardInt)) {
        setScriptError("Invalid cardInt parameter.");
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
        setScriptError("JSON RPC request error - fetchTemplateIntByCardInt.");
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
        setScriptError("JSON RPC request error - fetchCardIdsByAddress.");
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
        setScriptError("JSON RPC request error - fetchLatestBlockNumber.");
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
function _parseLogsAuction(event, rawLogs) {
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
            setScriptError("Invalid event.");
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
        const logs = _parseLogsAuction(event, responseJson.result);
        const z = "z";
        return logs;
    } else {
        setScriptError("JSON RPC request error - fetchLogsByBlockRange.");
    }
}

function _parseLogsLootBox(rawLogs) {
    return rawLogs.map(function(rawLog) {
        const topics = rawLog.topics;
        const recipientAddress = parseAddress(topics[1]);
        const referrerAddress = parseAddress(topics[2]);
        const category = convertHexToIntFixedLength(topics[3]);
        const quantity = convertHexToIntFixedLength(rawLog.data);
        
        return {
            recipientAddress: recipientAddress,
            referrerAddress: referrerAddress,
            category: category,
            quantity: quantity,
            blockNumber: rawLog.blockNumber,
            transactionHash: rawLog.transactionHash,
            transactionIndex: rawLog.transactionIndex,
            logIndex: rawLog.logIndex,
        };
    });
}

function fetchLogsByBlockRangeLootBox(fromBlock, toBlock) {
    const json = {
        jsonrpc: "2.0",
        method: "eth_getLogs",
        id: 74,
        params: [
            {
                "address": LOOT_BOX_CONTRACT_ADDRESS,
                "fromBlock": fromBlock,
                "toBlock": toBlock,
                "topics": ["0xf20405de859a7188253a3ee9359a07873bc40a87b78b50a37767e4c1929c6427"],
            }
        ],
    };
    
    const jsonString = JSON.stringify(json);
    const response = Spark.getHttp(INFURA_URL).postString(jsonString);
    const responseCode = response.getResponseCode();
    const responseJson = response.getResponseJson();
    
    if (responseJson.error) {
        setScriptError("JSON RPC request error: " + responseJson.error);
    } else if (responseCode === 200) {
        const logs = _parseLogsLootBox(responseJson.result);
        const z = "z";
        return logs;
    } else {
        setScriptError("JSON RPC request error - fetchLogsByBlockRangeLootBox.");
    }
}
        
function fetchAuctionByCardInt(cardInt) {
    if (!Number.isInteger(cardInt)) {
        setScriptError("Invalid cardInt parameter.");
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
        setScriptError("JSON RPC request error - fetchAuctionByCardInt.");
    }
}

function fetchAuctionCurrentPriceByTokenId(cardInt) {
    if (!Number.isInteger(cardInt)) {
        setScriptError("Invalid cardInt parameter.");
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
        setScriptError("ERROR", "JSON RPC request error - fetchAuctionCurrentPriceByTokenId.");
    }
}

/**
 * @param string signedTx - signed transaction
 * @return string - transaction hash of submitted transaction
 **/
function submitSignedTransaction(signedTx) {
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
            Spark.getLog().error({
                errorMessage: "Submit signed transaction error: " + error.message,
                json: json,
            });
            return null;
        }
        
        return responseJson.result;
    } else {
        setScriptError("JSON RPC request error - submitCreateAuctionTransaction.");
    }
}

// TODO: probably don't need all these submit functions can share one.
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
            setScriptError("Error on submit transaction: " + error.message);
        }
        
        return responseJson.result;
    } else {
        setScriptError("JSON RPC request error - submitCreateAuctionTransaction.");
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
            setScriptError("Error on submit transaction: " + error.message);
        }
        
        return responseJson.result;
    } else {
        setScriptError("JSON RPC request error - submitBidAuctionTransaction.");
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
            setScriptError("Error on submit transaction: " + error.message);
        }
        
        return responseJson.result;
    } else {
        setScriptError("JSON RPC request error - submitCancelAuctionTransaction.");
    }
}
