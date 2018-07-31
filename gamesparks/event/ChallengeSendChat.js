// ====================================================================================================
//
// Cloud Code for ChallengeSendChat, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeMovesModule");

const CHAT_ID_GG = 0; // "GG"
const CHAT_ID_REKT = 1; // "REKT"
const CHAT_ID_WOW = 2; // "Wow"
const CHAT_ID_PREPARE_YOURSELF = 3; // "Prepare yourself"
const CHAT_ID_HEART_OF_THE_CARDS = 4; // "Heart of the cards"
const CHAT_ID_GET_GOOD = 5; // "Get good"
const CHAT_ID_WELL_PLAYED = 6; // "Well played"

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const challengeId = Spark.getData().challengeId;
const chatId = Spark.getData().chatId;

if (challengeId.length <= 0) {
    setScriptError("Invalid parameter - challengeId cannot be empty.");
}

const challenge = Spark.getChallenge(challengeId);
if (challenge == null) {
    setScriptError("Invalid challenge ID.");
}

if (challenge.getRunState() != "RUNNING") {
    setScriptError(challengeId, "Challenge is not running.");
}

const challengeStateData = challenge.getPrivateData("data");
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

if (challengeStateData.mode != CHALLENGE_STATE_MODE_NORMAL) {
    setScriptError("Challenge not in normal mode.");
}

const message = Spark.message("ChallengeSendChatMessage");
const messageData = {};

messageData.challengeId = challengeId;
messageData.chatId = chatId;

message.setMessageData(messageData);
message.setPlayerIds([opponentId]);
message.send();

setScriptSuccess();
