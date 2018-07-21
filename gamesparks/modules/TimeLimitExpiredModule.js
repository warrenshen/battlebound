// ====================================================================================================
//
// This module ends the turn of a player whose turn time limit has expired when
// invoked by the Spark scheduler by sending a server-created ChallengeEndTurn event.
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeMovesModule");

const category = Spark.getData().category;
const challengeId = Spark.getData().challengeId;

var request;
var response;

if (category === TIME_LIMIT_CATEGORY_NORMAL) {
    const hasTurnPlayerId = Spark.getData().hasTurnPlayerId;
    
    request = new SparkRequests.LogChallengeEventRequest();
    request.eventKey = "ChallengeEndTurn";
    request.challengeInstanceId = challengeId;
    request.isExpired = 1;
    
    response = request.ExecuteAs(hasTurnPlayerId);
} else if (category === TIME_LIMIT_CATEGORY_MULLIGAN) {
    const challenge = Spark.getChallenge(challengeId);
    const challengeStateData = challenge.getPrivateData("data");
    
    const playerIds = Spark.getData().playerIds;
    playerIds.forEach(function(playerId) {
        var playerState = challengeStateData.current[playerId];
        if (playerState.mode === PLAYER_STATE_MODE_MULLIGAN) {
            request = new SparkRequests.LogEventRequest();
            request.eventKey = "ChallengePlayMulligan";
            request.challengeId = challengeId;
            request.cardIds = [];
            
            response = request.ExecuteAs(playerId);
        }
    });
} else {
    setScriptError("Unknown time limit expired category.");
}
