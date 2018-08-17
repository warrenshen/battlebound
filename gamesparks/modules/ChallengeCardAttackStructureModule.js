// ====================================================================================================
//
// Cloud Code for ChallengeCardAttackStructureModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeCardModule");
require("ChallengeEffectsModule");

function handleChallengeCardAttackStructure(challengeStateData, playerId, cardId, attributes) {
    const fieldId = attributes.fieldId;
    const targetId = attributes.targetId;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    if (fieldId != opponentId) {
        setScriptError("Invalid fieldId parameter.");
    }
    
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }
    
    const playerField = playerState.field;
    
    // Find index of attacking card in player field.
    const attackingIndex = playerField.findIndex(function(card) { return card.id === cardId });
    if (attackingIndex < 0) {
        setScriptError("Invalid cardId parameter: " + cardId);
    }
    
    const attackingCard = playerField[attackingIndex];
    // Check if card can actually attack - return error if not and update if so.
    if (attackingCard.canAttack <= 0) {
        setScriptError("Card cannot attack anymore.");
    } else {
        attackingCard.canAttack -= 1;
    }
    
    const opponentState = challengeStateData.current[opponentId];
    const opponentFieldBack = opponentState.fieldBack;
    
    // Find index of defending card in opponent field.
    defendingIndex = opponentFieldBack.findIndex(function(card) { return card.id === targetId });
    if (defendingIndex < 0) {
        setScriptError("Invalid targetId parameter - card does not exist.");
    }
    
    defendingCard = opponentFieldBack[defendingIndex];
    
    defendingIndex = opponentFieldBack.findIndex(function(card) { return card.id === targetId });
    if (defendingIndex < 0) {
        setScriptError("Invalid targetId parameter - card does not exist.");
    }
    
    const defendingFieldCards = _getFieldCardsByPlayerIdAndFieldBackIndex(
        challengeStateData,
        opponentId,
        defendingIndex + 6
    );
    if (defendingFieldCards.length > 0) {
        setScriptError("Invalid targetId parameter - cannot attack structure if creatures are in front of it.");
    }
            
    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    challengeStateData.moveTakenThisTurn = 1;
    
    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_CARD_ATTACK,
        attributes: {
            cardId: cardId,
            fieldId: fieldId,
            targetId: targetId,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    processCardAttackStructure(challengeStateData, playerId, cardId, fieldId, targetId);
}
