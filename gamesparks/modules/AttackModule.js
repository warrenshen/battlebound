// ====================================================================================================
//
// Cloud Code for AttackModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const TARGET_ID_FACE = "TARGET_ID_FACE";

/**
 * @return int - damage done to card
 **/
function damageCard(card, damage) {
    if (card.hasShield) {
        card.hasShield = 0;
        return 0;
    } else {
        const initialHealth = card.health;
        card.health -= damage;
        return Math.min(initialHealth, damage);
    }
}

function healCard(card, amount) {
    if (amount % 10 !== 0) {
        setScriptError("Invalid heal card amount.");    
    }
    
    card.health = Math.min(card.health + amount, card.healthMax);
    
    return card;
}

/**
 * @return int - damage done to face
 **/
function damageFace(playerState, damage) {
    const initialHealth = playerState.health;
    playerState.health -= damage;
    return Math.min(initialHealth, damage);
}

function healFace(playerState, amount) {
    playerState.health = Math.min(playerState.health + amount, playerState.healthMax);
}

function _removeBuffsFromCard(card, deadCardIds) {
    if (card.id === "EMPTY") {
        return card;
    }
    
    const newBuffs = [];
    card.buffs.forEach(function(buff) {
        if (deadCardIds.indexOf(buff.granterId) >= 0) {
            card.attack -= buff.attack;
        } else {
            newBuffs.push(buff);
        }
    });
    card.buffs = newBuffs;
    return card;
}

/**
 * Given two player fields, updates the fields based on which cards
 * have non-positive health and returns the new player fields (cards of positive health remain).
 * 
 * @param array playerField
 * @param array opponentField
 * @return array - four-element array of [
 *   array of new player field,
 *   array of new opponent field,
 *   array of player dead cards,
 *   array of opponent dead cards,
 * ]
 **/
function filterDeadCardsFromFields(playerField, opponentField) {
    const playerDeadCards = playerField.filter(function(card) { return card.id !== "EMPTY" && card.health <= 0 });
    const opponentDeadCards = opponentField.filter(function(card) { return card.id !== "EMPTY" && card.health <= 0 });
    
    const playerDeadCardIds = playerDeadCards.map(function(card) { return card.id });
    const opponentDeadCardIds = opponentDeadCards.map(function(card) { return card.id });
    const deadCardIds = playerDeadCardIds.concat(opponentDeadCardIds);
    
    const newOpponentField = opponentField
        .map(function(card) { return (card.health && card.health > 0) ? card : { id: "EMPTY" } })
        .map(function(card) { return _removeBuffsFromCard(card, deadCardIds) });
    const newPlayerField = playerField
        .map(function(card) { return (card.health && card.health > 0) ? card : { id: "EMPTY" } })
        .map(function(card) { return _removeBuffsFromCard(card, deadCardIds) });
        
    return [newPlayerField, newOpponentField, playerDeadCards, opponentDeadCards];
}


/**
 * @param deck - a non-empty array of Card objects
 * @return - a two element array: drawn Card object + array of remaining Card objects
 **/
function drawCard(deck) {
    if (!Array.isArray(deck) || deck.length === 0) {
        setScriptError("Invalid deck parameter.");
    }
    const deckSize = deck.length;
    const randomIndex = Math.floor(Math.random() * deckSize);
    return [deck[randomIndex], deck.slice(0, randomIndex).concat(deck.slice(randomIndex + 1))];
}

/**
 * @param deck - a non-empty array of Card objects
 * @param count - number of cards to draw
 * @return - a two element array: array of drawn Card objects + array of remaining Card objects
 **/
function drawCards(deck, count) {
    if (!Array.isArray(deck) || deck.length === 0) {
        setScriptError("Invalid deck parameter.");
    } else if (count < 0) {
        setScriptError("Invalid count parameter.");
    }
    
    var response;
    var drawnCards = [];
    
    while (count > 0 && deck.length > 0) {
        response = drawCard(deck);
        drawnCards.push(response[0]);
        deck = response[1];
        count -= 1;
    }
    
    return [drawnCards, deck];
}

/**
 * @return object - draw card move object for drawn card
 **/
function drawCardForPlayer(playerId, playerState) {
    const playerDeck = playerState.deck;
    
    if (playerDeck.length > 0) {
        const drawCardResponse = drawCard(playerDeck);
        const drawnCard = drawCardResponse[0];
        const newDeck = drawCardResponse[1];
        
        playerState.deck = newDeck;
        playerState.deckSize = newDeck.length;
        
        return addCardToPlayerHand(playerId, playerState, drawnCard);
    } else {
        return {
            playerId: playerId,
            category: MOVE_CATEGORY_DRAW_CARD_FAILURE,
        };
    }
}

/**
 * @param ChallengeCard card - must have `id` field set
 * @return object - draw card move object for drawn card
 **/
function addCardToPlayerHand(playerId, playerState, card) {
    playerState.hand.push(card);
        
    return {
        playerId: playerId,
        category: MOVE_CATEGORY_DRAW_CARD,
        attributes: {
            card: card,
        },
    };
}

/**
 * @param ChallengeCard card - must have `id` field set
 * @return object - draw card move object for drawn card
 **/
function addCardToPlayerDeck(playerId, playerState, card) {
    playerState.deck.push(card);
    playerState.deckSize = playerState.deck.length;
}

function addChallengeMove(challengeStateData, move) {
    const moveCount = challengeStateData.moves.length;
    move.rank = moveCount;
    
    challengeStateData.moves.push(move);
    challengeStateData.lastMoves.push(move);
}
