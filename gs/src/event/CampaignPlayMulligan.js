// ====================================================================================================
//
// Cloud Code for CampaignPlayMulligan, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeStateModule");
require("ChallengePlayMulliganModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const API = Spark.getGameDataService();
var dexDataItem = API.getItem("Dex", playerId).document();
if (dexDataItem === null) {
    setScriptError("Player does not have a Dex.");
}

const dexData = dexDataItem.getData();
const challengeStateData = dexData.campaign;
const challengeId = "CAMPAIGN";

// const challengeId = Spark.getData().challengeId;
// if (challenge.getRunState() != "RUNNING") {
//     setScriptErrorWithUnlockKey(challengeId, "Challenge is not running.");
// }

const cardIds = Spark.getData().cardIds;

handleChallengePlayMulligan(challengeStateData, playerId, cardIds);

var playerResponse = getChallengeStateForPlayerNoSet(playerId, challengeStateData);
var playerMessage = Spark.message("ChallengePlayMulliganMessage");
var playerMessageData = {};

playerMessageData.challengeId = challengeId;
playerMessageData.nonce = playerResponse.nonce;
playerMessageData.playerState = playerResponse.playerState;
playerMessageData.opponentState = playerResponse.opponentState;
playerMessageData.newMoves = playerResponse.newMoves;
playerMessageData.moveCount = playerResponse.moveCount;
playerMessageData.spawnCount = playerResponse.spawnCount;

playerMessage.setMessageData(playerMessageData);
playerMessage.setPlayerIds([playerId]);
playerMessage.send();

const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

handleChallengePlayMulligan(challengeStateData, opponentId, []);

if (challengeStateData.nonce == null) {
    challengeStateData.nonce = 0;
} else {
    challengeStateData.nonce += 1;
}
error = dexDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

// cancelMulliganTimeEvents(challengeId);

const playerState = challengeStateData.current[playerId];
const opponentState = challengeStateData.current[opponentId];

if (playerState.hasTurn === 1) {
    // startTurnTimeEvents(challengeId, playerId);
} else if (opponentState.hasTurn === 1) {
    // startTurnTimeEvents(challengeId, opponentId);
} else {
    setScriptError("Neither player has turn after mulligan.");
}

playerResponse = getChallengeStateForPlayerNoSet(playerId, challengeStateData);
playerMessage = Spark.message("ChallengePlayMulliganMessage");
playerMessageData = {};

playerMessageData.challengeId = challengeId;
playerMessageData.nonce = playerResponse.nonce;
playerMessageData.playerState = playerResponse.playerState;
playerMessageData.opponentState = playerResponse.opponentState;
playerMessageData.newMoves = playerResponse.newMoves;
playerMessageData.moveCount = playerResponse.moveCount;
playerMessageData.spawnCount = playerResponse.spawnCount;

playerMessage.setMessageData(playerMessageData);
playerMessage.setPlayerIds([playerId]);
playerMessage.send();

setScriptSuccess();
