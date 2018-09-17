// ====================================================================================================
//
// Cloud Code for CampaignEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeStateModule");
require("ChallengeEndTurnModule");
require("ChallengePlayCardModule");
require("ChallengeCardAttackCardModule");

// This is secure because the only thing a malicious actor
// can do is send end turn's on time expired for themselves -
// which would only be detrimental to themselves.
const isExpired = Spark.getData().isExpired;

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const API = Spark.getGameDataService();
var dexDataItem = API.getItem("Dex", playerId).document();
if (dexDataItem === null) {
    setScriptError("Player does not have a Dex.");
}

const dexData = dexDataItem.getData();
const challengeStateData = dexData.campaign;
const challengeId = "CAMPAIGN";

// Note that the call below must be before the `challenge.consumeTurn()` call.
// TODO: Be careful with challengeId!
// cancelScheduledTimeEvents(challengeId, playerId);

// Used to determine whether to send
// time expiring messages for opponent.
var isChallengeOver = false;

const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

// If this is an end turn request from time expired and no move
// has been taken this turn, increment player's expired streak.
if (isExpired && challengeStateData.moveTakenThisTurn === 0) {
    challengeStateData.expiredStreakByPlayerId[playerId] += 1;
    
    // If expired streak is greater than 2, auto-surrender player.
    if (challengeStateData.expiredStreakByPlayerId[playerId] > 2) {
        // Reset `lastMoves` attribute in ChallengeState.
        challengeStateData.lastMoves = [];
    
        var move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SURRENDER_BY_EXPIRE,
        };
        addChallengeMove(challengeStateData, move);

        winChallenge(challengeStateData, opponentId);
        isChallengeOver = true;
    }
} else {
    challengeStateData.expiredStreakByPlayerId[playerId] = 0;
}

function handleCampaignBotTurn(challengeStateData, playerId) {
    const playerState = challengeStateData.current[playerId];
    // Opponent = campaign bot.
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    
    var done = false;
    var fieldIndex;
    while (opponentState.manaCurrent > 0 && !done) {
        fieldIndex = -1;
        [2, 3, 1, 4, 0, 5].forEach(function(index) {
            if (fieldIndex < 0 && opponentState.field[index].id === "EMPTY") {
                fieldIndex = index;
            }
        });
        if (fieldIndex < 0) {
            done = true;
        } else {
            var cardId = null;
            opponentState.hand.forEach(function(handCard) {
                if (handCard.category === 0 && handCard.cost <= opponentState.manaCurrent) {
                    cardId = handCard.id;
                }
            });
            if (cardId == null) {
                done = true;
            } else {
                handleChallengePlayCard(challengeStateData, opponentId, cardId, { fieldIndex: fieldIndex });
            }
        }
    }
    
    done = false;
    while (!done) {
        var attackMade = false;
        var targetId = null;
        
        var tauntTargets = playerState.field.filter(function(fieldCard) {
            return fieldCard.id != "EMPTY" && fieldCard.abilities.indexOf(1) >= 0;
        });
        if (tauntTargets.length > 0) {
            targetId = tauntTargets[0].id;
        } else {
            var normalTargets = playerState.field.filter(function(fieldCard) {
                return fieldCard.id != "EMPTY";
            });
            if (normalTargets.length > 0) {
                targetId = normalTargets[0].id;
            }
        }
        
        if (targetId == null) {
            targetId = "TARGET_ID_FACE";
        }
        
        opponentState.field.forEach(function(fieldCard) {
            if (!attackMade && fieldCard.id != "EMPTY" && fieldCard.canAttack > 0) {
                handleChallengeCardAttackCard(
                    challengeStateData,
                    opponentId,
                    fieldCard.id,
                    {
                        fieldId: playerId,
                        targetId: targetId,
                    }
                );
                attackMade = true;
            }
        });
        
        if (!attackMade) {
            done = true;
        }
    }
    
    handleChallengeEndTurn(challengeStateData, opponentId);
}

// We only do the following if challenge is not over.
if (!isChallengeOver) {
    challengeStateData.lastMoves = [];
    handleChallengeEndTurn(challengeStateData, playerId);
    handleCampaignBotTurn(challengeStateData, playerId);
    // startTurnTimeEvents(challengeId, opponentId);
}

if (challengeStateData.nonce == null) {
    challengeStateData.nonce = 0;
} else {
    challengeStateData.nonce += 1;
}
const error = dexDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

setChallengeStateForPlayer(playerId, challengeStateData);
setScriptSuccess();
