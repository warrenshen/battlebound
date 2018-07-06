// ====================================================================================================
//
// Cloud Code for ChallengeStateModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeMovesModule");

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
    
    return {
        challengeId: challengeId,
        nonce: challengeStateData.nonce,
        playerState: filteredPlayerState,
        opponentState: filteredOpponentState,
        newMoves: newMoves,
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
    
    Spark.setScriptData("challengeId", response.challengeId);
    Spark.setScriptData("nonce", response.nonce);
    Spark.setScriptData("playerState", response.playerState);
    Spark.setScriptData("opponentState", response.opponentState);
    Spark.setScriptData("newMoves", response.newMoves);
    
    return challenge;
}
