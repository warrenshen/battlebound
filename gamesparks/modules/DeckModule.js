// ====================================================================================================
//
// Cloud Code for DeckModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
/**
 * Fetches Card and Template objects from the DB and combines
 * them to form card objects as shown to the client.
 * 
 * @param cardIds - array of Card IDs
 * @return - array of Card objects
 **/
function getDeckByCardIds(cardIds) {
    var API = Spark.getGameDataService();
    var cards = [];
    
    var cardsDataQuery = API.S("id").in(cardIds);
    var cardsDataQueryResult = API.queryItems("Card", cardsDataQuery);
    var cardsDataQueryResultError = cardsDataQueryResult.error();
    
    if (cardsDataQueryResultError) {
        Spark.setScriptError("ERROR", cardsDataQueryResultError);
        Spark.exit();
    } else {
        var cardsDataCursor = cardsDataQueryResult.cursor();
        while (cardsDataCursor.hasNext()) {
            var cardDataItem = cardsDataCursor.next();
            cards.push(cardDataItem.getData());
        }    
    }
    
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
        
        return result;
    });
    
    return results;
}

/**
 * Returns array of card IDs of active deck of player.
 * 
 * @param playerId - GS player ID
 * @return array - array of card IDs
 **/
function getActiveDeckByPlayerId(playerId) {
    var API = Spark.getGameDataService();
    
    var decksGetResult = API.getItem("PlayerDecks", playerId);
    var decksDataItem = decksGetResult.document();
    var decksData = decksDataItem.getData();
    var decks = decksData.decks;
    var activeDeck = decksData.activeDeck;
    
    return decks[activeDeck];
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
