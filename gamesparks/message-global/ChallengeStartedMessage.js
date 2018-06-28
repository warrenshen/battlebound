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
 
const API = Spark.getGameDataService();

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

const expiredStreakByPlayerId = {};
expiredStreakByPlayerId[challengerId] = 0;
expiredStreakByPlayerId[challengedId] = 0;
challengeStateData.expiredStreakByPlayerId = expiredStreakByPlayerId;

challengeStateData.moveTakenThisTurn = 0;

const challengerDrawCardsResponse = drawCards(challengerDeck, 3);
const challengerHand = challengerDrawCardsResponse[0];
const challengerDeckAfterDraw = challengerDrawCardsResponse[1];
const challengerData = {
    manaCurrent: 3,
    manaMax: 3,
    health: 30,
    armor: 0,
    // GS does not allow array of different types to be persisted, so we use id of "EMPTY" to denote lack of card.
    field: [{ id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }],
    hand: challengerHand,
    deck: challengerDeckAfterDraw,
    deckSize: challengerDeckAfterDraw.length,
};

const challengedDrawCardsResponse = drawCards(challengedDeck, 3);
const challengedHand = challengedDrawCardsResponse[0];
const challengedDeckAfterDraw = challengedDrawCardsResponse[1];
const challengedData = {
    manaCurrent: 3,
    manaMax: 3,
    health: 30,
    armor: 0,
    field: [{ id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }, { id: "EMPTY" }],
    hand: challengedHand,
    deck: challengedDeckAfterDraw,
    deckSize: challengedDeckAfterDraw.length,
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
