// ====================================================================================================
//
// Cloud Code for PlayerDecksGet, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var API = Spark.getGameDataService();
var playerId = Spark.getPlayer().getPlayerId();

var playerDecksGetResult = API.getItem("PlayerDecks", playerId);
var playerDecksDataItem = playerDecksGetResult.document();

if (playerDecksDataItem === null) {
    Spark.setScriptError("ERROR", "ERROR");
    Spark.exit();
}

var playerDecksData = playerDecksDataItem.getData();
var decks = playerDecksData.decks;

Spark.setScriptData("decks", decks);
