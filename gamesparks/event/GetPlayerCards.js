// ====================================================================================================
//
// Cloud Code for GetLibrary, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("DeckModule");
require("OnChainModule");

// var cardsOnChain = fetchCardsOnChainByAddress("0x2aFAf114B6833B951A00610cEdeDD3D7c7A48AE8");
// var cardsOnChain = fetchCardsOnChainByAddress("0x6014dC52AfEa7b5faf11458A02A5DA5761f81AEB");

var API = Spark.getGameDataService();
var playerId = Spark.getPlayer().getPlayerId();

var playerDecksGetResult = API.getItem("PlayerDecks", playerId);
var playerDecksDataItem = playerDecksGetResult.document();

if (playerDecksDataItem === null) {
    Spark.setScriptError("ERROR", "Player decks does not exist.");
    Spark.exit();
}

var playerDecksData = playerDecksDataItem.getData();
var cardIds = playerDecksData.cardIds;

var cards = getDeckFromCardIds(cardIds);
Spark.setScriptData("cards", cards);
