// ====================================================================================================
//
// Cloud Code for ChallengeEndTurnModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("CardAbilitiesModule");
require("CancelScheduledTimeEventsModule");
require("ChallengeMovesModule");
require("AttackModule");
require("ChallengeEffectsModule");

function handleChallengeEndTurnEvent(challengeStateData, playerId) {
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    
    // PLAYER STATE UPDATES //
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }

    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    
    var move = {
        playerId: playerId,
        category: MOVE_CATEGORY_END_TURN,
    };
    addChallengeMove(challengeStateData, move);

    playerState.hasTurn = 0;    
    challengeStateData.turnCountByPlayerId[playerId] += 1;
    
    processEndTurn(challengeStateData, playerId);

    // OPPONENT STATE UPDATES //
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    opponentState.hasTurn = 1;
        
    // Increase opponent's max mana by 1 if it's not max'ed out.
    if (opponentState.manaMax < 100) {
        opponentState.manaMax += 10;
    }
    opponentState.manaCurrent = opponentState.manaMax;
    
    // Draw a card for opponent to start its turn.
    move = drawCardForPlayer(opponentId, opponentState);
    addChallengeMove(challengeStateData, move);
    
    processStartTurn(challengeStateData, opponentId);
    
    // Perform start of turn events for cards on field.
    for (var i = 0; i < opponentField.length; i += 1) {
        var fieldCard = opponentField[i];
        
        if (fieldCard.id === "EMPTY") {
            continue;
        }
        
        if (fieldCard.health <= 0) {
            continue;
        }
        
        // TODO: maybe should not set to 1 for all cards.
        fieldCard.canAttack = 1;
        
        if (fieldCard.isFrozen > 0) {
            fieldCard.isFrozen -= 1;
        }
        
        // fieldCard.buffs.forEach(function(buff) {
        //     if (buff.category === BUFF_CATEGORY_UNSTABLE_POWER) {
        //         fieldCard.health = 0;
        //     }
        // });
    }
    
    // Player with next turn should start with no move taken.
    challengeStateData.moveTakenThisTurn = 0;
}
