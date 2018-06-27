// ====================================================================================================
//
// Cloud Code for ScriptDataModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function setScriptError(errorMessage) {
    const logger = Spark.getLog();
    // const stackTrace = Spark.currentStack();
    
    const logMessage = {
        errorMessage: errorMessage,
        // stackTrace: stackTrace,
    };
    logger.error(logMessage);

    Spark.setScriptError("errorMessage", errorMessage);
    Spark.setScriptError("statusCode", 400);
    Spark.exit();
}

function setScriptSuccess() {
    Spark.setScriptData("statusCode", 200);
    Spark.exit();
}