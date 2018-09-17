// ====================================================================================================
//
// Cloud Code for ChallengeEndTurnModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeCardModule");
require("ChallengeEffectsModule");

/*
 * @param bool shouldReset - whether to reset challenge state last moves
 */
function handleChallengeEndTurn(challengeStateData, playerId) {
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    
    // PLAYER STATE UPDATES //
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode != PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }

    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_END_TURN,
    };
    addChallengeMove(challengeStateData, move);

    if (playerState.hasTurn == null || playerState.turnCount == null) {
        setScriptError("Player state has invalid has turn or turn count.");
    }
    
    playerState.hasTurn = 0;
    playerState.turnCount += 1;
        
    const playerField = playerState.field;
    // Perform start of turn events for cards on field.
    for (var i = 0; i < playerField.length; i += 1) {
        var fieldCard = playerField[i];
        
        if (fieldCard.id === "EMPTY") {
            continue;
        }
        
        if (fieldCard.health <= 0) {
            setScriptError("Creature on opponent field that is dead at end turn.");
        }
        
        if (fieldCard.isFrozen > 0) {
            fieldCard.isFrozen -= 1;
        }
    }
    
    processEndTurn(challengeStateData, playerId);

    // OPPONENT STATE UPDATES //
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    opponentState.hasTurn = 1;

    // Increase opponent's max mana by 1 if:
    // - This is not player's first turn.
    // - It's not max'ed out.
    if (
        opponentState.turnCount > 0 &&
        opponentState.manaMax < 100
    ) {
        opponentState.manaMax += 10;
    }
    opponentState.manaCurrent = opponentState.manaMax;

    processStartTurn(challengeStateData, opponentId);
    
    // Perform start of turn events for cards on field.
    for (var i = 0; i < opponentField.length; i += 1) {
        var fieldCard = opponentField[i];
        
        if (fieldCard.id === "EMPTY") {
            continue;
        }
        
        if (fieldCard.health <= 0) {
            setScriptError("Creature on opponent field that is dead at start turn.");
        }
        
        if (hasCardAbilityOrBuff(fieldCard, CARD_ABILITY_DOUBLE_STRIKE)) {
            fieldCard.canAttack = 2;
        } else {
            fieldCard.canAttack = 1;
        }
    }
    
    // Player with next turn should start with no move taken.
    challengeStateData.moveTakenThisTurn = 0;
}
