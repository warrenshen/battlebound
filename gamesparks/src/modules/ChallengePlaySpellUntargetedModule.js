// ====================================================================================================
//
// Cloud Code for ChallengePlaySpellUntargetedModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("AttackModule");
require("ChallengeCardModule");
require("ChallengeEffectsModule");

function handleChallengePlaySpellUntargeted(challengeStateData, playerId, cardId) {
    const challengeState = challengeStateData.current;
    
    const playerState = challengeState[playerId];
    if (playerState.mode !== PLAYER_STATE_MODE_NORMAL) {
        setScriptError("Player state is not in normal mode.");
    }
    
    const playerManaCurrent = playerState.manaCurrent;
    const playerHand = playerState.hand;
    const playerField = playerState.field;
    
    // Find index of card played in hand.
    const handIndex = playerHand.findIndex(function(card) { return card.id === cardId });
    if (handIndex < 0) {
        setScriptError("Invalid cardId parameter");
    }
    
    const playedCard = playerHand[handIndex];
        
    if (playedCard.category !== CARD_CATEGORY_SPELL) {
        setScriptError("Invalid card category - must be spell category.");
    }
    
    if (playedCard.cost > playerManaCurrent) {
        setScriptError("Card mana cost exceeds player's current mana.");
    } else {
        playerState.manaCurrent -= playedCard.cost;
        if (!Number.isInteger(playerState.manaCurrent)) {
            setScriptError("Player mana current is no longer an int.");
        }
    }
    
    // Reset `lastMoves` attribute in ChallengeState.
    challengeStateData.lastMoves = [];
    challengeStateData.moveTakenThisTurn = 1;
    
    var move = {
        playerId: playerId,
        category: MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
        attributes: {
            card: playedCard,
            cardId: cardId,
            handIndex: handIndex,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    // Remove played card from hand.
    removeCardFromHandByIndex(playerState, handIndex);
    
    processSpellUntargetedPlay(challengeStateData, playerId, playedCard);
}
