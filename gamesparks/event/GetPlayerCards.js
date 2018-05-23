require("DeckModule");
require("OnChainModule");

// var cardsOnChain = fetchCardsOnChainByAddress("0x2aFAf114B6833B951A00610cEdeDD3D7c7A48AE8");
// var cardsOnChain = fetchCardsOnChainByAddress("0x6014dC52AfEa7b5faf11458A02A5DA5761f81AEB");

const playerId = Spark.getPlayer().getPlayerId();

const result = getCardsAndDecksByPlayerId(playerId);
const cards = result[0];
const decks = result[1];

Spark.setScriptData("cards", cards);
Spark.setScriptData("decks", decks)
Spark.setScriptData("statusCode", 200);
