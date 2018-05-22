// ====================================================================================================
//
// Cloud Code for ChallengeStartedMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// GLOBAL
require("DeckModule");

var API = Spark.getGameDataService();

var challengeId = Spark.getData().challenge.challengeId;
var challenge = Spark.getChallenge(challengeId);

var challengerId = challenge.getChallengerId();
var challengedId = challenge.getChallengedPlayerIds()[0];

// Get challenger player deck for battle.
var challengerCardIds = getActiveDeckByPlayerId(challengerId);
var challengerDeck = getDeckByCardIds(challengerCardIds);

// Get challenged player deck for battle.
var challengedCardIds = getActiveDeckByPlayerId(challengedId);
var challengedDeck = getDeckByCardIds(challengedCardIds);

var challengeStateDataItem = API.createItem("ChallengeState", challengeId);
var challengeStateData = challengeStateDataItem.getData();
var currentChallengeState = challengeStateData.current = {};
challengeStateData.events = [];

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
    challengerData.activeTurn = true;
    challengedData.activeTurn = false;
} else {
    challengerData.activeTurn = false;
    challengedData.activeTurn = true;
}

currentChallengeState[challengerId] = challengerData;
currentChallengeState[challengedId] = challengedData;

var error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}
