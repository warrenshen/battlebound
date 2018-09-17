// ====================================================================================================
//
// Cloud Code for GetCardAuctions, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("DeckModule");

const player = Spark.getPlayer();
var address = player.getPrivateData("address");
if (address === null)
{
    address = "";
}

const API = Spark.getGameDataService();

const masterDataItem = API.getItem("Master", "master").document();

if (masterDataItem === null) {
    setScriptError("Master does not exist.");
}

const master = masterDataItem.getData();
const auctions = master.auctions;

const bCardIds = auctions.map(function(cardInt) { return "B" + cardInt.toString() });
const bCards = getBCardsByBCardIds(bCardIds);

const AUCTIONABLE_CARD_FIELDS = [
    "id",
    "level",
    "auction",
    "seller",
];
const instances = getInstancesByCards(bCards, AUCTIONABLE_CARD_FIELDS);
const validAuctions = instances.filter(function(instance) { return instance.seller != prefixHex(address.toLowerCase()) });

var balance = 0;
if (address) {
    balance = fetchBalanceByAddress(address);
}
Spark.setScriptData("address", address);
Spark.setScriptData("balance", balance);
Spark.setScriptData("auctions", validAuctions);
setScriptSuccess();
