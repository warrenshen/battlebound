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

    var playerDecksDataItem = API.createItem("PlayerDecks", playerId);
    var playerDecksData = playerDecksDataItem.getData();
    playerDecksData.cardIds = ["B0", "B1"];
    playerDecksData.decks = {};
    playerDecksData.activeDeck = "";
    
    error = playerDecksDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
}