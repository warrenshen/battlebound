require("ScriptDataModule");
require("DeckModule");

const player = Spark.getPlayer();

const result = getCardsAndDecksByPlayer(player);
const cards = result[0];
const decks = result[1];

Spark.setScriptData("cards", cards);
Spark.setScriptData("decks", decks)
setScriptSuccess();
