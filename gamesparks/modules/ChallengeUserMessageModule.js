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
require("ChallengeMovesModule");

const API = Spark.getGameDataService();

const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challenge.challengeId;
const challenge = Spark.getChallenge(challengeId);

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    setScriptError("ChallengeState does not exist.");
}

const challengeStateData = challengeStateDataItem.getData();
const challengeState = challengeStateData.current;

const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

const playerState = challengeState[playerId];
const opponentState = challengeState[opponentId];

const playerExpiredStreak = challengeStateData.expiredStreakByPlayerId[playerId];
const opponentExpiredStreak = challengeStateData.expiredStreakByPlayerId[opponentId];

const fields = [
    "hasTurn",
    "manaCurrent",
    "manaMax",
    "health",
    "armor",
    "field",
    "hand",
    "deckSize",
];
const playerFields = [
    // "hand",
];

function obfuscateOpponentHand(hand) {
    return hand.map(function(card) {
        return { id: "HIDDEN" };
    });
}

const filteredPlayerState = {};
const filteredOpponentState = {};
fields.forEach(function(field) {
    filteredPlayerState[field] = playerState[field];
    
    if (field === "hand") {
        filteredOpponentState[field] = obfuscateOpponentHand(opponentState[field]);
    } else {
        filteredOpponentState[field] = opponentState[field];
    }
});
playerFields.forEach(function(field) {
    filteredPlayerState[field] = playerState[field];
});

filteredPlayerState.id = playerId;
filteredOpponentState.id = opponentId;
filteredPlayerState.expiredStreak = playerExpiredStreak;
filteredOpponentState.expiredStreak = opponentExpiredStreak;

const lastMoves = challengeStateData.lastMoves || [];
const obfuscatedMoves = lastMoves.map(function(move) {
    if (move.playerId === opponentId && move.category === MOVE_CATEGORY_DRAW_CARD) {
        return {
            playerId: move.playerId,
            category: move.category,
            attributes: {
                card: { id: "HIDDEN" },
            },
        };
    } else {
        return move;
    }
});
const newMoves = obfuscatedMoves.filter(function(move) {
    if (move === null) {
        setScriptError("Move is null - did you remember to set move to something?");
    }
    
    return move.playerId == opponentId || move.category == MOVE_CATEGORY_DRAW_CARD;
});

Spark.setScriptData("newMoves", newMoves);

Spark.setScriptData("challengeId", challengeId);
Spark.setScriptData("nonce", challengeStateData.nonce);
Spark.setScriptData("playerState", filteredPlayerState);
Spark.setScriptData("opponentState", filteredOpponentState);
