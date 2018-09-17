// ====================================================================================================
//
// Cloud Code for CampaignEndTurn, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeStateModule");
require("ChallengeEndTurnModule");

// This is secure because the only thing a malicious actor
// can do is send end turn's on time expired for themselves -
// which would only be detrimental to themselves.
const isExpired = Spark.getData().isExpired;

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const API = Spark.getGameDataService();
var dexDataItem = API.getItem("Dex", playerId).document();
if (dexDataItem === null) {
    setScriptError("Player does not have a Dex.");
}

const dexData = dexDataItem.getData();
const challengeStateData = dexData.campaign;
const challengeId = "CAMPAIGN";

// Note that the call below must be before the `challenge.consumeTurn()` call.
// TODO: Be careful with challengeId!
// cancelScheduledTimeEvents(challengeId, playerId);

// Used to determine whether to send
// time expiring messages for opponent.
var isChallengeOver = false;

const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

// If this is an end turn request from time expired and no move
// has been taken this turn, increment player's expired streak.
if (isExpired && challengeStateData.moveTakenThisTurn === 0) {
    challengeStateData.expiredStreakByPlayerId[playerId] += 1;
    
    // If expired streak is greater than 2, auto-surrender player.
    if (challengeStateData.expiredStreakByPlayerId[playerId] > 2) {
        // Reset `lastMoves` attribute in ChallengeState.
        challengeStateData.lastMoves = [];
    
        var move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SURRENDER_BY_EXPIRE,
        };
        addChallengeMove(challengeStateData, move);

        winChallenge(challengeStateData, opponentId);
        isChallengeOver = true;
    }
} else {
    challengeStateData.expiredStreakByPlayerId[playerId] = 0;
}

// We only do the following if challenge is not over.
if (!isChallengeOver) {
    handleChallengeEndTurn(challengeStateData, playerId, true);
    handleChallengeEndTurn(challengeStateData, opponentId, false);
    // startTurnTimeEvents(challengeId, opponentId);
}

if (challengeStateData.nonce == null) {
    challengeStateData.nonce = 0;
} else {
    challengeStateData.nonce += 1;
}
const error = dexDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

setChallengeStateForPlayer(playerId, challengeStateData);
setScriptSuccess();
