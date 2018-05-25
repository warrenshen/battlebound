// ====================================================================================================
//
// Cloud Code for ChallengeStartedMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// GLOBAL
require("DeckModule");

/**
 * Card schema: {
 *   id: string,
 *   type: string,
 *   name: string,
 *   level: int,
 *   manaCost: int,
 *   health: int,
 *   attack: int,
 *   canAttack: bool, // Field probably not set until card is played on field.
 * }
 * 
 * Move schema: {
 *   type: string,
 *   attributes: { ... },
 * }
 * 
 * ChallengeState schema: {
 *   nonce: int, // A counter incremented every time the ChallengeState is updated.
 *   current: {
 *     opponentIdByPlayerId: { playerId: opponentId },
 *     [playerIdOne]: {
 *       hasTurn: bool,
 *       turnCount: int,
 *       manaCurrent: int,
 *       manaMax: int,
 *       health: int,
 *       armor: int,
 *       field: [Card, ...],
 *       hand: [Card, ...],
 *       handSize: int,
 *       deck: [Card, ...],
 *       deckSize: int,
 *     },
 *     [playerIdTwo]: ...,
 *   },
 *   moves: [Move, ...], // An array of moves by both players in chronological order.
 *   lastMoves: [Move, ...], // An array of the move(s) in last request of player whose turn it is.
 * }
 **/
 
const API = Spark.getGameDataService();

const challengeId = Spark.getData().challenge.challengeId;
const challenge = Spark.getChallenge(challengeId);

const challengerId = challenge.getChallengerId();
const challengedId = challenge.getChallengedPlayerIds()[0];

// Get challenger player deck for battle.
const challengerDeck = getActiveDeckByPlayerId(challengerId);

// Get challenged player deck for battle.
const challengedDeck = getActiveDeckByPlayerId(challengedId);

const challengeStateDataItem = API.createItem("ChallengeState", challengeId);
const challengeStateData = challengeStateDataItem.getData();
const challengeState = challengeStateData.current = {};
challengeStateData.moves = [];

const opponentIdByPlayerId = {};
opponentIdByPlayerId[challengerId] = challengedId;
opponentIdByPlayerId[challengedId] = challengerId;
challengeStateData.opponentIdByPlayerId = opponentIdByPlayerId;

const challengerData = {
    manaCurrent: 3,
    manaMax: 3,
    health: 30,
    armor: 0,
    field: [],
    hand: [],
    handSize: 0,
    deck: challengerDeck,
    deckSize: challengerDeck.length
};

const challengedData = {
    manaCurrent: 3,
    manaMax: 3,
    health: 30,
    armor: 0,
    field: [],
    hand: [],
    handSize: 0,
    deck: challengedDeck,
    deckSize: challengedDeck.length
};

if (Spark.getData().challenge.nextPlayer === challengerId) {
    challengerData.hasTurn = true;
    challengedData.hasTurn = false;
} else {
    challengerData.hasTurn = false;
    challengedData.hasTurn = true;
}

challengeState[challengerId] = challengerData;
challengeState[challengedId] = challengedData;

require("PersistChallengeStateModule");
