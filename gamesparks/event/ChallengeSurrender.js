// ====================================================================================================
//
// Cloud Code for ChallengeSurrender, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeMovesModule");

const API = Spark.getGameDataService();

const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeId;
const challenge = Spark.getChallenge(challengeId);

if (challenge.getRunState() != "RUNNING") {
    setScriptError("Challenge is not running.");
}

const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();

if (challengeStateDataItem === null) {
    setScriptError("ChallengeState does not exist.");
}

const challengeStateData = challengeStateDataItem.getData();
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

const move = {
    playerId: playerId,
    category: MOVE_CATEGORY_SURRENDER_BY_CHOICE,
};
challengeStateData.moves.push(move);
challengeStateData.lastMoves = [move];
challengeStateData.moveTakenThisTurn = 1;

require("PersistChallengeStateModule");

const opponent = Spark.loadPlayer(opponentId);
challenge.winChallenge(opponent);

setScriptSuccess();
