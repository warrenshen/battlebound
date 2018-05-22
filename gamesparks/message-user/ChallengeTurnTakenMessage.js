// ====================================================================================================
//
// Cloud Code for ChallengeTurnTakenMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// USER
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
    "manaCurrent",
    "manaMax",
    "health",
    "armor",
    "field",
    "handSize",
    "deckSize"
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
