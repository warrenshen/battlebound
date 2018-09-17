// ====================================================================================================
//
// Cloud Code for GetPlayerCardAuctions, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AuctionModule");

const player = Spark.getPlayer();
const bCards = getBCardsByPlayer(player);

const address = player.getPrivateData("address");
if (!address) {
    setScriptError("Player does not have an address.");
}

const auctionedCards = findAuctionsBySeller(address);

Spark.setScriptData("auctionableCards", bCards);
Spark.setScriptData("auctionedCards", auctionedCards);
setScriptSuccess();
