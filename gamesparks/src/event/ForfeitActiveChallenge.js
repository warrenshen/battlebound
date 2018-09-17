// ====================================================================================================
//
// Cloud Code for ForfeitActiveChallenge, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const activeChallengeId = player.getPrivateData("activeChallengeId");

if (activeChallengeId) {
    const request = new SparkRequests.LogEventRequest();
    request.eventKey = "ChallengeSurrender";
    request.challengeId = activeChallengeId;
    const response = request.ExecuteAs(playerId);
}

// Force remove active challenge ID even though it may
// have already been removed by some other code path.
player.removePrivateData("activeChallengeId");

setScriptSuccess();
