// ====================================================================================================
//
// Cloud Code for GetMarketplaceData, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AuctionModule");
require("DeckModule");
require("OnChainModule");

const player = Spark.getPlayer();
var address = player.getPrivateData("address");
if (address === null)
{
    address = "";
}

const buyableAuctions = findAuctionsExceptSeller(address);
const cancelableAuctions = findAuctionsBySeller(address);

const bCards = getBCardsByPlayer(player);

Spark.setScriptData("buyableCards", buyableAuctions);
Spark.setScriptData("sellableCards", bCards);
Spark.setScriptData("cancelableCards", cancelableAuctions);

Spark.setScriptData("contractAddressTreasury", TREASURY_CONTRACT_ADDRESS);
Spark.setScriptData("contractAddressAuction", AUCTION_CONTRACT_ADDRESS);
Spark.setScriptData("gasPriceSuggested", "3000000000");

setScriptSuccess();
