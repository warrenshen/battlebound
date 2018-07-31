// ====================================================================================================
//
// Cloud Code for ScriptDataModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function setScriptError(errorMessage) {
    const logger = Spark.getLog();
    const stackTrace = Spark.currentStack();
    
    const logMessage = {
        errorMessage: errorMessage,
    };
    logger.error(logMessage);

    Spark.setScriptError("errorMessage", errorMessage);
    Spark.setScriptError("stackTrace", stackTrace);
    Spark.setScriptError("statusCode", 400);
    Spark.exit();
}

function setScriptErrorWithUnlockKey(keyName, errorMessage) {
    if (keyName == null) {
        setScriptError("Invalid key name given to function.");
    }
    
    Spark.unlockKeyFully(keyName);
    setScriptError(errorMessage);
}

function setScriptSuccess() {
    Spark.setScriptData("statusCode", 200);
    Spark.exit();
}