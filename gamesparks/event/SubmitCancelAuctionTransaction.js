// ====================================================================================================
//
// Cloud Code for SubmitBidAuctionTransaction, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const player = Spark.getPlayer();
const address = player.address;

const signedTx = Spark.getData().signedTx;
const tokenId = Spark.getData().tokenId;

if (tokenId < 0) {
    setScriptError("Invalid token ID.");
}

// Get auction start.
const API = Spark.getGameDataService();
    
const bCardId = "B" + tokenId.toString();
const cardDataItem = API.getItem("Card", bCardId).document();

const cardData = cardDataItem.getData();
const auction = cardData.auction;
// Get auction end.

if (auction.seller !== address) {
    setScriptError("Auction cannot be canceled by non-seller.");
}
    
// Validate that price is valid which means that auction is valid.
const currentPrice = fetchAuctionCurrentPriceByTokenId(tokenId);

if (currentPrice <= 0) {
    setScriptError("Token is not on auction.");
}

const txHash = submitCancelAuctionTransaction(signedTx);
cardData.txHash = txHash;

const error = cardDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

Spark.setScriptData("txHash", txHash);
setScriptSuccess();

