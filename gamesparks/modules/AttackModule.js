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