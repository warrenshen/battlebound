// ====================================================================================================
//
// Cloud Code for AttackModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
/**
 * @return int - damage done to card
 **/
function damageCard(card, damage) {
    // add has card ability func
    if (hasCardAbilityOrBuff(card, CARD_ABILITY_SHIELD)) {
        card.abilities = card.abilities.filter(function(ability) {
            return ability != CARD_ABILITY_SHIELD;
        });
        return 0;
    } else {
        const initialHealth = card.health;
        card.health -= damage;
        return Math.min(initialHealth, damage);
    }
}

/**
 * Reduce card health to zero.
 * 
 * @return int - damage done to card
 **/
function damageCardMax(card) {
    const initialHealth = card.health;
    card.health = 0;
    return initialHealth;
}

/**
 * @return int - damage done to card
 **/
function damageCardWithLethal(card, damage) {
    if (card.abilities.indexOf(CARD_ABILITY_SHIELD) >= 0) {
        card.abilities = card.abilities.filter(function(ability) { ability != CARD_ABILITY_SHIELD });
        return 0;
    } else {
        return damageCardMax(card);
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
 * @return int - health given to card
 **/
function healCardMax(card) {
    const healthMax = card.healthMax;
    const initialHealth = card.health;
    card.health = healthMax;
    return healthMax - initialHealth;
}

function silenceCard(card) {
    card.isSilenced = 1;
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
 * Draw a card to replace mulligan card. Do NOT use for regular draw card.
 * 
 * @return object - draw card move object for drawn card
 **/
function drawCardMulliganForPlayer(playerId, playerState) {
    const playerDeck = playerState.deck;
    
    if (playerDeck.length > 0) {
        const drawCardResponse = drawCard(playerDeck);
        const drawnCard = drawCardResponse[0];
        const newDeck = drawCardResponse[1];
        
        playerState.deck = newDeck;
        playerState.deckSize = newDeck.length;
        
        playerState.hand.push(drawnCard);
        _updateHandCardCosts(playerState);
        
        return {
            playerId: playerId,
            category: MOVE_CATEGORY_DRAW_CARD_MULLIGAN,
            attributes: {
                card: drawnCard,
            },
        };
    } else {
        return {
            playerId: playerId,
            category: MOVE_CATEGORY_DRAW_CARD_FAILURE,
        };
    }
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
            category: MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY,
        };
    }
}

/**
 * @param ChallengeCard card - must have `id` field set
 * @return object - draw card move object for drawn card
 **/
function addCardToPlayerHand(playerId, playerState, card) {
    if (playerState.hand.length >= 10) {
        return {
            playerId: playerId,
            category: MOVE_CATEGORY_DRAW_CARD_HAND_FULL,
            attributes: {
                card: card,
            },
        };
    } else {
        playerState.hand.push(card);
        _updateHandCardCosts(playerState);
        
        return {
            playerId: playerId,
            category: MOVE_CATEGORY_DRAW_CARD,
            attributes: {
                card: card,
            },
        };
    }
}

/**
 * @param ChallengeCard card - must have `id` field set
 * @return object - draw card move object for drawn card
 **/
function addCardToPlayerDeck(playerId, playerState, card) {
    playerState.deck.push(card);
    playerState.deckSize = playerState.deck.length;
}


function removeCardFromHandByIndex(playerState, handIndex) {
    const hand = playerState.hand;
    const newHand = hand.slice(0, handIndex).concat(hand.slice(handIndex + 1));
    playerState.hand = newHand;
    _updateHandCardCosts(playerState);
}

const HAND_CARD_DECREASE_COST_BY_COLOR = 0;

function _updateHandCardCosts(playerState) {
    const colorToCount = {};
    const hand = playerState.hand;
    
    hand.forEach(function(card) {
        const color = card.color;
        if (color <= 0) {
            return;
        }
        
        if (colorToCount[color] != null) {
            colorToCount[color] += 1;
        } else {
            colorToCount[color] = 1;
        }
    });
    
    hand.forEach(function(card) {
        const color = card.color;
        if (color <= 0) {
            return;
        }
        
        if (colorToCount[color] >= 3) {
            if (card.handBuffs == null) {
                card.handBuffs = [];
            }
            if (card.handBuffs.indexOf(HAND_CARD_DECREASE_COST_BY_COLOR) < 0) {
                card.handBuffs.push(HAND_CARD_DECREASE_COST_BY_COLOR);
            }
        } else {
            if (card.handBuffs != null && card.handBuffs.indexOf(HAND_CARD_DECREASE_COST_BY_COLOR) >= 0) {
                card.handBuffs = card.handBuffs.filter(function(handBuff) {
                    return handBuff != HAND_CARD_DECREASE_COST_BY_COLOR;
                });
            }
        }
    });
    
    hand.forEach(function(card) {
        var baseCost = card.costStart;
        if (card.handBuffs != null && card.handBuffs.indexOf(HAND_CARD_DECREASE_COST_BY_COLOR) >= 0) {
            baseCost -= 10;
        }
        
        card.cost = baseCost;
    });
}

function addChallengeMove(challengeStateData, move) {
    const moveCount = challengeStateData.moves.length;
    move.rank = moveCount;
    
    challengeStateData.moves.push(move);
    challengeStateData.lastMoves.push(move);
}

function addChallengeDeadCard(challengeStateData, deadCard) {
    challengeStateData.deadCards.forEach(function(card) {
        if (card.id === deadCard.id) {
            setScriptError("Cannot add a duplicate card to dead cards.");
        }
    });
    challengeStateData.deadCards.push(deadCard);    
}

function getNewSpawnRank(challengeStateData) {
    const spawnRank = challengeStateData.spawnCount;
    challengeStateData.spawnCount += 1;
    return spawnRank;
}

function getNewDeathRank(challengeStateData) {
    const deathRank = challengeStateData.deathCount;
    challengeStateData.deathCount += 1;
    return deathRank;
}
