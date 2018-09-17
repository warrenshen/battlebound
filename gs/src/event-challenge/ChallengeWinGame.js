// ====================================================================================================
//
// Cloud Code for ChallengeWinGame, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var player = Spark.getPlayer();
var challengeId = Spark.getData().challengeInstanceId;
var challenge = Spark.getChallenge(challengeId);

challenge.winChallenge(player);
