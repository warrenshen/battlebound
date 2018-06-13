// ====================================================================================================
//
// Cloud Code for SubmitBidAuctionTransaction, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("OnChainModule");

const player = Spark.getPlayer();
const address = player.address;

const signedTx = Spark.getData().signedTx;
const tokenId = Spark.getData().tokenId;
const bidPrice = Spark.getData().bidPrice;

if (tokenId < 0) {
    Spark.setScriptError("ERROR", "Invalid token ID.");
    Spark.exit();
}

// Get auction start.
const API = Spark.getGameDataService();
    
const bCardId = "B" + tokenId.toString();
const cardDataItem = API.getItem("Card", bCardId).document();

const cardData = cardDataItem.getData();
const auction = cardData.auction;
// Get auction end.

if (auction.seller === address) {
    Spark.setScriptError("ERROR", "Auction cannot be bid on by seller.");
    Spark.exit();
}

const error = cardDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}
    
// Validate that bid price is greater than current price of token on chain.
const currentPrice = fetchAuctionCurrentPriceByTokenId(tokenId);

if (currentPrice <= 0) {
    Spark.setScriptError("ERROR", "Token is not on auction.");
    Spark.exit();
} else if (currentPrice > bidPrice) {
    Spark.setScriptError("ERROR", "Bid price is less than auction current price.");
    Spark.exit();
}

const txHash = submitBidAuctionTransaction(signedTx);

Spark.setScriptData("txHash", txHash);
Spark.setScriptData("statusCode", 200);
