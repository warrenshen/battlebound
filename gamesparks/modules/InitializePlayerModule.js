// ====================================================================================================
//
// Initializes a PlayerDecks instance for current player.
// This consists of non-blockchain starter cards.
// Note that non-blockhain templates have IDs prefixed with a "C" whereas
// blockchain templates have IDs prefixed with a "B".
//
// ====================================================================================================
require("AddressModule");

function initializePlayer() {
    const player = Spark.getPlayer();
    
    if (player === null) {
        Spark.setScriptError("ERROR", "Player does not exist.");
        Spark.exit();
    }
    
    const API = Spark.getGameDataService();
    
    resetPlayerAddressChallenge();
    
    // Array of template IDs a new user starts with.
    const templateIds = [
        "C0",
        "C0",
        "C1",
        "C2",
        "C2",
        "C3",
        "C4",
        "C4"
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
    
    const playerDecksDataItem = API.createItem("PlayerDecks", player.getPlayerId());
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
