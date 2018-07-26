// ====================================================================================================
//
// Cloud Code for PlayerDeckCreate, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("DeckModule");

const API = Spark.getGameDataService();
const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const previousName = Spark.getData().previousName;
const name = Spark.getData().name;
const cardIds = Spark.getData().cardIds;

if (typeof previousName !== "string" || typeof name !== "string" || !Array.isArray(cardIds)) {
    setScriptError("Invalid parameter(s)");
}

const playerDecksDataItem = API.getItem("PlayerDecks", playerId).document();
if (playerDecksDataItem === null) {
    setScriptError("PlayerDecks instance does not exist.");
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
        setScriptError("Invalid card ID(s)");
    }
});

// Verify that there is no duplicate card.
const cardIdsDict = {};
cardIds.forEach(function(cardId) {
    if (cardIdsDict[cardId] == null) {
        cardIdsDict[cardId] = 0;
    } else {
        setScriptError("Cannot have duplicate card ID in a deck.");
    }
});

// Verify that there is at most two cards of the same template.
const bIds = cardIds.filter(function(cardId) { return cardId.indexOf("B") === 0 });
const cIds = cardIds.filter(function(cardId) { return cardId.indexOf("C") === 0 });
const bCards = getBCardsByBCardIds(bIds);
const cCards = cIds.map(function(cId) {
    return cardByCardId[cId];
});

const templateIdToCount = {};
bCards.concat(cCards).forEach(function(card) {
    var templateId = card.templateId;
    if (templateIdToCount[templateId] == null) {
        templateIdToCount[templateId] = 1;
    } else {
        templateIdToCount[templateId] += 1;
    }
    
    if (templateIdToCount[templateId] > 2) {
        setScriptError("Cannot have more than two of any template ID in a deck.");
    }
});
//--

deckByName[name] = cardIds;

if (previousName !== name && deckByName[previousName]) {
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
    setScriptError(error)
}

const result = getCardsAndDecksByPlayer(player);
const cards = result[0];
const decks = result[1];

Spark.setScriptData("cards", cards);
Spark.setScriptData("decks", decks);
setScriptSuccess();
