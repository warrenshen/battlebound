// ====================================================================================================
//
// A module of inline code to persist a ChallengeState instance.
// Expects the variables `challengeStateData` and `challengeStateDataItem` to be defined.
// Sets a scriptData field and exits on error.
//
// ====================================================================================================
if (challengeStateData.nonce === undefined) {
    challengeStateData.nonce = 0;
} else {
    challengeStateData.nonce += 1;
}

const error = challengeStateDataItem.persistor().persist().error();
if (error) {
    Spark.setScriptError("ERROR", error);
    Spark.exit();
}
