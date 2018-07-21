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

if (matchShortCode === "CasualMatch") {
    var request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = 0;
    request.Execute();
} else if (matchShortCode === "RankedMatch") {
    var scoreRequest = new SparkRequests.GetLeaderboardEntriesRequest();
    scoreRequest.leaderboards = ["HIGH_SCORE_LB"];
    scoreRequest.player = playerId;
    var response = scoreRequest.Send();
    rankedScore = response.HIGH_SCORE_LB.SCORE_ATTR;
    
    var request = new SparkRequests.MatchmakingRequest();
    request.matchShortCode = matchShortCode;
    request.skill = rankedScore;
    request.Execute();
    
} else {
    setScriptError("Invalid match short code.");
}

setScriptSuccess();
