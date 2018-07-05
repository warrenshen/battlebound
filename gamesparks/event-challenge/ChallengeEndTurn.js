// ====================================================================================================
//
// Cloud Code for ChallengeEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("CardAbilitiesModule");
require("CancelScheduledTimeEventsModule");
require("ChallengeMovesModule");
require("AttackModule");

// This is secure because the only thing a malicious actor
// can do is send end turn's on time expired for themselves -
// which would only be detrimental to themselves.
const isExpired = Spark.getData().isExpired;

// Note that the call below must be before the `challenge.consumeTurn()` call.
cancelScheduledTimeEvents(challengeId, playerId);

const challengeState = challengeStateData.current;

// PLAYER STATE UPDATES //
const playerState = challengeState[playerId];
if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
    setScriptError("Player state is not in normal mode.");
}

const playerField = playerState.field;

playerState.hasTurn = 0;

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];

var move = {
    playerId: playerId,
    category: MOVE_CATEGORY_END_TURN,
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves.push(move);

challengeStateData.turnCountByPlayerId[playerId] += 1;

// Used to determine whether to send
// time expiring messages for opponent.
var isChallengeOver = false;

// If this is a end turn request from time expired and no move
// has been taken this turn, increment player's expired streak.
if (isExpired && challengeStateData.moveTakenThisTurn === 0) {
    challengeStateData.expiredStreakByPlayerId[playerId] += 1;
    
    // If expired streak is greater than 2, auto-surrender player.
    if (challengeStateData.expiredStreakByPlayerId[playerId] > 2) {
        move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SURRENDER_BY_EXPIRE,
        };
        challengeStateData.moves.push(move);
        challengeStateData.lastMoves.push(move);

        // TODO: log challenge winner in ChallengeState.
        const opponent = Spark.loadPlayer(opponentId);
        challenge.winChallenge(opponent);
        isChallengeOver = true;
    }
} else {
    challengeStateData.expiredStreakByPlayerId[playerId] = 0;
}

if (!isChallengeOver) {
    // MORE PLAYER STATE UPDATES //
    playerField.forEach(function(fieldCard) {
        if (fieldCard.id === "EMPTY" || !fieldCard.abilities) {
            return;
        }
        
        if (fieldCard.abilities.indexOf(CARD_ABILITY_END_TURN_HEAL_TEN) >= 0) {
            healCard(fieldCard, 10);
        } else if (fieldCard.abilities.indexOf(CARD_ABILITY_END_TURN_HEAL_TWENTY) >= 0) {
            healCard(fieldCard, 20);
        } else if (fieldCard.abilities.indexOf(CARD_ABILITY_END_TURN_DRAW_CARD) >= 0) {
            // Draw a card for opponent to start its turn.
            move = drawCardForPlayer(playerId, playerState);
            challengeStateData.moves.push(move);
            challengeStateData.lastMoves.push(move);
        }
    });

    // OPPONENT STATE UPDATES //
    const opponentState = challengeState[opponentId];
    const opponentField = opponentState.field;
    
    opponentState.hasTurn = 1;
        
    // Increase opponent's max mana by 1 if it's not max'ed out.
    if (opponentState.manaMax < 100) {
        opponentState.manaMax += 10;
    }
    opponentState.manaCurrent = opponentState.manaMax;
    
    // Draw a card for opponent to start its turn.
    move = drawCardForPlayer(opponentId, opponentState);
    challengeStateData.moves.push(move);
    challengeStateData.lastMoves.push(move);
    
    // Perform start of turn events for cards on field.
    for (var i = 0; i < opponentField.length; i += 1) {
        const fieldCard = opponentField[i];
        
        if (fieldCard.id === "EMPTY") {
            continue;
        }
        
        // TODO: maybe should not set to 1 for all cards.
        fieldCard.canAttack = 1;
        
        // if (fieldCard.abilities.indexOf(CARD_ABILITY_END_TURN_HEAL_TWENTY) >= 0) {
        //     healCard(fieldCard, 20);
        // } else if (fieldCard.abilities.indexOf(CARD_ABILITY_END_TURN_DRAW_CARD) >= 0) {
        //     // Draw a card for opponent to start its turn.
        //     move = drawCardForPlayer(playerId, playerState);
        //     challengeStateData.moves.push(move);
        //     challengeStateData.lastMoves.push(move);
        // }
        
        if (fieldCard.health <= 0 || !fieldCard.buffs) {
            continue;
        }
        
        fieldCard.buffs.forEach(function(buff) {
            if (buff.category === BUFF_CATEGORY_UNSTABLE_POWER) {
                fieldCard.health = 0;
            }
        });
    }
    
    const filterDeadResponse = filterDeadCardsFromFields(playerField, opponentField);
    playerState.field = filterDeadResponse[0];
    opponentState.field = filterDeadResponse[1];
    
    // Player with next turn should start with no move taken.
    challengeStateData.moveTakenThisTurn = 0;
    
    // Finish player turn (without sending a ChallengeTurnTaken message)
    // since this will already be sent because this is a challenge event.
    challenge.consumeTurn(playerId);
    
    // We only send these time expiring messages if challenge is not over.
    const scheduler = Spark.getScheduler();
    const opponentRunningKey = "TROM" + ":" + challengeId + ":" + opponentId;
    const opponentExpiredKey = "TLEM" + ":" + challengeId + ":" + opponentId;
    const data = {
        challengeId: challengeId,
        opponentId: opponentId,
        hasTurnPlayerId: opponentId,
    };
    // var success;
    // success = scheduler.inSeconds("TimeRunningOutModule", 60, data, opponentRunningKey);
    // success = scheduler.inSeconds("TimeLimitExpiredModule", 75, data, opponentExpiredKey);
}

require("PersistChallengeStateModule");

setScriptSuccess();
