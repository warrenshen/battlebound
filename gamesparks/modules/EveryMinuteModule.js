// ====================================================================================================
//
// Cloud Code for EveryMinuteModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AuctionModule");

const LOCK_EVERY_MINUTE_MODULE = "LOCK_EVERY_MINUTE_MODULE";
Spark.lockKey(LOCK_EVERY_MINUTE_MODULE, 5000);

// const TEMPLATE_ID_TO_LIMIT = {
//     0: 88888,
//     1: 88888,
//     2: 88888,
//     3: 88888,
//     4: 88888,
//     5: 88888,
//     6: 88888,
//     7: 88888,
//     8: 88888,
//     9: 88888,
//     10: 88888,
//     11: 88888,
//     12: 88888,
//     13: 88888,
//     14: 88888,
//     15: 88888,
//     16: 88888,
//     17: 88888,
//     18: 88888,
//     19: 88888,
//     20: 88888,
//     21: 88888,
//     22: 88888,
//     23: 88888,
//     24: 88888,
//     25: 88888,
//     26: 88888,
//     27: 88888,
//     28: 88888,
//     29: 88888,
//     30: 88888,
//     31: 88888,
//     32: 88888,
//     33: 88888,
//     34: 88888,
//     35: 14632,
//     36: 14632,
//     37: 14632,
//     38: 14632,
//     39: 14632,
//     40: 14632,
//     41: 14632,
//     42: 14632,
//     43: 14632,
//     44: 14632,
//     45: 14632,
//     46: 39905,
//     47: 39905,
//     48: 39905,
//     49: 39905,
//     50: 39905,
//     51: 39905,
//     52: 39905,
//     53: 39905,
//     54: 39905,
//     55: 39905,
//     56: 39905,
//     57: 39905,
//     58: 39905,
//     59: 39905,
//     60: 39905,
//     61: 39905,
//     62: 39905,
//     63: 39905,
//     64: 39905,
//     65: 39905,
//     66: 39905,
//     67: 39905,
//     68: 3737,
//     69: 3737,
//     70: 30,
// };

const TEMPLATE_ID_TO_LIMIT = {
    0: 0,
    1: 0,
    2: 100,
    3: 1000,
    4: 1000,
    5: 10,
};

const BLOCK_CONFIRMATIONS_COUNT = 4;
const latestBlockNumber = fetchLatestBlockNumber();
const toBlock = "0x" + (parseInt(latestBlockNumber, 16) - BLOCK_CONFIRMATIONS_COUNT).toString(16);

const API = Spark.getGameDataService();
const masterDataItem = API.getItem("Master", "master").document();
if (masterDataItem === null) {
    setScriptError("Master does not exist.");
}
const master = masterDataItem.getData();

if (!master.auctions) {
    master.auctions = [];
}

if (!master.blockNumber) {
    master.blockNumber = "0x24E810";
}

if (!master.transactionQueue) {
    master.transactionLast = null; // Transaction hash of last transaction sent to the network.
    /*
     * Transaction schema: {
     *   type: string, // Type of transaction.
     *   data: Object, // Object of data for transaction.
     * }
     */
    master.transactionQueue = []; // Queue of Transaction objects waiting to be sent to network.
}

if (!master.templateIdToCount) {
    master.templateIdToCount = {};
}

var auctions = master.auctions;
var transactionQueue = master.transactionQueue;
var transactionLast = master.transactionLast;
const fromBlock = master.blockNumber;
const templateIdToCount = master.templateIdToCount;


// ====================================================================================================
// AUCTION POLLING LOGIC START
// ====================================================================================================
const auctionCreatedLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_CREATED, fromBlock, toBlock);
const auctionCanceledLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_CANCELED, fromBlock, toBlock);
const auctionSuccessfulLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL, fromBlock, toBlock);

// const auctionCreatedLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_CREATED, "0x24E810", toBlock);
// const auctionCanceledLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_CANCELED, "0x24E810", toBlock);
// const auctionSuccessfulLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL, "0x24E810", toBlock);

