// ====================================================================================================
//
// Cloud Code for GS_MINUTE, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("EveryMinuteModule");

// EMM = every minute module.
const runningKey = "SYSTEM:EMM";
const scheduler = Spark.getScheduler();
const success = scheduler.inSeconds("EveryMinuteModule", 30, {}, runningKey);
