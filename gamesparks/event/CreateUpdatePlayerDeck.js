// ====================================================================================================
//
// Cloud Code for PlayerDeckCreate, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("InitializePlayerModule");

var API = Spark.getGameDataService();
var playerId = Spark.getPlayer().getPlayerId();

var previousName = Spark.getData().previousName;
var name = Spark.getData().name;
var cardIds = Spark.getData().cardIds;

if (typeof previousName !== "string" || typeof name !== "string" || !Array.isArray(cardIds)) {
    Spark.setScriptError("ERROR", "Invalid parameter(s)");
    Spark.exit(); 
}

var playerDecksGetResult = API.getItem("PlayerDecks", playerId);
var playerDecksDataItem = playerDecksGetResult.document();

if (playerDecksDataItem === null) {
    var error = initializePlayer(playerId);

    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    } else {
        playerDecksGetResult = API.getItem("PlayerDecks", playerId);
        playerDecksDataItem = playerDecksGetResult.document();
    }
}

var playerDecksData = playerDecksDataItem.getData();
var cardByCardId = playerDecksData.cardByCardId;
var decks = playerDecksData.decks;

cardIds.forEach(function(cardId) {
    if (!cardByCardId[cardId]) {
        Spark.setScriptError("ERROR", "Invalid card ID(s)");
        Spark.exit(); 
    }
});
decks[name] = cardIds;

if (decks[previousName]) {
    delete decks[previousName];
}

// Guarantee that all decks are valid - should be a non-empty array.
Object.keys(decks).forEach(function(deckName) {
    if (!Array.isArray(decks[deckName]) || decks[deckName].length === 0) {
        delete decks[deckName];
    }
});

var error = playerDecksDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error)
    Spark.exit();
}

Spark.setScriptData("decks", decks);
Spark.setScriptData("statusCode", 200);
