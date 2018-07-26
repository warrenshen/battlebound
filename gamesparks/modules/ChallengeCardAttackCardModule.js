// ====================================================================================================
//
// Cloud Code for ChallengeCardAttackCardModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeMovesModule");
require("ChallengeEffectsModule");

function handleChallengeCardAttackCard(challengeStateData, playerId, cardId, attributes) {
    const fieldId = attributes.fieldId;
    const targetId = attributes.targetId;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }
    
    const playerField = playerState.field;
    
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    if (fieldId !== playerId && fieldId !== opponentId) {
        setScriptError("Invalid fieldId parameter.");
    }
    
    // Find index of attacking card in player field.
    const attackingIndex = playerField.findIndex(function(card) { return card.id === cardId });
    if (attackingIndex < 0) {
        setScriptError("Invalid cardId parameter.");
    }
    
    const attackingCard = playerField[attackingIndex];
    // Check if card can actually attack - return error if not and update if so.
    if (attackingCard.canAttack <= 0) {
        setScriptError("Card cannot attack anymore.");
    } else {
        attackingCard.canAttack -= 1;
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
    
    var defendingIndex;
    var defendingCard;
    var attackingDamageDone;
    var defendingDamageDone;
    var newPlayerField;
    var newOpponentField;
    var filterDeadResponse;
    
    // Friendly fire.
    if (fieldId === playerId) {
        if (targetId === TARGET_ID_FACE) {
            setScriptError("Invalid targetId parameter - cannot attack self.");
        } else {
            // Find index of defending card in player field.
            defendingIndex = playerField.findIndex(function(card) { return card.id === targetId });
            if (defendingIndex < 0) {
                setScriptError("Invalid targetId parameter - card does not exist.");
            } else if (attackingIndex === defendingIndex) {
                setScriptError("Invalid targetId parameter - cannot attack self.");
            }
            
            defendingCard = playerField[defendingIndex];
            
            attackingDamageDone = damageCard(defendingCard, attackingCard.attack);
            defendingDamageDone = damageCard(attackingCard, defendingCard.attack);
            
            filterDeadResponse = filterDeadCardsFromFields(playerField, opponentField);
            playerState.field = filterDeadResponse[0];
            opponentState.field = filterDeadResponse[1];
        }
    } else {
        const tauntCards = opponentField.filter(function(card) { return card.abilities && card.abilities.indexOf(CARD_ABILITY_TAUNT) >= 0 });
        const tauntIds = tauntCards.map(function(card) { return card.id });
        
        // `targetId` must be of a taunt card if any exist.
        if (tauntIds.length > 0 && tauntIds.indexOf(targetId) < 0) {
            setScriptError("Invalid targetId parameter - taunt cards exist.");
        }
    
        processCreatureAttack(challengeStateData, playerId, cardId, fieldId, targetId);
    }
}
