// ====================================================================================================
//
// Cloud Code for ChallengePlayStructure, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("ChallengePlayStructureModule");

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

var attributes;

if (attributesJson.fieldIndex != null) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (attributes.fieldIndex == null) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

handleChallengePlayStructure(challengeStateData, playerId, cardId, attributes);
    
require("PersistChallengeStateModule");

setScriptSuccess();
