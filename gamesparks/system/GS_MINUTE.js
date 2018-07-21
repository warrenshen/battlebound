// ====================================================================================================
//
// Cloud Code for GS_MINUTE, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AuctionModule");

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

const BLOCK_CONFIRMATIONS_COUNT = 8;

const latestBlockNumber = fetchLatestBlockNumber();
const toBlock = "0x" + (parseInt(latestBlockNumber, 16) - BLOCK_CONFIRMATIONS_COUNT).toString(16);

var auctions = master.auctions;
const fromBlock = master.blockNumber;

const auctionCreatedLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_CREATED, fromBlock, toBlock);
const auctionCanceledLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_CANCELED, fromBlock, toBlock);
const auctionSuccessfulLogs = fetchLogsByBlockRange(BLOCKCHAIN_EVENT_AUCTION_SUCCESSFUL, fromBlock, toBlock);

const logs = auctionCreatedLogs.concat(auctionCanceledLogs.concat(auctionSuccessfulLogs));
// Sort logs into "chronological" order.
logs.sort(function(logA, logB) {
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

logs.forEach(function(log) {
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
master.blockNumber = "0x" + (parseInt(toBlock, 16) + 1).toString(16);

const error = masterDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

// Spark.getLog().info("GS_MINUTE success up to block: " + toBlock);
