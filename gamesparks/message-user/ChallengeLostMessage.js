// ====================================================================================================
//
// Cloud Code for ChallengeLostMessage, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

//If it's a ranked match, process rank
if(Spark.getData().challenge.shortCode === "CasualChallenge")
{
    //This player lost so declare the outcome to 'loss'
    var outcome = "loss";

    //Depending on the outcome variable the 'processRank' module will either increase rank or decrease player rank
    require("ProcessRank");
}
