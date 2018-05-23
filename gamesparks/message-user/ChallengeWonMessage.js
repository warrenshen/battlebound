// ====================================================================================================
//
// Cloud Code for ChallengeWonMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

//If it's a ranked match, process rank
// use casual match for now
if(Spark.getData().challenge.shortCode === "CasualChallenge")
{
    //This player was victorious so declare the outcome to 'Victory'
    var outcome = "victory";

    //Depending on the outcome variable the 'processRank' module will either increase rank or decrease player rank
    require("ProcessRank");
}