// ====================================================================================================
//
// Cloud Code for ChallengeIssuedMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const challengeId = Spark.getData().challenge.challengeId;

const request = new SparkRequests.AcceptChallengeRequest();
request.challengeInstanceId = challengeId;
request.Send();
