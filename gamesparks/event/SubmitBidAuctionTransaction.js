// ====================================================================================================
//
// Cloud Code for SubmitBidAuctionTransaction, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const publicAddress = Spark.getData().publicAddress;
const signedTx = Spark.getData().signedTx;
const tokenId = Spark.getData().tokenId;
const bidPrice = Spark.getData().bidPrice;

const player = Spark.getPlayer();
const address = player.getPrivateData("address");

if (prefixHex(publicAddress.toLowerCase()) != prefixHex(address.toLowerCase())) {
    setScriptError("Public address does not match player's address.");
}

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

if (cardData.seller === address) {
    setScriptError("Auction cannot be bid on by seller.");
}
    
// Validate that bid price is greater than current price of token on chain.
const currentPrice = fetchAuctionCurrentPriceByTokenId(tokenId);

if (currentPrice <= 0) {
    setScriptError("Token is not on auction.");
} else if (currentPrice > bidPrice) {
    setScriptError("Bid price is less than auction current price.");
}

const txHash = submitBidAuctionTransaction(signedTx);
cardData.txHash = txHash;

const error = cardDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

Spark.setScriptData("txHash", txHash);
setScriptSuccess();
