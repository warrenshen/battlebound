// ====================================================================================================
//
// This module contains inline code to generate challenge user message
// response to the players. It fetches the ChallengeState associated
// with the challenge ID and sets two scriptData fields:
// "playerState" and "opponentState".
// 
// Note this is in-line with the design pattern where "global messages"
// change the ChallengeState and "user messages" just read from it.
//
// ====================================================================================================
 
 var API = Spark.getGameDataService();

var playerId = Spark.getPlayer().getPlayerId();
var challengeId = Spark.getData().challenge.challengeId;
var challenge = Spark.getChallenge(challengeId);

var challengeStateGetResult = API.getItem("ChallengeState", challengeId);
var challengeStateDataItem = challengeStateGetResult.document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "Challenge state does not exist.");
    Spark.exit();
}

var challengeStateData = challengeStateDataItem.getData();
var currentChallengeState = challengeStateData.current;

var opponentId = challengeStateData.opponentIdByPlayerId[playerId];

var currentPlayerState = currentChallengeState[playerId];
var currentOpponentState = currentChallengeState[opponentId];

var fields = [
    "hasTurn",
    "manaCurrent",
    "manaMax",
    "health",
    "armor",
    "field",
    "handSize",
    "deckSize",
];
var playerFields = [
    "hand",
];
var opponentFields = [
    
];

var playerState = {};
var opponentState = {};
fields.forEach(function(field) {
    playerState[field] = currentPlayerState[field];
    opponentState[field] = currentOpponentState[field];
});
playerFields.forEach(function(field) {
    playerState[field] = currentPlayerState[field];
});
opponentFields.forEach(function(field) {
    opponentState[field] = currentOpponentState[field];
});

challenge.setScriptData("playerState", playerState);
challenge.setScriptData("opponentState", opponentState);
