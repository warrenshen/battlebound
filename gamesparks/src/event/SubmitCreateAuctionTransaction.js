// ====================================================================================================
//
// Cloud Code for SubmitCreateAuctionTransaction, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const publicAddress = Spark.getData().publicAddress;
const signedTx = Spark.getData().signedTx;
const tokenId = Spark.getData().tokenId;
const startingPrice = Spark.getData().startingPrice;
const endingPrice = Spark.getData().endingPrice;

const player = Spark.getPlayer();
const address = player.getPrivateData("address");

if (prefixHex(publicAddress.toLowerCase()) != prefixHex(address.toLowerCase())) {
    setScriptError("Public address does not match player's address.");
}

if (tokenId < 0) {
    setScriptError("Invalid token ID.");
}

if (startingPrice <= 0 || endingPrice <= 0) {
    setScriptError("Starting price and ending price must be greater than 0.");
}

// Check that auction for token does not exist yet (started at should be 0).
// We check the chain instead of our local cache to be even more accurate.
const auctionOnChain = fetchAuctionByCardInt(tokenId);
const startedAt = auctionOnChain.startedAt;

if (startedAt > 0) {
    setScriptError("Token is already on auction.");
}

const txHash = submitCreateAuctionTransaction(signedTx);

// Get auction start.
const API = Spark.getGameDataService();
    
const bCardId = "B" + tokenId.toString();
const cardDataItem = API.getItem("Card", bCardId).document();

const cardData = cardDataItem.getData();
const auction = cardData.auction;
// Get auction end.

cardData.txHash = txHash;

const error = cardDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

Spark.setScriptData("txHash", txHash);
setScriptSuccess();
