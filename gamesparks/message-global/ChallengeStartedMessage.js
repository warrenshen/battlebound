// ====================================================================================================
//
// Cloud Code for ChallengeStartedMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// GLOBAL
require("DeckModule");
 
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

const turnCountByPlayerId = {};
turnCountByPlayerId[challengerId] = 0;
turnCountByPlayerId[challengedId] = 0;
challengeStateData.turnCountByPlayerId = turnCountByPlayerId;

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
    challengerData.hasTurn = 1;
    challengedData.hasTurn = 0;
} else {
    challengerData.hasTurn = 0;
    challengedData.hasTurn = 1;
}

challengeState[challengerId] = challengerData;
challengeState[challengedId] = challengedData;

require("PersistChallengeStateModule");
