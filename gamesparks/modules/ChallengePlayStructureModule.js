// ====================================================================================================
//
// Cloud Code for ChallengePlayStructureModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeCardModule");
require("ChallengeEffectsModule");

function handleChallengePlayStructure(challengeStateData, playerId, cardId, attributes) {
    const fieldIndex = attributesJson.fieldIndex;
    // Ensure that index to play card at is valid.
    if (fieldIndex < 6 || fieldIndex > 9) {
        setScriptError("Invalid fieldIndex parameter.");
    }
    const fieldBackIndex = fieldIndex - 6;
    
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode != PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }
    
    const playerManaCurrent = playerState.manaCurrent;
    const playerHand = playerState.hand;
    const playerFieldBack = playerState.fieldBack;
    
    // const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    // const opponentState = challengeStateData.current[opponentId];
    // const opponentField = opponentState.field;
    
    // Find index of card played in hand.
    const handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
    if (handIndex < 0) {
        setScriptError("Invalid cardId parameter");
    }
    
    const playedCard = playerHand[handIndex];
    
    if (playedCard.category != CARD_CATEGORY_STRUCTURE) {
        setScriptError("Invalid card category - must be structure category.");
    }
    
    if (playedCard.cost > playerManaCurrent) {
        setScriptError("Card mana cost exceeds player's current mana.");
    } else {
        playerState.manaCurrent -= playedCard.cost;
        if (!Number.isInteger(playerState.manaCurrent)) {
            setScriptError("Player mana current is no longer an int.");
        }
    }
    
    if (playerFieldBack[fieldBackIndex].id !== "EMPTY") {
        setScriptError("Invalid fieldIndex parameter - card exists at fieldIndex.");
    }
    
    const spawnRank = getNewSpawnRank(challengeStateData);
    playedCard.spawnRank = spawnRank;
    
    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    challengeStateData.moveTakenThisTurn = 1;
    
    const fieldId = attributes.fieldId;
    const targetId = attributes.targetId;
    
    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_PLAY_STRUCTURE,
        attributes: {
            card: playedCard,
            cardId: cardId,
            fieldIndex: fieldIndex,
            handIndex: handIndex,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    // Remove played card from hand.
    removeCardFromHandByIndex(playerState, handIndex);
    
    // Play card onto field.
    playerFieldBack[fieldBackIndex] = playedCard;
    
    processPlayStructure(challengeStateData, playerId, cardId);
}
