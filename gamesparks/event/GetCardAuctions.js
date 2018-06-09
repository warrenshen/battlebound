// ====================================================================================================
//
// Cloud Code for GetCardAuctions, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("DeckModule");

const API = Spark.getGameDataService();

const masterDataItem = API.getItem("Master", "master").document();

if (masterDataItem === null) {
    Spark.setScriptError("ERROR", "Master does not exist.");
    Spark.exit();
}

const master = masterDataItem.getData();
const auctions = master.auctions;

const bCardIds = auctions.map(function(cardInt) { return "B" + cardInt.toString() });
const bCards = _getBCardsByBCardIds(bCardIds);

Spark.setScriptData("auctions", bCards);
