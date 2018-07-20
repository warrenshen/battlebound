// ====================================================================================================
//
// Cloud Code for ChallengePlaySpellTargetedModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeMovesModule");
require("ChallengeEffectsModule");

function handlePlaySpellTargeted(challengeStateData, playerId, cardId, attributes) {
    const playerState = challengeStateData.current[playerId];
    if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }
    
    const playerManaCurrent = playerState.manaCurrent;
    const playerHand = playerState.hand;
    const playerField = playerState.field;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    // Find index of card played in hand.
    const handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
    if (handIndex < 0) {
        setScriptError("Invalid cardId parameter");
    }
    
    const playedCard = playerHand[handIndex];
    
    if (playedCard.cost > playerManaCurrent) {
        setScriptError("Card mana cost exceeds player's current mana.");
    } else {
        playerState.manaCurrent -= playedCard.cost;
        if (!Number.isInteger(playerState.manaCurrent)) {
            setScriptError("Player mana current is no longer an int.");
        }
    }
    
    if (playedCard.category !== CARD_CATEGORY_SPELL) {
        setScriptError("Invalid card category - must be spell category.");
    }
    
    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    challengeStateData.moveTakenThisTurn = 1;
    
    var move = {
        playerId: playerId,
        category: MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        attributes: {
            card: playedCard,
            cardId: cardId,
            handIndex: handIndex,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    const fieldId = attributes.fieldId;
    const targetId = attributes.targetId;
    
    processSpellTargetedPlay(challengeStateData, playerId, playedCard, fieldId, targetId);
    
    // const filterDeadResponse = filterDeadCardsFromFields(playerField, opponentField);
    // playerState.field = filterDeadResponse[0];
    // opponentState.field = filterDeadResponse[1];
    
    // Remove played card from hand.
    const newHand = playerHand.slice(0, handIndex).concat(playerHand.slice(handIndex + 1));
    playerState.hand = newHand;
}
