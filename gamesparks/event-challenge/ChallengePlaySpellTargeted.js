// ====================================================================================================
//
// Cloud Code for ChallengePlaySpellTargeted, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("ChallengePlaySpellTargetedModule");

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

handlePlaySpellTargeted(challengeStateData, playerId, cardId, attributes);

require("PersistChallengeStateModule");
setScriptSuccess();
