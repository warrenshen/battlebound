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
 
const API = Spark.getGameDataService();

const playerId = Spark.getPlayer().getPlayerId();
const challengeId = Spark.getData().challenge.challengeId;
const challenge = Spark.getChallenge(challengeId);

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    Spark.setScriptError("ERROR", "ChallengeState does not exist.");
    Spark.exit();
}

const challengeStateData = challengeStateDataItem.getData();
const challengeState = challengeStateData.current;

const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

const playerState = challengeState[playerId];
const opponentState = challengeState[opponentId];

const fields = [
    "hasTurn",
    "manaCurrent",
    "manaMax",
    "health",
    "armor",
    "field",
    "handSize",
    "deckSize",
];
const playerFields = [
    "hand",
];

const filteredPlayerState = {};
const filteredOpponentState = {};
fields.forEach(function(field) {
    filteredPlayerState[field] = playerState[field];
    filteredOpponentState[field] = opponentState[field];
});
playerFields.forEach(function(field) {
    filteredPlayerState[field] = playerState[field];
});

const lastMoves = challengeStateData.lastMoves || [];
const opponentMoves = lastMoves.filter(function(move) { return move.playerId === opponentId });

if (opponentMoves.length) {
    Spark.setScriptData("opponentMoves", opponentMoves);
} else {
    Spark.removeScriptData("opponentMoves");
}

Spark.setScriptData("playerState", filteredPlayerState);
Spark.setScriptData("opponentState", filteredOpponentState);
Spark.setScriptData("nonce", challengeStateData.nonce);
