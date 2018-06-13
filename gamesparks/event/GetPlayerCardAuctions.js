// ====================================================================================================
//
// Cloud Code for GetPlayerCardAuctions, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AuctionModule");

const player = Spark.getPlayer();
const bCards = getBCardsByPlayer(player);

const auctionedCards = findAuctionedCardsBySeller(player.getPrivateData("address"));

Spark.setScriptData("auctionableCards", bCards);
Spark.setScriptData("auctionedCards", auctionedCards);
