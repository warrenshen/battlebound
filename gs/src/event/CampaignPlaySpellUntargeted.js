// ====================================================================================================
//
// Cloud Code for CampaignPlaySpellUntargeted, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("CampaignEventPrefix");
require("ChallengePlaySpellUntargetedModule");

const cardId = Spark.getData().cardId;

handleChallengePlaySpellUntargeted(challengeStateData, playerId, cardId);

campaignEventSuffix(challengeStateData);
setScriptSuccess();
