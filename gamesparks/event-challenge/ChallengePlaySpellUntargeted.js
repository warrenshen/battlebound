// ====================================================================================================
//
// Cloud Code for ChallengePlaySpellUntargeted, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("CardAbilitiesModule");
require("AttackModule");
require("ChallengeMovesModule");

const cardId = Spark.getData().cardId;

const challengeState = challengeStateData.current;

const playerState = challengeState[playerId];
if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
    setScriptError("Player state is not in normal mode.");
}

const playerManaCurrent = playerState.manaCurrent;
const playerHand = playerState.hand;
const playerField = playerState.field;

const opponentState = challengeState[opponentId];
const opponentField = opponentState.field;

// Find index of card played in hand.
const handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
if (handIndex < 0) {
    setScriptError("Invalid cardId parameter");
}

const playedCard = playerHand[handIndex];

if (playedCard.cost > playerManaCurrent) {
    setScriptError("Card mana cost exceeds player's current mana.");
} else {
    playerState.manaCurrent -= playedCard.cost;
    if (!Number.isInteger(playerState.manaCurrent)) {
        setScriptError("Player mana current is no longer an int.");
    }
}

if (playedCard.category !== CARD_CATEGORY_SPELL) {
    setScriptError("Invalid card category - must be spell category.");
}

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];
challengeStateData.moveTakenThisTurn = 1;

var move = {
    playerId: playerId,
    category: MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
    attributes: {
        card: playedCard,
        cardId: cardId,
        handIndex: handIndex,
    },
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];

if (playedCard.name === "Blizzard") {
    // For now, only spell that exists is to damage all enemy cards by 1.
    opponentField.forEach(function(card) { damageCard(card, 2) });
} else if (playedCard.name === "Greedy Fingers") {
    for (var i = 0; i < 2; i += 1) {
        move = drawCardForPlayer(playerId, playerState);
        challengeStateData.moves.push(move);
        challengeStateData.lastMoves = [move];
    }
} else if (playedCard.name === "Grave-digging") {
    const moves = challengeState.moves;
    for (var i = 0; i < moves.length; i += 1) {
        var currentMove = moves[i];
        if (
            currentMove.playerId === playerId &&
            currentMove.card &&
            currentMove.card.playerId === playerId &&
            currentMove.card.health <= 0
        ) {
            
        }
    }
} else {
    setScriptError("Unrecognized spell card name.");
}

const filterDeadResponse = filterDeadCardsFromFields(playerField, opponentField);
playerState.field = filterDeadResponse[0];
opponentState.field = filterDeadResponse[1];

// Remove played card from hand.
const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
playerState.hand = newHand;
    
require("PersistChallengeStateModule");

setScriptSuccess();
