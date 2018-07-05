// ====================================================================================================
//
// Cloud Code for ChallengePlayMulligan, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("CardAbilitiesModule");
require("AttackModule");
require("ChallengeMovesModule");
require("ChallengeStateModule");

const API = Spark.getGameDataService();

const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeId;

if (challengeId.length <= 0) {
    setScriptError("Invalid parameter - challengeId cannot be empty.");
}

const challenge = Spark.getChallenge(challengeId);

if (challenge.getRunState() != "RUNNING") {
    setScriptError("Challenge is not running.");
}

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    setScriptError("ChallengeState does not exist.");
}

const challengeStateData = challengeStateDataItem.getData();
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

const cardIds = Spark.getData().cardIds;

if (!Array.isArray(cardIds)) {
    setScriptError("Invalid cardIds parameter - must be an array.");
}

const challengeState = challengeStateData.current;
const playerState = challengeState[playerId];
if (playerState.mode !== PLAYER_STATE_MODE_MULLIGAN) {
    setScriptError("Player state is not in mulligan mode.");
}

const mulliganCards = playerState.mulliganCards;
const mulliganCardIds = mulliganCards.map(function(mulliganCard) { return mulliganCard.id });
cardIds.forEach(function(cardId) {
    if (mulliganCardIds.indexOf(cardId) < 0) {
        setScriptError("Invalid cardIds parameter - a card ID does not exist in mulligan card IDs.");
    }
});

// Indices of cards going back into deck.
const deckCardIndices = [];
mulliganCards.forEach(function(mulliganCard, index) {
    const isDeck = cardIds.indexOf(mulliganCard.id) < 0;
    if (isDeck) {
        deckCardIndices.push(index);
        addCardToPlayerDeck(playerId, playerState, mulliganCard);
    } else {
        addCardToPlayerHand(playerId, playerState, mulliganCard);
    }
});

// Draw cards into hand to replaces ones put back in deck.
mulliganCards.forEach(function(mulliganCard, index) {
    const isDeck = cardIds.indexOf(mulliganCard.id) < 0;
    if (isDeck) {
        drawCardForPlayer(playerId, playerState);
    }
});

const opponentState = challengeState[opponentId];

if (opponentState.mode === PLAYER_STATE_MODE_MULLIGAN_WAITING) {
    opponentState.mode = PLAYER_STATE_MODE_NORMAL;
    playerState.mode = PLAYER_STATE_MODE_NORMAL;
} else {
    playerState.mode = PLAYER_STATE_MODE_MULLIGAN_WAITING;
}
playerState.mulliganCards = [];

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];

const move = {
    playerId: playerId,
    category: MOVE_CATEGORY_PLAY_MULLIGAN,
    attributes: {
        deckCardIndices: deckCardIndices,
    },
};

challengeStateData.moves.push(move);
challengeStateData.lastMoves.push(move);

require("PersistChallengeStateModule");

const playerResponse = getChallengeStateForPlayerNoSet(playerId, challengeStateData);
const playerMessage = Spark.message("ChallengePlayMulliganMessage");
const playerMessageData = {};

playerMessageData.challengeId = playerResponse.challengeId;
playerMessageData.nonce = playerResponse.nonce;
playerMessageData.playerState = playerResponse.playerState;
playerMessageData.opponentState = playerResponse.opponentState;
playerMessageData.newMoves = playerResponse.newMoves;

playerMessage.setMessageData(playerMessageData);
playerMessage.setPlayerIds([playerId]);
playerMessage.send();

const opponentResponse = getChallengeStateForPlayerNoSet(opponentId, challengeStateData);
const opponentMessage = Spark.message("ChallengePlayMulliganMessage");
const opponentMessageData = {};

opponentMessageData.challengeId = opponentResponse.challengeId;
opponentMessageData.nonce = opponentResponse.nonce;
opponentMessageData.playerState = opponentResponse.playerState;
opponentMessageData.opponentState = opponentResponse.opponentState;
opponentMessageData.newMoves = opponentResponse.newMoves;

opponentMessage.setMessageData(opponentMessageData);
opponentMessage.setPlayerIds([opponentId]);
opponentMessage.send();

setScriptSuccess();
