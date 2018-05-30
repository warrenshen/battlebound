// ====================================================================================================
//
// Cloud Code for FindMatch, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("DeckModule");

const API = Spark.getGameDataService();
const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const matchShortCode = Spark.getData().matchShortCode;
const playerDeck = Spark.getData().playerDeck;

if (!matchShortCode || !playerDeck) {
    Spark.setScriptError("ERROR", "Invalid parameter(s).");
    Spark.exit();
}

const playerDecksDataItem = API.getItem("PlayerDecks", playerId).document();

if (playerDecksDataItem === null) {
    Spark.setScriptError("ERROR", "PlayerDecks instance does not exist.");
    Spark.exit();
}

const playerDecksData = playerDecksDataItem.getData();
if (!playerDecksData.deckByName[playerDeck]) {
    Spark.setScriptError("ERROR", "Invalid player deck parameter.");
    Spark.exit();
}

// Sync player's PlayerDecks instance with blockchain.
const isChanged = syncPlayerDecksByPlayer(player);

playerDecksData.activeDeck = playerDeck;

const error = playerDecksDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}

if (matchShortCode === "CasualMatch") {
    const request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = 0;
    request.Execute();
} else {
    Spark.setScriptError("ERROR", "Invalid match short code.");
    Spark.exit();
}

Spark.setScriptData("statusCode", 200);
