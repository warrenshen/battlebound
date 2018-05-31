// ====================================================================================================
//
// Cloud Code for PlayerDeckCreate, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const API = Spark.getGameDataService();
const playerId = Spark.getPlayer().getPlayerId();

const previousName = Spark.getData().previousName;
const name = Spark.getData().name;
const cardIds = Spark.getData().cardIds;

if (typeof previousName !== "string" || typeof name !== "string" || !Array.isArray(cardIds)) {
    Spark.setScriptError("ERROR", "Invalid parameter(s)");
    Spark.exit(); 
}

const playerDecksDataItem = API.getItem("PlayerDecks", playerId).document();
if (playerDecksDataItem === null) {
    Spark.setScriptError("ERROR", "PlayerDecks instance does not exist.");
    Spark.exit(); 
}

const playerDecksData = playerDecksDataItem.getData();
const bCardIds = playerDecksData.bCardIds || [];
const cardByCardId = playerDecksData.cardByCardId;
const deckByName = playerDecksData.deckByName;

// Verify that player actually owns all cards in deck.
// Note that prior to a match a similar verification happens so
// we can simply use the data cached on the PlayerDecks instance here.
const validCardIds = bCardIds.concat(Object.keys(cardByCardId));
cardIds.forEach(function(cardId) {
    if (validCardIds.indexOf(cardId) < 0) {
        Spark.setScriptError("ERROR", "Invalid card ID(s)");
        Spark.exit(); 
    }
});
deckByName[name] = cardIds;

if (deckByName[previousName]) {
    delete deckByName[previousName];
}

// Guarantee that all decks are valid - should be a non-empty array.
Object.keys(deckByName).forEach(function(deckName) {
    if (!Array.isArray(deckByName[deckName]) || deckByName[deckName].length === 0) {
        delete deckByName[deckName];
    }
});

const error = playerDecksDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error)
    Spark.exit();
}

Spark.setScriptData("deckByName", deckByName);
Spark.setScriptData("statusCode", 200);
