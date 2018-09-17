// ====================================================================================================
//
// Cloud Code for ChallengeStartedMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// GLOBAL
require("ScriptDataModule");
require("DeckModule");
require("AttackModule");
require("CancelScheduledTimeEventsModule");

const challengeId = Spark.getData().challenge.challengeId;
const challenge = Spark.getChallenge(challengeId);

const challengerId = challenge.getChallengerId();
const challengedId = challenge.getChallengedPlayerIds()[0];

const challenger = Spark.loadPlayer(challengerId);
const challenged = Spark.loadPlayer(challengedId);

challenger.setPrivateData("activeChallengeId", challengeId);
challenged.setPrivateData("activeChallengeId", challengeId);

// Get challenger player deck for battle.
const challengerDeck = getActiveDeckByPlayerId(challengerId);
// Get challenged player deck for battle.
const challengedDeck = getActiveDeckByPlayerId(challengedId);

const challengeStateData = {};
challengeStateData.id = challengeId;
challengeStateData.mode = CHALLENGE_STATE_MODE_MULLIGAN;
challengeStateData.moves = [];
challengeStateData.deadCards = [];
challengeStateData.spawnCount = 0;
challengeStateData.isFinalByPlayerId = {};

const challengeState = challengeStateData.current = {};

const opponentIdByPlayerId = {};
opponentIdByPlayerId[challengerId] = challengedId;
opponentIdByPlayerId[challengedId] = challengerId;
challengeStateData.opponentIdByPlayerId = opponentIdByPlayerId;

const expCardIdsByPlayerId = {}
expCardIdsByPlayerId[challengerId] = [];
expCardIdsByPlayerId[challengedId] = [];
challengeStateData.expCardIdsByPlayerId = expCardIdsByPlayerId;

const expiredStreakByPlayerId = {};
expiredStreakByPlayerId[challengerId] = 0;
expiredStreakByPlayerId[challengedId] = 0;
challengeStateData.expiredStreakByPlayerId = expiredStreakByPlayerId;

challengeStateData.moveTakenThisTurn = 0;

const displayData = challengeStateData.displayData = {};
//

const HEALTH_START = 300;
const MANA_START = 10;
const ARMOR_START = 0;

const challengerData = {
    displayName: challenger.getUserName(),
    turnCount: 0,
    manaCurrent: MANA_START,
    manaMax: MANA_START,
    health: HEALTH_START,
    healthMax: HEALTH_START,
    armor: ARMOR_START,
    // GS does not allow array of different types to be persisted, so we use id of "EMPTY" to denote lack of card.
    field: [{ id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }],
    fieldBack: [{ id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }],
    hand: [],
};

const challengedData = {
    displayName: challenged.getUserName(),
    turnCount: 0,
    manaCurrent: MANA_START,
    manaMax: MANA_START,
    health: HEALTH_START,
    healthMax: HEALTH_START,
    armor: ARMOR_START,
    field: [{ id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }],
    fieldBack: [{ id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }],
    hand: [],
};

if (Spark.getData().challenge.nextPlayer === challengerId) {
    challengerData.hasTurn = 1;
    challengedData.hasTurn = 0;
} else {
    challengerData.hasTurn = 0;
    challengedData.hasTurn = 1;
}

// Challenger
var challengerDrawCardsResponse;
if (challengerData.hasTurn === 1) {
    challengerDrawCardsResponse = drawCards(challengerDeck, 3);
} else {
    challengerDrawCardsResponse = drawCards(challengerDeck, 4);
}

const challengerMulligan = challengerDrawCardsResponse[0];
const challengerDeckAfterDraw = challengerDrawCardsResponse[1];

challengerData.deck = challengerDeckAfterDraw;
challengerData.deckSize = challengerDeckAfterDraw.length;
challengerData.cardCount = challengerDeck.length;
challengerData.mode = PLAYER_STATE_MODE_MULLIGAN;
challengerData.mulliganCards = challengerMulligan;
// --

// Challenged
var challengedDrawCardsResponse;
if (challengedData.hasTurn === 1) {
    challengedDrawCardsResponse = drawCards(challengedDeck, 3);
} else {
    challengedDrawCardsResponse = drawCards(challengedDeck, 4);
}

const challengedMulligan = challengedDrawCardsResponse[0];
const challengedDeckAfterDraw = challengedDrawCardsResponse[1];

challengedData.deck = challengedDeckAfterDraw;
challengedData.deckSize = challengedDeckAfterDraw.length;
challengedData.cardCount = challengedDeck.length;
challengedData.mode = PLAYER_STATE_MODE_MULLIGAN;
challengedData.mulliganCards = challengedMulligan;
// --
    
challengeState[challengerId] = challengerData;
challengeState[challengedId] = challengedData;

require("PersistChallengeStateModule");

startMulliganTimeEvents(challengeId, [challengerId, challengedId]);
