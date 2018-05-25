// ====================================================================================================
//
// A module of functions related to the PlayerDecks table and in-memory deck arrays.
//
// ====================================================================================================
 
/**
 * Fetches Template objects from the DB and uses
 * them to form card objects as shown to the client.
 * 
 * @param cards - array of cards from a PlayerDecks instance
 * @return - array of Card objects
 **/
function _getDeckByCards(cards) {
    var API = Spark.getGameDataService();

    // Query Template table by `templateIds`.
    var templateIds = cards.map(function(card) { return card.templateId });
    var templates = [];
    
    var templatesDataQuery = API.S("id").in(templateIds);
    var templatesDataQueryResult = API.queryItems("Template", templatesDataQuery);
    var templatesDataQueryResultError = templatesDataQueryResult.error();
    
    if (templatesDataQueryResultError) {
        Spark.setScriptError("ERROR", templatesDataQueryResultError);
        Spark.exit();
    } else {
        var templatesDataCursor = templatesDataQueryResult.cursor();
        while (templatesDataCursor.hasNext()) {
            var templateDataItem = templatesDataCursor.next();
            templates.push(templateDataItem.getData());
        }
    }
    
    var templateIdToTemplate = {};
    templates.map(function(template) { templateIdToTemplate[template.id] = template });
    
    var cardFields = [
        "id",
        "level"
    ];
    var templateFields = [
        // "type",
        "attack",
        "health",
        "manaCost",
        "name"
    ];
    
    // Form final card objects by combining fields of both Card and Template objects.
    var results = cards.map(function(card) {
        var result = {};
        var template = templateIdToTemplate[card.templateId];
        
        cardFields.map(function(field) { result[field] = card[field] });
        templateFields.map(function(field) { result[field] = template[field] });
        result.type = "CARD_TYPE_MINION"; // Temp.
        
        return result;
    });
    
    return results;
}

/**
 * Returns array of card objects of all cards player owns.
 * 
 * @param playerId - GS player ID
 * @return array - array of card objects
 **/
function getCardsAndDecksByPlayerId(playerId) {
    // The term instance = card + template combined.
    var API = Spark.getGameDataService();
    
    var decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    var decksData = decksDataItem.getData();
    var decks = decksData.decks;
    var cardByCardId = decksData.cardByCardId;
    
    var cards = Object.keys(cardByCardId).map(function(cardId) {
        return cardByCardId[cardId]; 
    });
    
    // Array of instances of all cards of player;
    var instances = _getDeckByCards(cards);
    
    var instanceByCardId = {};
    instances.map(function(instance) {
       instanceByCardId[instance.id] = instance;
    });
    
    var deckNameToDeck = {};
    Object.keys(decks).map(function(deckName) {
        var deck = decks[deckName];
        deckNameToDeck[deckName] = deck.map(function(cardId) {
            return instanceByCardId[cardId];
        });
    });
    return [instances, deckNameToDeck];
}

/**
 * Returns array of card objects of active deck of player.
 * 
 * @param playerId - GS player ID
 * @return array - array of card objects
 **/
function getActiveDeckByPlayerId(playerId) {
    var API = Spark.getGameDataService();
    
    var decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    var decksData = decksDataItem.getData();
    var decks = decksData.decks;
    var activeDeck = decksData.activeDeck;
    var cardIds = decks[activeDeck];
    var cardByCardId = decksData.cardByCardId;
    
    var cards = cardIds.map(function(cardId) {
        return cardByCardId[cardId]; 
    });
    
    return _getDeckByCards(cards);
}

/**
 * @param deck - a non-empty array of Card objects.
 * @return - a two element array: drawn Card object + array of remaining Card objects
 **/
function drawCard(deck) {
    if (!Array.isArray(deck) || deck.length === 0) {
        Spark.setScriptError("ERROR", "Invalid deck parameter.");
        Spark.exit();
    }
    var deckSize = deck.length;
    var randomIndex = Math.floor(Math.random() * (deckSize - 1));
    return [deck[randomIndex], deck.slice(0, randomIndex).concat(deck.slice(randomIndex + 1))];
}
