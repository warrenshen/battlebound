// ====================================================================================================
//
// Cloud Code for ChallengeWonMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// USER
//If it's a ranked match, process rank
// use casual match for now

require("ProcessRank");

if (Spark.getData().challenge.shortCode === "CasualChallenge") {

    var playerId1 = Spark.getData().challenge.challenger.id;
    var playerId2 = Spark.getData().challenge.challenged[0].id;

    var winnerPlayerId = Spark.getPlayer().getPlayerId();
    var loserPlayerId = winnerPlayerId != playerId1 ? playerId1 : playerId2;

    processRank(winnerPlayerId, loserPlayerId);
}

require("ChallengeUserMessageSuffix");
