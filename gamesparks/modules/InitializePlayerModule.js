// ====================================================================================================
//
// Cloud Code for InitializePlayerModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function initializePlayer(playerId) {
    var error;
    var API = Spark.getGameDataService();
    
    // Array of template IDs a new user starts with.
    var templateIds = [
        "C0",
        "C0",
        "C0"
    ];
    
    var cards = templateIds.map(function(templateId) {
        return {
            templateId: templateId,
            level: 0,
        };
    });
    
    var cardByCardId = {};
    cards.forEach(function(card, index) {
        var cardId = "C" + index.toString();
        card.id = cardId;
        cardByCardId[cardId] = card; 
    });
    
    /**
     * PlayerDecks schema: {
     *   cardByCardId: {
     *     [id]: {
     *       templateId: string,
     *       level: int,
     *     },
     *     ...
     *   },
     *   decks: {
     *     [name]: [int (card id), ...]
     *   },
     *   activeDeck: string,
     * }
     **/
    
    var playerDecksDataItem = API.createItem("PlayerDecks", playerId);
    var playerDecksData = playerDecksDataItem.getData();
    playerDecksData.cardByCardId = cardByCardId;
    playerDecksData.decks = {};
    playerDecksData.activeDeck = "";
    
    error = playerDecksDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
}
