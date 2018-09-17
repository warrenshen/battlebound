// ====================================================================================================
//
// Cloud Code for RegistrationRequest, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");

const userName = Spark.getData().userName;
const password = Spark.getData().password;

if (
    userName == null ||
    userName.length < 3 ||
    userName.indexOf("@") < 0 ||
    userName.indexOf(".") < 0
) {
    setScriptError("Email address is invalid.");
} else if (userName.length > 30) {
    setScriptError("Email address cannot be more than 30 characters.");
}

if (
    password == null ||
    password.length < 8
) {
    setScriptError("Password must be at least 8 characters.");
} else if (password.length > 36) {
    setScriptError("Password cannot be more than 36 characters.");
}

