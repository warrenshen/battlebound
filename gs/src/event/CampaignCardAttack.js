// ====================================================================================================
//
// Cloud Code for CampaignCardAttack, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeStateModule");
require("ChallengeCardAttackCardModule");

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

var attributes;

if (attributesJson.targetId) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (!attributes.targetId) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

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

handleChallengeCardAttackCard(challengeStateData, playerId, cardId, attributes);

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
setScriptSuccess();
