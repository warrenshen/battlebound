// ====================================================================================================
//
// Cloud Code for MatchFoundMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");

var playerId = Spark.getPlayer().getPlayerId();
var matchShortCode = Spark.getData().matchShortCode;
var participants = Spark.getData().participants;

var challengeShortCode;
if (matchShortCode === "CasualMatch") {
    challengeShortCode = "CasualChallenge";
} else if (matchShortCode === "RankedMatch") {
    challengeShortCode = "RankedChallenge";
} else {
    setScriptError("Invalid challenge short code.");
}

// If player is first participant - prevents double send.
if (playerId === participants[0].id) {
    const request = new SparkRequests.CreateChallengeRequest();
    
    const endTimeDate = new Date();
    endTimeDate.setDate(endTimeDate.getDate() + 1)
    const endTimeString = endTimeDate.toISOString();

    request.accessType = "PRIVATE";
    request.challengeShortCode = challengeShortCode;
    request.endTime = endTimeString;
    request.expiryTime = endTimeString;
    request.usersToChallenge = participants[1].id;
    
    request.Send();
}
