// ====================================================================================================
//
// Cloud Code for ChallengePlayMulliganModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeMovesModule");
require("ChallengeEffectsModule");

function handleChallengePlayMulligan(challengeStateData, playerId, cardIds) {
    const challengeId = challengeStateData.id;
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    
    if (!Array.isArray(cardIds)) {
        setScriptErrorWithUnlockKey(challengeId, "Invalid cardIds parameter - must be an array.");
    }
    
    if (challengeStateData.mode != CHALLENGE_STATE_MODE_MULLIGAN) {
        setScriptErrorWithUnlockKey(challengeId, "Challenge state is not in mulligan mode.");
    }
    
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode !== PLAYER_STATE_MODE_MULLIGAN) {
        setScriptErrorWithUnlockKey(challengeId, "Player state is not in mulligan mode.");
    }

    const mulliganCards = playerState.mulliganCards;
    const mulliganCardIds = mulliganCards.map(function(mulliganCard) { return mulliganCard.id });
    cardIds.forEach(function(cardId) {
        if (mulliganCardIds.indexOf(cardId) < 0) {
            setScriptErrorWithUnlockKey(challengeId, "Invalid cardIds parameter - a card ID does not exist in mulligan card IDs.");
        }
    });

    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    
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
    
    var move = {
        playerId: playerId,
        category: MOVE_CATEGORY_PLAY_MULLIGAN,
        attributes: {
            deckCardIndices: deckCardIndices,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    // Draw cards into hand to replaces ones put back in deck.
    mulliganCards.forEach(function(mulliganCard, index) {
        const isDeck = cardIds.indexOf(mulliganCard.id) < 0;
        if (isDeck) {
            const move = drawCardMulliganForPlayer(playerId, playerState, index);
            addChallengeMove(challengeStateData, move);
        }
    });
    
    const opponentState = challengeStateData.current[opponentId];
    
    if (opponentState.mode === PLAYER_STATE_MODE_MULLIGAN_WAITING) {
        opponentState.mode = PLAYER_STATE_MODE_NORMAL;
        playerState.mode = PLAYER_STATE_MODE_NORMAL;
    } else {
        playerState.mode = PLAYER_STATE_MODE_MULLIGAN_WAITING;
    }
    playerState.mulliganCards = [];
    
    if (playerState.mode === PLAYER_STATE_MODE_NORMAL) {
        challengeStateData.mode = CHALLENGE_STATE_MODE_NORMAL;
        
        move = {
            category: MOVE_CATEGORY_FINISH_MULLIGAN,
        };
        addChallengeMove(challengeStateData, move);
    
        // Draw card for starting player.
        if (playerState.hasTurn === 1) {
            processStartTurn(challengeStateData, playerId);
        } else if (opponentState.hasTurn === 1) {
            processStartTurn(challengeStateData, opponentId);
        } else {
            setScriptErrorWithUnlockKey(challengeId, "Neither player has turn after mulligan.");
        }
    }
}
