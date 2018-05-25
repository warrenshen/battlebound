// ====================================================================================================
//
// Cloud Code for ChallengeWinGame, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const player = Spark.getPlayer();
const challengeId = Spark.getData().challengeInstanceId;
const challenge = Spark.getChallenge(challengeId);

challenge.winChallenge(player);
