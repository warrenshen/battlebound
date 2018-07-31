// ====================================================================================================
//
// Cloud Code for ChallengeLogDeviceError, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");

const challengeId = Spark.getData().challengeId;
const requestName = Spark.getData().requestName;
const errorMessage = Spark.getData().errorMessage;
const stackTrace = Spark.getData().stackTrace;
const devicePlayerState = Spark.getData().devicePlayerState;
const deviceOpponentState = Spark.getData().deviceOpponentState;

const log = {
    challengeId: challengeId,
    requestName: requestName,
    errorMessage: errorMessage,
    stackTrace: stackTrace,
    devicePlayerState: devicePlayerState,
    deviceOpponentState: deviceOpponentState,
};

const logger = Spark.getLog();
logger.error(log);

setScriptSuccess();
