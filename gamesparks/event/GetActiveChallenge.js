// ====================================================================================================
//
// Cloud Code for GetActiveChallenge, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeStateModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const activeChallengeId = player.getPrivateData("activeChallengeId");

if (activeChallengeId) {
    const challenge = getChallengeStateForPlayer(playerId, activeChallengeId);
    if (challenge.getRunState() !== "RUNNING") {
        player.removePrivateData("activeChallengeId");
        setScriptError("Challenge is already over.")
    }
} else {
    setScriptError("Player does not have an active challenge.")
}

setScriptSuccess();
