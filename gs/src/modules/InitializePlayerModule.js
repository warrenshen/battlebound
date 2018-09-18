// ====================================================================================================
//
// Initializes a PlayerDecks instance for a player ID.
// This consists of non-blockchain starter cards.
// Note that non-blockhain templates have IDs prefixed with a "C" whereas
// blockchain templates have IDs prefixed with a "B".
//
// ====================================================================================================
function initializePlayerByPlayerId(playerId) {
    const player = Spark.loadPlayer(playerId);
    
    if (player === null) {
        setScriptError("Player does not exist.");
    }
    
    const API = Spark.getGameDataService();
    // Array of template IDs a new user starts with.
    const templateIds = [
        "C0",
        "C0",
        "C1",
        "C1",
        "C2",
        "C2",
        "C3",
        "C3",
        "C4",
        "C4",
        "C5",
        "C5",
        "C6",
        "C6",
        "C7",
        "C7",
        "C8",
        "C8",
        "C9",
        "C9",
        "C10",
        "C10",
        "C11",
        "C11",
        "C12",
        "C12",
        "C13",
        "C13",
        "C14",
        "C14",
        "C15",
        "C15",
        "C16",
        "C16",
        "C17",
        "C17",
        "C18",
        "C18",
        "C19",
        "C19",
        "C20",
        "C20",
        "C21",
        "C21",
        "C22",
        "C22",
        "C23",
        "C23",
        "C24",
        "C24",
        "C25",
        "C25",
        "C26",
        "C26",
        // "C27",
        "C28",
        "C28",
        "C29",
        "C29",
        "C30",
        "C30",
        // "C31",
        // "C32",
        // "C33",
        // "C34",
        // "C35",
        // "C36",
        // "C37",
        // "C38",
        // "C39",
        // "C40",
        // "C41",
        // "C42",
        // "C43",
        // "C44",
        // "C45",
        // "C46",
        // "C47",
        // "C48",
        // "C49",
        // "C50",
        // "C51",
        // "C52",
        // "C53",
        // "C54",
        // "C55",
        "C56",
        "C56",
    ];
    
    const cards = templateIds.map(function(templateId) {
        return {
            templateId: templateId,
            level: 1,
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
    playerDecksData.activeDeck = "Basic";
    const deckByName = playerDecksData.deckByName = {};
    deckByName["Basic"] = [
        "C0",
        "C2",
        "C4",
        "C6",
        "C8",
        "C10",
        "C12",
        "C14",
        "C16",
        "C18",
        "C20",
        "C22",
        "C24",
        "C26",
        "C28",
        "C30",
        "C32",
        "C34",
        "C36",
        "C38",
        "C40",
        "C42",
        "C44",
        "C46",
        "C48",
        "C50",
        "C52",
        "C54",
        "C56",
        "C58"
    ];
    
    const error = playerDecksDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
    
    return playerDecksDataItem;
}
