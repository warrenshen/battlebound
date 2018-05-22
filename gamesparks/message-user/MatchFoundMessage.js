// ====================================================================================================
//
// Cloud Code for MatchFoundMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var playerId = Spark.getPlayer().getPlayerId();
var matchShortCode = Spark.getData().matchShortCode;
var participants = Spark.getData().participants;

if (matchShortCode === "CasualMatch") {
    // If player is first participant - prevents double send.
    if (playerId === participants[0].id) {
        var request = new SparkRequests.CreateChallengeRequest();
        
        var endTimeDate = new Date();
        endTimeDate.setDate(endTimeDate.getDate() + 1)
        var endTimeString = endTimeDate.toISOString();

        request.accessType = "PRIVATE";
        request.challengeShortCode = "CasualChallenge";
        request.endTime = endTimeString;
        request.expiryTime = endTimeString;
        request.usersToChallenge = participants[1].id;
        
        request.Send();
    }
}
