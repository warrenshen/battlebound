// ====================================================================================================
//
// Cloud Code for EveryMinuteModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AuctionModule");
require("MintCardsModule");

const LOCK_EVERY_MINUTE_MODULE = "LOCK_EVERY_MINUTE_MODULE";
Spark.lockKey(LOCK_EVERY_MINUTE_MODULE, 5000);

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
        category: log.category,
        quantity: log.quantity,
        recipientAddress: log.recipientAddress,
    });
    transactionQueue = transactionQueue.concat([{
        type: "TRANSACTION_TYPE_LOOT_BOX",
        data: {
            category: log.category,
            quantity: log.quantity,
            recipientAddress: log.recipientAddress,
        },
    }]);
});

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

// ====================================================================================================
// LOOT BOX LOGIC END
// ====================================================================================================

master.blockNumber = "0x" + (parseInt(toBlock, 16) + 1).toString(16);

const error = masterDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

Spark.unlockKeyFully(LOCK_EVERY_MINUTE_MODULE);
