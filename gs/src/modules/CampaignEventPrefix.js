// ====================================================================================================
//
// Cloud Code for CampaignEventPrefix, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ChallengeStateModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const API = Spark.getGameDataService();
var dexDataItem = API.getItem("Dex", playerId).document();
if (dexDataItem === null) {
    setScriptError("Player does not have a Dex.");
}

const dexData = dexDataItem.getData();
const challengeStateData = dexData.campaign;
const challengeId = "CAMPAIGN";

function campaignEventSuffix(challengeStateData) {
    if (challengeStateData.nonce == null) {
        challengeStateData.nonce = 0;
    } else {
        challengeStateData.nonce += 1;
    }
    const error = dexDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
    
    setChallengeStateForPlayer(playerId, challengeStateData);
}
