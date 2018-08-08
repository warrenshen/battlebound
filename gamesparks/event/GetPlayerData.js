// ====================================================================================================
//
// Cloud Code for GetPlayerData, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("OnChainModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const address = player.getPrivateData("address");
var balance = 0;
if (address) {
    balance = fetchBalanceByAddress(address);
}

var rankGlobal = 0;
var rankElo = 0;

const leaderboard = Spark.getLeaderboards().getLeaderboard("HIGH_SCORE_LB");
const leaderboardCursor = leaderboard.getEntriesFromPlayer(playerId, 1);

if (leaderboardCursor.hasNext()) {
    leaderboardEntry = leaderboardCursor.next();
    rankGlobal = leaderboardEntry.getRank();
    rankElo = leaderboardEntry.getAttribute("SCORE_ATTR");
}

const GAME_VERSION_LATEST = "0.1";

Spark.setScriptData("playerId", playerId);
Spark.setScriptData("displayName", player.getDisplayName());
Spark.setScriptData("address", address);
Spark.setScriptData("balance", balance);
Spark.setScriptData("activeChallengeId", player.getPrivateData("activeChallengeId"));
Spark.setScriptData("infoByMatchType", player.getPrivateData("infoByMatchType"));
Spark.setScriptData("level", player.getPrivateData("level"));
Spark.setScriptData("rankGlobal", rankGlobal)
Spark.setScriptData("rankElo", rankElo);
Spark.setScriptData("latestVersion", GAME_VERSION_LATEST);

setScriptSuccess();
