// ====================================================================================================
//
// Challenge event for when player uses a card in its hand. This can be to move
// a card from the player's hand onto the field or to use the spell power of a
// card from the player's hand (which discards the card afterwards).
//
// Card categories:
// - 0: minion (play on field)
// - 1: spell (use and discard)
// - 2: structure (play on field)
// - 3: weapon (use and discard)
//
// Security concerns:
// - Is the Card played valid (attributes are not changed)?
//   The Card is stored on server-side and cannot be tampered with.
// - Does the player own the Card played?
//   It could only be in the player's hand (stored server side) if so.
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("AttackModule");
require("ChallengeMovesModule");
require("ChallengeEffectsModule");

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

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

var attributes;

if (attributesJson.fieldIndex != null) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (attributes.fieldIndex == null) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

if (playedCard.category !== CARD_CATEGORY_MINION) {
    setScriptError("Invalid card category - must be minion category.");
}
// Index at which to play card.
const fieldIndex = attributes.fieldIndex;

// Ensure that index to play card at is valid.
if (fieldIndex < 0 || fieldIndex > 5) {
    setScriptError("Invalid fieldIndex parameter.");
}

if (playerField[fieldIndex].id !== "EMPTY") {
    setScriptError("Invalid fieldIndex parameter - card exists at fieldIndex.");
}

if (!Array.isArray(playedCard.abilities)) {
    playedCard.abilities = [];
}
if (!Array.isArray(playedCard.buffs)) {
    playedCard.buffs = [];    
}

if (playedCard.abilities.indexOf(CARD_ABILITY_CHARGE) >= 0) {
    playedCard.canAttack = 1;
} else {
    playedCard.canAttack = 0;
}
playedCard.isFrozen = 0;
playedCard.isSilenced = 0;

const spawnRank = getNewSpawnRank(challengeStateData);
playedCard.spawnRank = spawnRank;

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];
challengeStateData.moveTakenThisTurn = 1;

var move = {
    playerId: playerId,
    category: MOVE_CATEGORY_PLAY_MINION,
    attributes: {
        card: playedCard,
        cardId: cardId,
        fieldIndex: fieldIndex,
        handIndex: handIndex,
    },
};
addChallengeMove(challengeStateData, move);

// Remove played card from hand.
const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
playerState.hand = newHand;

// Play card onto field.
playerField[fieldIndex] = playedCard;

processCreaturePlay(challengeStateData, playerId, cardId);

// if (playedCard.abilities.indexOf(CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN) >= 0) {
//     // Iterate through cards already on field and grant them buff(s).
//     playerField.forEach(function(card) {
//         if (card.category === CARD_CATEGORY_MINION) {
//             card.attack += 10;
//             card.buffs.push({
//                 category: BUFF_CATEGORY_INCREMENT_ATTACK,
//                 granterId: playedCard.id,
//                 attack: 1,
//                 abilities: [],
//             });
//         }
//     });
// }

// Iterate through cards already on field and grant played card buff(s).
// playerField.forEach(function(fieldCard) {
//     if (fieldCard.abilities && fieldCard.abilities.indexOf(CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN) >= 0) {
//         playedCard.attack += 1;
//         playedCard.buffs.push({
//             granterId: fieldCard.id,
//             attack: 1,
//             abilities: [],
//         });
//     }
// });
    
require("PersistChallengeStateModule");

setScriptSuccess();
