// ====================================================================================================
//
// Cloud Code for ChallengePlayMulligan, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeStateModule");
require("CancelScheduledTimeEventsModule");
require("ChallengePlayMulliganModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeId;

if (challengeId.length <= 0) {
    setScriptError("Invalid parameter - challengeId cannot be empty.");
}

const challenge = Spark.getChallenge(challengeId);
if (challenge == null) {
    setScriptError("Invalid challenge ID.");
}

// This lock call MUST be before the `challenge.getPrivateData` call below.
Spark.lockKey(challengeId, 3000);

if (challenge.getRunState() != "RUNNING") {
    setScriptErrorWithUnlockKey(challengeId, "Challenge is not running.");
}

const challengeStateData = challenge.getPrivateData("data");
const cardIds = Spark.getData().cardIds;

handleChallengePlayMulligan(challengeStateData, playerId, cardIds);

require("PersistChallengeStateModule");

Spark.unlockKeyFully(challengeId);

if (challengeStateData.mode === CHALLENGE_STATE_MODE_NORMAL) {
    cancelMulliganTimeEvents(challengeId);
}

const playerResponse = getChallengeStateForPlayerNoSet(playerId, challengeStateData);
const playerMessage = Spark.message("ChallengePlayMulliganMessage");
const playerMessageData = {};

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

const opponentResponse = getChallengeStateForPlayerNoSet(opponentId, challengeStateData);
const opponentMessage = Spark.message("ChallengePlayMulliganMessage");
const opponentMessageData = {};

opponentMessageData.challengeId = challengeId;
opponentMessageData.nonce = opponentResponse.nonce;
opponentMessageData.playerState = opponentResponse.playerState;
opponentMessageData.opponentState = opponentResponse.opponentState;
opponentMessageData.newMoves = opponentResponse.newMoves;
opponentMessageData.moveCount = opponentResponse.moveCount;
opponentMessageData.spawnCount = opponentResponse.spawnCount;

opponentMessage.setMessageData(opponentMessageData);
opponentMessage.setPlayerIds([opponentId]);
opponentMessage.send();

setScriptSuccess();
