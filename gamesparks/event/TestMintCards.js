// ====================================================================================================
//
// Cloud Code for TestMintCards, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("MintCardsModule");

const templateIdToCount = Spark.getData().templateIdToCount;
const mintCount = Spark.getData().mintCount;
const category = Spark.getData().category;

// Turn on test mode.
// GLOBAL_ENVIRONMENT = 1;

const templateIds = mintRandomCards(mintCount, category, templateIdToCount);

Spark.setScriptData("mintCount", mintCount);
Spark.setScriptData("category", category);
Spark.setScriptData("templateIdToCount", templateIdToCount);
Spark.setScriptData("templateIds", templateIds);
setScriptSuccess();
