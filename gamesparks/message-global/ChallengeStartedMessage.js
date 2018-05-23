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
 *   name: string,
 *   level: int,
 *   manaCost: int,
 *   health: int,
 *   attack: int,
 *   canAttack: bool, (probably not set until card is played on field)
 * }
 * 
 * ChallengeState schema: {
 *   current: {
 *     opponentIdByPlayerId: { playerId: opponentId },
 *     [playerIdOne]: {
 *       hasTurn: bool,
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
 * }
 **/
 
var API = Spark.getGameDataService();

var challengeId = Spark.getData().challenge.challengeId;
var challenge = Spark.getChallenge(challengeId);

var challengerId = challenge.getChallengerId();
var challengedId = challenge.getChallengedPlayerIds()[0];

// Get challenger player deck for battle.
var challengerDeck = getActiveDeckByPlayerId(challengerId);

// Get challenged player deck for battle.
var challengedDeck = getActiveDeckByPlayerId(challengedId);

var challengeStateDataItem = API.createItem("ChallengeState", challengeId);
var challengeStateData = challengeStateDataItem.getData();
var currentChallengeState = challengeStateData.current = {};
// challengeStateData.events = [];

var opponentIdByPlayerId = {};
opponentIdByPlayerId[challengerId] = challengedId;
opponentIdByPlayerId[challengedId] = challengerId;
challengeStateData.opponentIdByPlayerId = opponentIdByPlayerId;

var challengerData = {
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

var challengedData = {
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

if (challenge.nextPlayer === challengerId) {
    challengerData.hasTurn = true;
    challengedData.hasTurn = false;
} else {
    challengerData.hasTurn = false;
    challengedData.hasTurn = true;
}

currentChallengeState[challengerId] = challengerData;
currentChallengeState[challengedId] = challengedData;

var error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}