const auctionLogs = auctionCreatedLogs.concat(auctionCanceledLogs.concat(auctionSuccessfulLogs));
// Sort auction logs into "chronological" order.
auctionLogs.sort(function(logA, logB) {
    const blockNumberA = parseInt(logA.blockNumber, 16);
    const blockNumberB = parseInt(logB.blockNumber, 16);
    if (blockNumberA < blockNumberB) {
        return -1;
    } else if (blockNumberA > blockNumberB) {
        return 1;
    } else {
        const transactionIndexA = parseInt(logA.transactionIndex, 16);
        const transactionIndexB = parseInt(logB.transactionIndex, 16);
        return transactionIndexA < transactionIndexB ? -1 : 1;
    }
});

auctionLogs.forEach(function(log) {
    const event = log.event;
    const tokenId = log.tokenId;
    
    const auctionIndex = auctions.indexOf(tokenId);
    
    if (event === BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL || event === BLOCKCHAIN_EVENT_AUCTION_CANCELED) {
        if (auctionIndex >= 0) {
            auctions = auctions.slice(0, auctionIndex).concat(auctions.slice(auctionIndex + 1));
            removeAuctionByTokenId(tokenId);
        }
    } else if (event === BLOCKCHAIN_EVENT_AUCTION_CREATED) {
        if (auctionIndex < 0) {
            auctions = auctions.concat([tokenId]);
            syncAuctionByTokenId(tokenId);
        }
    } else {
        setScriptError("Invalid event.");
    }
});

master.auctions = auctions;
// ====================================================================================================
// AUCTION POLLING LOGIC END
// ====================================================================================================


// ====================================================================================================
// LOOT BOX LOGIC START
// ====================================================================================================
const MINTER_ADDRESS = "0xe598350d4E3c223fCa03bbA55fb9C276576fA4ae";
const MINTER_PRIVATE_KEY = "0x10EB93083B4F2BD8CE2FBBEE18676C6DB1EBF778DDAF8CF7255237AFE8D61C60";

const lootBoxLogs = fetchLogsByBlockRangeLootBox(fromBlock, toBlock);
// const lootBoxLogs = fetchLogsByBlockRangeLootBox("0x2BC401", toBlock);
lootBoxLogs.forEach(function(log) {
    Spark.getLog().debug({
        message: "Found loot box purchase transaction.",
        transactionHash: log.transactionHash,
        quantity: log.quantity,
        recipientAddress: log.recipientAddress,
    });
    transactionQueue = transactionQueue.concat([{
        type: "TRANSACTION_TYPE_LOOT_BOX",
        data: {
            quantity: log.quantity,
            recipientAddress: log.recipientAddress,
        },
    }]);
});

// transactionLast = "0xf4865387dbfbfc0551b21057b418f232a7e92c77e9ea8e2e1485af65f4a5eeeb";
if (transactionLast) {
    const txStatus = fetchStatusByTransactionHash(transactionLast, latestBlockNumber);
    if (txStatus === 1) {
        // Transaction successfully went through.
        master.transactionLast = null;
        transactionLast = null;
    } else if (txStatus === 0) {
        // Transaction failed - log and allow next one.
        Spark.getLog().error({
            errorMessage: "Found failed transaction.",
            txHash: transactionLast,
            latestBlockNumber: latestBlockNumber,
        });
        master.transactionLast = null;
        transactionLast = null;
    }
}

if (!transactionLast && transactionQueue.length > 0) {
    const txElement = transactionQueue[0];
    const signedTx = fetchSignedTransaction(txElement);
    if (signedTx) {
        const txHash = submitSignedTransaction(signedTx);
        if (txHash) {
            transactionLast = txHash;
            master.transactionLast = transactionLast;
            transactionQueue.shift();
            
            Spark.getLog().debug({
                message: "Submit mint cards tx success.",
                transactionHash: txHash,
            });
        }
    }
}

function mintRandomCard() {
    var totalCardsRemaining = 0;
    const templateIds = Object.keys(TEMPLATE_ID_TO_LIMIT);
    templateIds.sort(function(a, b) {
        return a < b ? -1 : 1;
    });
    templateIds.forEach(function(templateId) {
        var templateCardsCount = templateIdToCount[templateId];
        if (templateCardsCount == null) {
            templateCardsCount = 0;
            templateIdToCount[templateId] = 0;
        }
        const templateCardsLeft = TEMPLATE_ID_TO_LIMIT[templateId] - templateCardsCount;
        totalCardsRemaining += templateCardsLeft;
    });
    
    const randomIndex = Math.floor(Math.random() * totalCardsRemaining);
    var mintTemplateId = null;
    var currentIndex = 0;
    for (var i = 0; i < templateIds.length; i += 1) {
        var templateId = templateIds[i];
        var templateCardsCount = templateIdToCount[templateId];
        var templateCardsLeft = TEMPLATE_ID_TO_LIMIT[templateId] - templateCardsCount;
        if (templateCardsLeft <= 0) {
            continue;
        } else if (randomIndex < currentIndex + templateCardsLeft) {
            mintTemplateId = templateId;
            templateIdToCount[templateId] = templateIdToCount[templateId] + 1;
            break;
        }
        currentIndex += templateCardsLeft;
    }
    
    if (mintTemplateId != null) {
        return parseInt(mintTemplateId);
    } else {
        return null;
    }
}

