require("ScriptDataModule");
require("DeckModule");

const player = Spark.getPlayer();

const result = getCardsAndDecksByPlayer(player);
const cards = result[0];
const decks = result[1];

const TEMP_CARD_NAME_WHITELIST = [
    "Blessed Newborn",
    "Temple Guardian",
    "Cursed Imp",
    "Waterborne Razorback",
    "Unstable Power",
    "Pyre Dancer",
    "Firebug Catelyn",
    "Marshwater Squealer",
    "Taji the Fearless",
];

const filteredCards = cards.filter(function(card) {
    return TEMP_CARD_NAME_WHITELIST.indexOf(card.name) >= 0;
});
const filteredCardIds = filteredCards.map(function(card) { return card.id });
const filteredDecks = {};
Object.keys(decks).forEach(function(deckName) {
    filteredDecks[deckName] = decks[deckName].filter(function(cardId) { return filteredCardIds.indexOf(cardId) >= 0 });
});

// Spark.setScriptData("cards", filteredCards);
// Spark.setScriptData("decks", filteredDecks);
Spark.setScriptData("cards", cards);
Spark.setScriptData("decks", decks);
setScriptSuccess();
