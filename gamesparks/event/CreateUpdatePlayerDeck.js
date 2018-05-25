// ====================================================================================================
//
// Cloud Code for PlayerDeckCreate, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("InitializePlayerModule");

const API = Spark.getGameDataService();
const playerId = Spark.getPlayer().getPlayerId();

const previousName = Spark.getData().previousName;
const name = Spark.getData().name;
const cardIds = Spark.getData().cardIds;

if (typeof previousName !== "string" || typeof name !== "string" || !Array.isArray(cardIds)) {
    Spark.setScriptError("ERROR", "Invalid parameter(s)");
    Spark.exit(); 
}

const playerDecksGetResult = API.getItem("PlayerDecks", playerId);
const playerDecksDataItem = playerDecksGetResult.document();

if (playerDecksDataItem === null) {
    initializePlayer(playerId);
    playerDecksGetResult = API.getItem("PlayerDecks", playerId);
    playerDecksDataItem = playerDecksGetResult.document();
}

const playerDecksData = playerDecksDataItem.getData();
const cardByCardId = playerDecksData.cardByCardId;
const decks = playerDecksData.decks;

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

const error = playerDecksDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error)
    Spark.exit();
}

Spark.setScriptData("decks", decks);
Spark.setScriptData("statusCode", 200);
