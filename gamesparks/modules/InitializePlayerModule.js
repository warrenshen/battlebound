// ====================================================================================================
//
// Initializes a PlayerDecks instance for player associated with given player ID.
// This consists of non-blockchain starter cards.
// Note that non-blockhain templates have IDs prefixed with a "C" whereas
// blockchain templates have IDs prefixed with a "B".
//
// PlayerDecks schema: {
//   cardByCardId: {
//     [id]: {
//       templateId: string,
//       level: int,
//     },
//     ...
//   },
//   decks: {
//    [name]: [int (card id), ...]
//   },
//   activeDeck: string,
// }
//
// ====================================================================================================
function initializePlayer(playerId) {
    const API = Spark.getGameDataService();
    
    // Array of template IDs a new user starts with.
    const templateIds = [
        "C0",
        "C0",
        "C0"
    ];
    
    const cards = templateIds.map(function(templateId) {
        return {
            templateId: templateId,
            level: 0,
        };
    });
    
    const cardByCardId = {};
    cards.forEach(function(card, index) {
        var cardId = "C" + index.toString();
        card.id = cardId;
        cardByCardId[cardId] = card; 
    });
    
    const playerDecksDataItem = API.createItem("PlayerDecks", playerId);
    const playerDecksData = playerDecksDataItem.getData();
    
    playerDecksData.cardByCardId = cardByCardId;
    playerDecksData.decks = {};
    playerDecksData.activeDeck = "";
    
    const error = playerDecksDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
}