function mintRandomCards(mintCount) {
    const templateIds = [];
    for (var _ = 0; _ < mintCount; _ += 1) {
        var newTemplateId = mintRandomCard();
        if (newTemplateId != null) {
            templateIds.push(newTemplateId);
        }
    }
    const variations = templateIds.map(function(templateId) {
        const randomIndex = Math.floor(Math.random() * 100);
        return randomIndex >= 99 ? 1 : 0;
    });
    
    return [templateIds, variations];
}

function _computeGasLimit(mintCount) {
    if (mintCount <= 1) {
        return 100000; // 91431
    } else if (mintCount <= 25) {
        return 1800000; // 1739780
    } else if (mintCount <= 50) {
        return 3400000; // 3364777
    } else if (mintCount <= 100) {
        return 6800000; // 6704952
    } else {
        Spark.getLog().error({
            errorMessage: "Compute gas limit called on count > 100.",
            mintCount: mintCount,
        });
        return null;
    }
}

function fetchSignedTransaction(txElement) {
    const nonce = fetchNonceByAddress(MINTER_ADDRESS);
    if (txElement.type === "TRANSACTION_TYPE_LOOT_BOX") {
        const quantity = txElement.data.quantity;
        const mintCount = quantity * 5;
        const mintResponse = mintRandomCards(mintCount);
        const templateIds = mintResponse[0];
        const variations = mintResponse[1];
        if (templateIds.length <= 0 || templateIds.length != variations.length) {
            // Create signed transaction failed - log error.
            Spark.getLog().error({
                errorMessage: "Mint random cards fail.",
                templateIds: templateIds,
                variations: variations,
            });
            return null;
        }
        
        var json;
        const gasLimit = _computeGasLimit(templateIds.length);
        
        if (Spark.getConfig().getStage() === "live") {
            json = {
                nonce: nonce,
                recipientAddress: txElement.data.recipientAddress,
                gasPrice: 2,
                gasLimit: gasLimit,
                chainId: 4,
                contractAddress: "",
                privateKey: "",
                templateIds: templateIds,
                variations: variations,
            };
        } else {
            json = {
                nonce: nonce,
                recipientAddress: txElement.data.recipientAddress,
                gasPrice: 2,
                gasLimit: gasLimit,
                chainId: 4,
                contractAddress: "0x948d395aA9Bafb8C819F9A5EC59f36b8E92E375B",
                privateKey: MINTER_PRIVATE_KEY,
                templateIds: templateIds,
                variations: variations,
            };
        }
        
        const jsonString = JSON.stringify(json);
        const response = Spark.getHttp("https://lwiyu7lla4.execute-api.us-west-2.amazonaws.com/Beta/sign-mint-cards").postString(jsonString);
        const responseCode = response.getResponseCode();
        const responseJson = response.getResponseJson();
        
        if (responseJson == null) {
            // Create signed transaction failed - log error.
            Spark.getLog().error({
                errorMessage: "Sign transaction with Lambda failed: no response.",
                json: json,
            });
            return null;
        }
        
        const rawTx = responseJson.rawTransaction;
        const error = responseJson.error;
        if (rawTx) {
            return responseJson.rawTransaction;
        } else {
            // Create signed transaction failed - log error.
            Spark.getLog().error({
                errorMessage: "Sign transaction with Lambda failed: " + error,
                json: json,
            });
            return null;
        }
    }
}

// ====================================================================================================
// LOOT BOX LOGIC END
// ====================================================================================================

master.blockNumber = "0x" + (parseInt(toBlock, 16) + 1).toString(16);

const error = masterDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

Spark.unlockKeyFully(LOCK_EVERY_MINUTE_MODULE);
