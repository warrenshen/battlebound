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
require("InitializeChallengeModule");
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

const challengeStateData = initializeChallengeStateData(
    challengeId,
    challengerId,
    challengerDeck,
    challenger.getDisplayName(),
    challengedId,
    challengedDeck,
    challenged.getDisplayName(),
    Spark.getData().challenge.nextPlayer
);

require("PersistChallengeStateModule");

startMulliganTimeEvents(challengeId, [challengerId, challengedId]);
