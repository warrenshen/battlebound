// ====================================================================================================
//
// Cloud Code for FindMatch, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var API = Spark.getGameDataService();
var player = Spark.getPlayer();
var playerId = player.getPlayerId();

var matchShortCode = Spark.getData().matchShortCode;
var playerDeck = Spark.getData().playerDeck;

if (!matchShortCode || !playerDeck) {
    Spark.setScriptError("ERROR", "Invalid parameter(s)");
    Spark.exit();
}

var playerDecksGetResult = API.getItem("PlayerDecks", playerId);
var playerDecksDataItem = playerDecksGetResult.document();

if (playerDecksDataItem === null) {
    Spark.setScriptError("ERROR", "ERROR");
    Spark.exit();
}

var playerDecksData = playerDecksDataItem.getData();
if (!playerDecksData.decks[playerDeck]) {
    Spark.setScriptError("ERROR", "Invalid player deck parameter");
    Spark.exit();
}

playerDecksData.activeDeck = playerDeck;

var error = playerDecksDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error)
    Spark.exit();
}

if (matchShortCode === "CasualMatch") {
    var request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = 0;
    request.Execute();
} else {
    Spark.setScriptError("ERROR", "Invalid match short code");
    Spark.exit();
}

Spark.setScriptData("statusCode", 200);
