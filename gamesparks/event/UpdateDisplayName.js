// ====================================================================================================
//
// Cloud Code for UpdateDisplayName, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");

const displayName = Spark.getData().displayName;
if (displayName == null || displayName.length <= 0) {
    setScriptError("Username cannot be empty.");
} else if (displayName.length > 20) {
    setScriptError("Username cannot be more than 20 characters.");
}

const API = Spark.getGameDataService();
const dataQuery = API.S("displayName").eq(displayName);
const dataQueryResult = API.queryItems("PlayerDecks", dataQuery);
const dataQueryResultError = dataQueryResult.error();

if (dataQueryResultError) {
    setScriptError("An unexpected error occurred, please try again later.");
} else {
    const dataQueryCursor = dataQueryResult.cursor();
    if (dataQueryCursor.hasNext()) {
        setScriptError("Username already taken.")
    }
}

const player = Spark.getPlayer();

const request = new SparkRequests.ChangeUserDetailsRequest();
request.displayName = displayName;
const response = request.Send();

const playerId = player.getPlayerId();
const decksDataItem = API.getItem("PlayerDecks", playerId).document();
const decksData = decksDataItem.getData();
decksData.displayName = displayName;
const error = decksDataItem.persistor().persist().error();
if (error) {
    setScriptError("An unexpected error occurred, please try again later.");
}

Spark.setScriptData("displayName", displayName);
setScriptSuccess();
