// ====================================================================================================
//
// Cloud Code for ProcessRank, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

//Retrieve the number of stars the player earned
var currentRankScore = Spark.getPlayer().getScriptData("rankScore");

if (currentRankScore) {
    currentRankScore = 10;
}

if(outcome === "victory"){
    Spark.getPlayer().setScriptData("rankScore", currentRankScore + 1);
} else {
    Spark.getPlayer().setScriptData("rankScore", currentRankScore - 1);
}
