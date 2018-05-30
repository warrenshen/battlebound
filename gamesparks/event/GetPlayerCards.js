require("DeckModule");

// var cardsOnChain = fetchCardsOnChainByAddress("0x6014dC52AfEa7b5faf11458A02A5DA5761f81AEB");

const player = Spark.getPlayer();

const result = getCardsAndDecksByPlayer(player);
const cards = result[0];
const decks = result[1];

Spark.setScriptData("cards", cards);
Spark.setScriptData("decks", decks)
Spark.setScriptData("statusCode", 200);
