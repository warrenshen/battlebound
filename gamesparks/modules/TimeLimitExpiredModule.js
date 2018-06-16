// ====================================================================================================
//
// This module ends the turn of a player whose turn time limit has expired when
// invoked by the Spark scheduler by sending a server-created ChallengeEndTurn event.
//
// ====================================================================================================
const challengeId = Spark.getData().challengeId;
const hasTurnPlayerId = Spark.getData().hasTurnPlayerId;
    
const request = new SparkRequests.LogChallengeEventRequest();
request.eventKey = "ChallengeEndTurn";
request.challengeInstanceId = challengeId;
request.isExpired = 1;
const response = request.ExecuteAs(hasTurnPlayerId); // Wow this `ExecuteAs` function is really useful.
