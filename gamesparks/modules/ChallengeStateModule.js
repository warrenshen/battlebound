// ====================================================================================================
//
// Cloud Code for ChallengeStateModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeMovesModule");

function _getObfuscatedMoves(moves, playerId, opponentId) {
    return moves.map(function(move) {
        if (move === null) {
            setScriptError("Move is null - did you remember to set move to something?");
        }
        if (
            move.playerId === opponentId && 
            (
                move.category === MOVE_CATEGORY_DRAW_CARD ||
                move.category === MOVE_CATEGORY_DRAW_CARD_MULLIGAN
            ) 
        ) {
            return {
                playerId: move.playerId,
                category: move.category,
                rank: move.rank,
                attributes: {
                    card: { id: "HIDDEN" },
                },
            };
        } else {
            return move;
        }
    });
}

/**
 * @param playerId - player ID to return challenge state for
 **/ 
function getChallengeStateForPlayerNoSet(playerId, challengeStateData) {
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    
    const challengeState = challengeStateData.current;
    const playerState = challengeState[playerId];
    const opponentState = challengeState[opponentId];
    
    const playerExpiredStreak = challengeStateData.expiredStreakByPlayerId[playerId];
    const opponentExpiredStreak = challengeStateData.expiredStreakByPlayerId[opponentId];
    
    const fields = [
        "hasTurn",
        "manaCurrent",
        "manaMax",
        "health",
        "healthMax",
        "armor",
        "field",
        "hand",
        "deckSize",
        "cardCount",
        "mode",
        "mulliganCards",
    ];
    const playerFields = [
    ];
    
    function obfuscateOpponentCards(cards) {
        return cards.map(function(card) {
            return { id: "HIDDEN" };
        });
    }
    
    const filteredPlayerState = {};
    const filteredOpponentState = {};
    fields.forEach(function(field) {
        filteredPlayerState[field] = playerState[field];
        
        if (field === "hand" || field === "mulliganCards") {
            filteredOpponentState[field] = obfuscateOpponentCards(opponentState[field]);
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
    const obfuscatedLastMoves = _getObfuscatedMoves(lastMoves, playerId, opponentId);
    // const filteredMoves = obfuscatedMoves.filter(function(move) {
    //     return !(move.playerId !== playerId && move.category === MOVE_CATEGORY_PLAY_MULLIGAN);
    // });
    
    const moves = challengeStateData.moves || [];
    const obfuscatedMoves = _getObfuscatedMoves(moves, playerId, opponentId);
    
    return {
        nonce: challengeStateData.nonce,
        playerState: filteredPlayerState,
        opponentState: filteredOpponentState,
        newMoves: obfuscatedLastMoves,
        moveCount: challengeStateData.moves.length,
        spawnCount: challengeStateData.spawnCount,
        moves: obfuscatedMoves,
    };
}

/**
 * @param playerId - player ID to return challenge state for
 **/ 
function getChallengeStateForPlayer(playerId, challengeId) {
    const challenge = Spark.getChallenge(challengeId);
    if (challenge == null) {
        setScriptError("Challenge does not exist.");
    }

    const challengeStateData = challenge.getPrivateData("data");

    const response = getChallengeStateForPlayerNoSet(playerId, challengeStateData);
    
    Spark.setScriptData("challengeId", challengeId);
    Spark.setScriptData("nonce", response.nonce);
    Spark.setScriptData("playerState", response.playerState);
    Spark.setScriptData("opponentState", response.opponentState);
    Spark.setScriptData("newMoves", response.newMoves);
    Spark.setScriptData("moveCount", response.moveCount);
    Spark.setScriptData("spawnCount", response.spawnCount);
    
    return challenge;
}

/**
 * @param playerId - player ID to return challenge state for
 **/ 
function getChallengeStateForPlayerWithPastMoves(playerId, challengeId) {
    const challenge = Spark.getChallenge(challengeId);
    if (challenge == null) {
        setScriptError("Challenge does not exist.");
    }

    const challengeStateData = challenge.getPrivateData("data");

    const response = getChallengeStateForPlayerNoSet(playerId, challengeStateData);
    
    Spark.setScriptData("challengeId", challengeId);
    Spark.setScriptData("nonce", response.nonce);
    Spark.setScriptData("playerState", response.playerState);
    Spark.setScriptData("opponentState", response.opponentState);
    Spark.setScriptData("newMoves", response.newMoves);
    Spark.setScriptData("moves", response.moves);
    Spark.setScriptData("moveCount", response.moveCount);
    Spark.setScriptData("spawnCount", response.spawnCount);
    
    return challenge;
}
