// ====================================================================================================
//
// Cloud Code for FindMatch, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("DeckModule");

const API = Spark.getGameDataService();
const player = Spark.getPlayer();
const playerId = player.getPlayerId();

if (player.getPrivateData("activeChallengeId")) {
    setScriptError("Player already in an active challenge.");
}

const matchShortCode = Spark.getData().matchShortCode;
const playerDeck = Spark.getData().playerDeck;

if (!matchShortCode || !playerDeck) {
    setScriptError("Invalid parameter(s).");
}

const playerDecksDataItem = API.getItem("PlayerDecks", playerId).document();

if (playerDecksDataItem === null) {
    setScriptError("PlayerDecks instance does not exist.");
}

const playerDecksData = playerDecksDataItem.getData();
if (!playerDecksData.deckByName[playerDeck]) {
    setScriptError("Invalid player deck parameter.");
}

// Sync player's PlayerDecks instance with blockchain.
syncPlayerDecksByPlayer(player);

playerDecksData.activeDeck = playerDeck;

const error = playerDecksDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

var request;
var response;

if (matchShortCode === "CasualMatch") {
    // Cancel any in-flight matchmaking request.
    request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = 0;
    request.action = "cancel";
    response = request.Execute();
    if (response.error != null) {
        setScriptError(response.error);
    }
    
    // Create new matchmaking request.
    request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = 0;
    response = request.Execute();
    
    if (response.error != null) {
        setScriptError(response.error);
    }
} else if (matchShortCode === "RankedMatch") {
    // Cancel any in-flight matchmaking request.
    request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.action = "cancel";
    response = request.Execute();
    if (response.error != null) {
        setScriptError(response.error);
    }
    
    // Get player rank for matchmaking.
    request = new SparkRequests.GetLeaderboardEntriesRequest();
    request.leaderboards = ["HIGH_SCORE_LB"];
    request.player = playerId;
    response = request.Execute();
    if (response.error != null) {
        setScriptError(response.error);
    }
    rankedScore = response.HIGH_SCORE_LB.SCORE_ATTR;
    
    // Create new matchmaking request.
    request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = rankedScore;
    response = request.Execute();
    if (response.error != null) {
        setScriptError(response.error);
    }
} else {
    setScriptError("Invalid match short code.");
}

setScriptSuccess();
