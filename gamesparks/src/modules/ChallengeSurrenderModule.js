// ====================================================================================================
//
// Cloud Code for ChallengeSurrenderModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeMovesModule");

function handleChallengeSurrender(challengeStateData, playerId) {
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];

    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    
    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_SURRENDER_BY_CHOICE,
    };
    addChallengeMove(challengeStateData, move);
    
    winChallenge(challengeStateData, opponentId);
}
