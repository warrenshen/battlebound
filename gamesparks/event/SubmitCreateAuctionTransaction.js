// ====================================================================================================
//
// Cloud Code for SubmitCreateAuctionTransaction, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("OnChainModule");

const signedTx = Spark.getData().signedTx;
const tokenId = Spark.getData().tokenId;
const startingPrice = Spark.getData().startingPrice;
const endingPrice = Spark.getData().endingPrice;

if (tokenId < 0) {
    Spark.setScriptError("ERROR", "Invalid token ID.");
    Spark.exit();
}

if (startingPrice <= 0 || endingPrice <= 0) {
    Spark.setScriptError("ERROR", "Starting price and ending price must be greater than 0.");
    Spark.exit();
}

// Check that auction for token does not exist yet (started at should be 0).
// We check the chain instead of our local cache to be even more accurate.
const auctionOnChain = fetchAuctionByCardInt(tokenId);
const startedAt = auctionOnChain.startedAt;

if (startedAt > 0) {
    Spark.setScriptError("ERROR", "Token is already on auction.");
    Spark.exit();
}

const txHash = submitCreateAuctionTransaction(signedTx);

Spark.setScriptData("txHash", txHash);
Spark.setScriptData("statusCode", 200);
