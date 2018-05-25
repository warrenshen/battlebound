// ====================================================================================================
//
// Cloud Code for ChallengeLostMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================


// if (Spark.getData().challenge.shortCode === "CasualChallenge") {
//     require("ProcessRank");

//     const score = 0; // For winning
//     var currentPlayerRankScore = Spark.getPlayer().getScriptData('rankScore');
//     if (!currentPlayerRankScore) {
//         currentPlayerRankScore = 1200; // Initial ranking score for new players
//         // Hide the rank score for first ten games, need to add a counter.
//     }
//     var opponentPlayerId = Spark.getData().challenge.challenger.id;
//     var opponentPlayerRankScore = Spark.loadPlayer(opponentPlayerId).getScriptData('rankScore');
//     if (!opponentPlayerRankScore) {
//         opponentPlayerRankScore = 1200; // Initial ranking score for new players
//         // Hide the rank score for first ten games, need to add a counter.
//     }
//     var newRankScore = processRank(currentPlayerRankScore, opponentPlayerRankScore, score);
//     Spark.getPlayer().setScriptData('rankScore', newRankScore);
// }

require("ChallengeUserMessageSuffix");
