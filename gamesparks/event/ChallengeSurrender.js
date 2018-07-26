// ====================================================================================================
//
// Cloud Code for ChallengeSurrender, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("AttackModule");
require("ChallengeMovesModule");

const player = Spark.getPlayer();
const playerId = player.getPlayerId();
const challengeId = Spark.getData().challengeId;

if (challengeId.length <= 0) {
    setScriptError("Invalid parameter - challengeId cannot be empty.");
}

const challenge = Spark.getChallenge(challengeId);

if (challenge.getRunState() != "RUNNING") {
    setScriptError("Challenge is not running.");
}

const challengeStateData = challenge.getPrivateData("data");
const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

// Reset `lastMoves` attribute in ChallengeState.
challengeStateData.lastMoves = [];

const move = {
    playerId: playerId,
    category: MOVE_CATEGORY_SURRENDER_BY_CHOICE,
};
addChallengeMove(challengeStateData, move);

require("PersistChallengeStateModule");

const opponent = Spark.loadPlayer(opponentId);
challenge.winChallenge(opponent);

setScriptSuccess();
