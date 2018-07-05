// ====================================================================================================
//
// Challenge event for when player uses a card on its field to attack a card on opponent's field
// or the opponent's "face" directly. Without any special effects, in the card vs card situation
// the attacking card takes X damage and the defending card takes Y damage - where X is the
// defending card's attack and Y is the attacking card's attack. Any card whose health drop to
// less than equal to zero is taken off the field. Without any special effects, in the card vs
// face situation the opponent's health is decreased by the attacking card's attack and the
// attacking card is unaffected.
//
// Attributes:
// - targetId: card ID of card to attack or TARGET_ID_FACE
// - fieldId: player's ID => player's field (friendly fire), opponent's ID => opponent's field
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("CardAbilitiesModule");
require("AttackModule");
require("ChallengeMovesModule");

const TARGET_ID_FACE = "TARGET_ID_FACE";

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

var attributes;

if (attributesJson.targetId) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (!attributes.targetId) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

const fieldId = attributes.fieldId;
const targetId = attributes.targetId;

const challengeState = challengeStateData.current;

const playerState = challengeState[playerId];
if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
    setScriptError("Player state is not in normal mode.");
}

const playerField = playerState.field;

const opponentState = challengeState[opponentId];
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

var move = {
    playerId: playerId,
    category: MOVE_CATEGORY_CARD_ATTACK,
    attributes: {
        cardId: cardId,
        fieldId: fieldId,
        targetId: targetId,
    },
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];

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
        
    if (targetId === TARGET_ID_FACE) {
        damageFace(opponentState, attackingCard.attack);

        if (opponentState.health <= 0) {
            opponentState.health = 0;
            challenge.winChallenge(Spark.getPlayer());
        }
    } else {
        // Find index of defending card in opponent field.
        defendingIndex = opponentField.findIndex(function(card) { return card.id === targetId });
        if (defendingIndex < 0) {
            setScriptError("Invalid targetId parameter - card does not exist.");
        }
        
        defendingCard = opponentField[defendingIndex];
        
        attackingDamageDone = damageCard(defendingCard, attackingCard.attack);
        defendingDamageDone = damageCard(attackingCard, defendingCard.attack);
    
        // LIFESTEAL PROCESSING BEGIN //    
        if (attackingCard.abilities.indexOf(CARD_ABILITY_LIFE_STEAL) >= 0) {
            healFace(playerState, attackingDamageDone);
        }
        if (defendingCard.abilities.indexOf(CARD_ABILITY_LIFE_STEAL) >= 0) {
            healFace(opponentState, defendingDamageDone);
        }
        // LIFESTEAL PROCESSING END //
        
        filterDeadResponse = filterDeadCardsFromFields(playerField, opponentField);
        playerState.field = filterDeadResponse[0];
        opponentState.field = filterDeadResponse[1];
        
        // DEATHRATTLE PROCESSING BEGIN //
        playerDeadCards = filterDeadResponse[2];
        opponentDeadCards = filterDeadResponse[3];
        
        playerDeadCards.forEach(function(card) {
            if (card.abilities.indexOf(CARD_ABILITY_DEATH_RATTLE_DRAW_CARD) >= 0) {
                move = drawCardForPlayer(playerId, playerState);
                challengeStateData.moves.push(move);
                challengeStateData.lastMoves.push(move);
            } else if (card.abilities.indexOf(CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY) >= 0) {
                damageFace(opponentState, 20);
            }
        });
        opponentDeadCards.forEach(function(card) {
            if (card.abilities.indexOf(CARD_ABILITY_DEATH_RATTLE_DRAW_CARD) >= 0) {
                move = drawCardForPlayer(opponentId, opponentState);
                challengeStateData.moves.push(move);
                challengeStateData.lastMoves.push(move);
            } else if (card.abilities.indexOf(CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY) >= 0) {
                damageFace(playerState, 20);
            }
        });
        // DEATHRATTLE PROCESSING END //
    }
}

require("PersistChallengeStateModule");

setScriptSuccess();
