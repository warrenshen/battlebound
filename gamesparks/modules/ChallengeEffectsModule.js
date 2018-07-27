// ====================================================================================================
//
// Cloud Code for ChallengeEffectsModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
const EFFECT_PLAYER_AVATAR_DIE = "EFFECT_PLAYER_AVATAR_DIE";
const EFFECT_CARD_DIE = "EFFECT_CARD_DIE";
const EFFECT_CARD_DIE_AFTER_DEATH_RATTLE = "EFFECT_CARD_DIE_AFTER_DEATH_RATTLE";
const EFFECT_RANDOM_TARGET = "EFFECT_RANDOM_TARGET";
const EFFECT_CHANGE_TURN_DRAW_CARD = "EFFECT_START_TURN_DRAW_CARD"; // When opponent ends their turn.

const EFFECT_H_PRIORITY_ORDER = [
    CARD_ABILITY_LIFE_STEAL,
    CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
    EFFECT_CARD_DIE,
];

const EFFECT_M_PRIORITY_ORDER = [
    EFFECT_RANDOM_TARGET,
    EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
];

const EFFECT_L_PRIORITY_ORDER = [
    EFFECT_CHANGE_TURN_DRAW_CARD,
    BUFF_CATEGORY_UNSTABLE_POWER,
    
    CARD_ABILITY_END_TURN_HEAL_TEN,
    CARD_ABILITY_END_TURN_HEAL_TWENTY,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_END_TURN_DRAW_CARD,
    CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
    
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX,
    CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT,
    CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY,
    CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY,
    CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY,
    CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES,
    CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE,
    
    CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
    CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
    
    EFFECT_PLAYER_AVATAR_DIE,
];

const EFFECTS_END_TURN = [
    CARD_ABILITY_END_TURN_HEAL_TEN,
    CARD_ABILITY_END_TURN_HEAL_TWENTY,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_END_TURN_DRAW_CARD,
    CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
];

const EFFECTS_START_TURN = [
    BUFF_CATEGORY_UNSTABLE_POWER,
];

const EFFECTS_BATTLE_CRY = [
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX,
    CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT,
    CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY,
    CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY,
    CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY,
    CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES,
    CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE,
];

const EFFECTS_DEATH_RATTLE = [
    CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
    CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
    CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
];

function hasCardAbilityOrBuff(card, abilityOrBuff) {
    if (card.isSilenced == 1) {
        return false;
    }
    
    if (card.abilities && card.abilities.indexOf(abilityOrBuff) >= 0) {
        return true;
    }
    
    if (card.buffs) {
        // TODO: what to do about multiple buffs of same type?
        card.buffs.forEach(function(buff) {
            if (buff.category === abilityOrBuff) {
                return true;
            }
        });
    }
}

function addToQueues(newEffects) {
    if (!Array.isArray(newEffects)) {
        setScriptError("New effects param is not an array.");
        setScriptData("newEffects", newEffects);
    }
    
    newEffects.forEach(function(effect) {
        if (effect.spawnRank == null || effect.spawnRank < 0) {
            setScriptError("Effect with invalid spawn rank found.");
        }    
    });
    
    const hEffects = newEffects.filter(function(effect) {
        return EFFECT_H_PRIORITY_ORDER.indexOf(effect.name) >= 0
    });
    const mEffects = newEffects.filter(function(effect) {
        return EFFECT_M_PRIORITY_ORDER.indexOf(effect.name) >= 0
    });
    const lEffects = newEffects.filter(function(effect) {
        return EFFECT_L_PRIORITY_ORDER.indexOf(effect.name) >= 0
    });
    
    if (hEffects.length + mEffects.length + lEffects.length != newEffects.length) {
        setScriptError("HLM queues total length not same as newEffects length.");    
    }
    
    hEffects.sort(function(a, b) {
        const aOrder = EFFECT_H_PRIORITY_ORDER.indexOf(a.name);
        const bOrder = EFFECT_H_PRIORITY_ORDER.indexOf(b.name);
        
        if (a.spawnRank == b.spawnRank) {
            return aOrder < bOrder ? -1 : 1;
        } else {
            return a.spawnRank < b.spawnRank ? -1 : 1;
        }
    });
    mEffects.sort(function(a, b) {
        const aOrder = EFFECT_M_PRIORITY_ORDER.indexOf(a.name);
        const bOrder = EFFECT_M_PRIORITY_ORDER.indexOf(b.name);
        
        if (a.spawnRank == b.spawnRank) {
            return aOrder < bOrder ? -1 : 1;
        } else {
            return a.spawnRank < b.spawnRank ? -1 : 1;
        }
    });
    lEffects.sort(function(a, b) {
        const aOrder = EFFECT_L_PRIORITY_ORDER.indexOf(a.name);
        const bOrder = EFFECT_L_PRIORITY_ORDER.indexOf(b.name);
        
        if (a.spawnRank == b.spawnRank) {
            return aOrder < bOrder ? -1 : 1;
        } else {
            return a.spawnRank < b.spawnRank ? -1 : 1;
        }
    });
    
    return [hEffects, mEffects, lEffects];
}

function processEffectQueues(challengeStateData, hQueue, mQueue, lQueue) {
    var newEffects;
    var addQueues;
    var effect;
    
    if (!Array.isArray(hQueue)) {
        setScriptError("Effects queue must be a list.");
    }
    if (!Array.isArray(mQueue)) {
        setScriptError("Effects queue must be a list.");
    }
    if (!Array.isArray(lQueue)) {
        setScriptError("Effects queue must be a list.");
    }
    
    while (hQueue.length > 0 || mQueue.length > 0 || lQueue.length > 0) {
        if (hQueue.length > 0) {
            newEffects = processHQueue(challengeStateData, hQueue.shift());
        } else if (mQueue.length > 0) {
            newEffects = processMQueue(challengeStateData, mQueue.shift());
        } else {
            newEffects = processLQueue(challengeStateData, lQueue.shift());
        }
        
        addQueues = addToQueues(newEffects);
 
        hQueue = hQueue.concat(addQueues[0]);
        mQueue = mQueue.concat(addQueues[1]);
        lQueue = lQueue.concat(addQueues[2]);
    }
}

function processHQueue(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === effect.cardId });
    if (card == null) {
        setScriptError("Effect card ID is invalid.");
    }
    
    var newEffects;
    
    switch (effect.name) {
        case CARD_ABILITY_LIFE_STEAL:
            newEffects = abilityLifeSteal(challengeStateData, effect);
            break;
        case CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY:
            newEffects = abilityDamagePlayerFace(challengeStateData, effect, 30);
            break;
        case EFFECT_CARD_DIE:
            newEffects = effectCardDie(challengeStateData, effect);
            break;
        default:
            setScriptError("Effect not supported.");
            break;
    }
    
    return newEffects;
}

function abilityLifeSteal(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    
    healFace(playerState, effect.value);
    
    return [];
}

// Literally remove card from field, dying breath already handled.
function effectCardDie(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === effect.cardId });
    
    playerField[playerIndex] = { id: "EMPTY" };
    
    return [];
}

function processMQueue(challengeStateData, effect) {
    var newEffects;
    
    switch (effect.name) {
        case EFFECT_RANDOM_TARGET:
            newEffects = effectRandomTarget(challengeStateData, effect)
            break;
        case EFFECT_CARD_DIE_AFTER_DEATH_RATTLE:
            newEffects = effectCardDieAfterDeathRattle(challengeStateData, effect);
            break;
        default:
            setScriptError("Effect not supported.");
            break;
    }
    
    return newEffects;
}

function effectRandomTarget(challengeStateData, effect) {
    if (effect.card == null) {
        setScriptError("Card field must be set on effect.");
    }
    
    const playerId = effect.playerId;
    const card = effect.card;
    
    if (card.name === "Bombshell Bombadier") {
        return _effectRandomBombshellBombadier(challengeStateData, effect);
    } else if (card.name === "Spray n' Pray") {
        return _effectRandomSprayNPray(challengeStateData, effect);
    } else {
        setScriptError("Invalid card for this effect.");
    }
}

function _effectRandomBombshellBombadier(challengeStateData, effect) {
    const playerId = effect.playerId;
    const card = effect.card;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    var newEffects = [];
    
    const randomOpponentTargetableId = _getRandomTargetableId(opponentField);

    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_RANDOM_TARGET,
        attributes: {
            card: card,
            fieldId: opponentId,
            targetId: randomOpponentTargetableId,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    var damageDone;

    if (randomOpponentTargetableId === TARGET_ID_FACE) {
        damageDone = damageFace(opponentState, 20);
    } else {
        const defendingCard = opponentField.find(function(fieldCard) { return fieldCard.id === randomOpponentTargetableId });
        if (defendingCard == null) {
            setScriptError("Defending card with card ID " + randomOpponentTargetableId + " does not exist.")
        }
        damageDone = damageCard(defendingCard, 20);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, defendingCard, damageDone));
        
        if (defendingCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, defendingCard));
        }
    }
    
    return newEffects;
}

function _effectRandomSprayNPray(challengeStateData, effect) {
    const playerId = effect.playerId;
    const card = effect.card;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    var newEffects = [];
    
    const randomOpponentTargetableId = _getRandomTargetableId(opponentField);

    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_RANDOM_TARGET,
        attributes: {
            card: card,
            fieldId: opponentId,
            targetId: randomOpponentTargetableId,
        },
    };
    addChallengeMove(challengeStateData, move);
    
    var damageDone;

    if (randomOpponentTargetableId === TARGET_ID_FACE) {
        damageDone = damageFace(opponentState, 10);
    } else {
        const defendingCard = opponentField.find(function(fieldCard) { return fieldCard.id === randomOpponentTargetableId });
        if (defendingCard == null) {
            setScriptError("Defending card with card ID " + randomOpponentTargetableId + " does not exist.")
        }
        damageDone = damageCard(defendingCard, 10);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, defendingCard, damageDone));
        
        if (defendingCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, defendingCard));
        }
    }
    
    return newEffects;
}

// Literally remove card from field, dying breath already handled.
function effectCardDieAfterDeathRattle(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === effect.cardId });
    
    playerField[playerIndex] = { id: "EMPTY" };
    
    return [];
}

function processLQueue(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    var newEffects;
    
    switch (effect.name) {
        case CARD_ABILITY_END_TURN_HEAL_TEN:
            newEffects = abilityEndTurnHeal(challengeStateData, effect, 10);
            break;
        case CARD_ABILITY_END_TURN_HEAL_TWENTY:
            newEffects = abilityEndTurnHeal(challengeStateData, effect, 20);
            break;
        case CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX:
            newEffects = abilityHealFriendlyMax(challengeStateData, effect);
            break;
        case CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT:
            newEffects = abilitySilenceInFront(challengeStateData, effect);
            break;
        case CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY:
            newEffects = abilityTauntAdjacentFriendly(challengeStateData, effect);
            break;
        case CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY:
            newEffects = abilityHealAdjacentFriendly(challengeStateData, effect, 20);
            break;
        case CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY:
            newEffects = abilityHealAdjacentFriendly(challengeStateData, effect, 40);
            break;
        case CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY:
            newEffects = abilityDamagePlayerFace(challengeStateData, effect, 20);
            break;
        case CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY:
            newEffects = abilityHealAllCreatures(challengeStateData, effect, 40);
            break;
        case CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES:
            newEffects = abilitySilenceAllOpponentCreatures(challengeStateData, effect);
            break;
        case CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE:
            newEffects = abilityReviveHighestCostCreature(challengeStateData, effect);
            break;
        case CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN:
        case CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN:
            newEffects = abilityAttackInFront(challengeStateData, effect, 10);
            break;
        case CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY:
        case CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY:
            newEffects = abilityAttackInFront(challengeStateData, effect, 20);
            break;
        case CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN:
            newEffects = abilityDeathRattleAttackFace(challengeStateData, effect, 10);
            break;
        case CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY:
            newEffects = abilityDeathRattleAttackRandomThree(challengeStateData, effect);
            break;
        case CARD_ABILITY_DEATH_RATTLE_DRAW_CARD:
            newEffects = abilityDeathRattleDrawCard(challengeStateData, effect);
            break;
        case CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY:
            newEffects = abilityDeathRattleDamageAllOpponentCreatures(challengeStateData, effect, 20);
            break;
        case CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY:
            newEffects = abilityDeathRattleDamageAllCreatures(challengeStateData, effect, 30);
            break;
        case EFFECT_CHANGE_TURN_DRAW_CARD:
        case CARD_ABILITY_END_TURN_DRAW_CARD:
        case CARD_ABILITY_BATTLE_CRY_DRAW_CARD:
        case CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD:
            move = drawCardForPlayer(playerId, playerState);
            addChallengeMove(challengeStateData, move);
            newEffects = [];
            break;
        case BUFF_CATEGORY_UNSTABLE_POWER:
            newEffects = buffUnstablePower(challengeStateData, effect);
            break;
        default:
            setScriptError("Effect not supported.");
            break;
    }
    
    return newEffects;
}

function abilityEndTurnHeal(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === effect.cardId });
    if (card == null) {
        setScriptError("Effect card ID is invalid.");
    }
    
    healCard(card, 20);
    return [];
}

function abilityHealFriendlyMax(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    const playerField = playerState.field;
    playerField.forEach(function(fieldCard) {
        if (fieldCard.id != "EMPTY") {
            healCardMax(fieldCard);
        }
    });
    
    return [];
}

function _getInFrontCardByPlayerIdAndCardId(challengeStateData, playerId, cardId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });
    
    const opponentIndex = 5 - playerIndex;
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    const defendingCard = opponentField[opponentIndex];
    
    if (defendingCard.id === "EMPTY") {
        return null;
    } else {
        return defendingCard;
    }
}

function abilitySilenceInFront(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const defendingCard = _getInFrontCardByPlayerIdAndCardId(challengeStateData, playerId, cardId);
    
    const newEffects = [];
    
    if (defendingCard == null) {
        return newEffects;
    }
    
    silenceCard(defendingCard);
    
    return newEffects;
}

function _getAdjacentCardsByPlayerIdAndCardId(challengeStateData, playerId, cardId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });
    
    const adjacentCards = [];
    
    if (playerIndex > 0) {
        const leftCard = playerField[playerIndex - 1];
        if (leftCard.id != "EMPTY") {
            adjacentCards.push(leftCard);
        }
    }
    if (playerIndex < 5) {
        const rightCard = playerField[playerIndex + 1];
        if (rightCard.id != "EMPTY") {
            adjacentCards.push(rightCard);
        }
    }
    
    return adjacentCards;
}

function abilityTauntAdjacentFriendly(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const adjacentCards = _getAdjacentCardsByPlayerIdAndCardId(challengeStateData, playerId, cardId);
    adjacentCards.forEach(function(adjacentCard) {
        if (adjacentCard.abilities.indexOf(CARD_ABILITY_TAUNT) < 0) {
            adjacentCard.abilities.push(CARD_ABILITY_TAUNT);
        }
    });
    
    return [];
}

function abilityHealAdjacentFriendly(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const adjacentCards = _getAdjacentCardsByPlayerIdAndCardId(challengeStateData, playerId, cardId);
    adjacentCards.forEach(function(adjacentCard) {
        healCard(adjacentCard, amount);
    });
    
    return [];
}

function abilityDamagePlayerFace(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    
    damageFace(playerState, amount);
    
    return [];
}

function _getAllCreatureCards(challengeStateData, playerId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;

    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    const creatureCards = [];
    
    playerField.forEach(function(fieldCard) {
        if (fieldCard.id != "EMPTY") {
            creatureCards.push(fieldCard);
        }
    });
    opponentField.forEach(function(fieldCard) {
        if (fieldCard.id != "EMPTY") {
            creatureCards.push(fieldCard);
        }
    });
    
    return creatureCards;
}

function _getPlayerCreatureCards(challengeStateData, playerId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;

    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    return opponentField.filter(function(fieldCard) {
        return fieldCard.id != "EMPTY";
    });
}

function _getOpponentCreatureCards(challengeStateData, playerId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;

    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    return opponentField.filter(function(fieldCard) {
        return fieldCard.id != "EMPTY";
    });
}

function abilityHealAllCreatures(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const creatureCards = _getAllCreatureCards(challengeStateData, playerId);
    
    creatureCards.forEach(function(creatureCard) {
        healCard(creatureCard, amount);
    });
    
    return [];
}

function abilitySilenceAllOpponentCreatures(challengeStateData, effect) {
    const playerId = effect.playerId;
    const opponentCards = _getOpponentCreatureCards(challengeStateData, playerId);
    
    opponentCards.forEach(function(opponentCard) {
        opponentCard.isSilenced = 1;
    });
    
    return [];
}

function _getDeadCardsByPlayerId(challengeStateData, playerId) {
    const deadCards = challengeStateData.deadCards;
    const playerDeadCards = deadCards.filter(function(deadCard) {
        return deadCard.playerId === playerId;
    });
    
    // const sortedDeadCards = deadCards.slice().sort(function(a, b) {
    //     return a.spawnRank < b.spawnRank ? -1 : 1;
    // });
    
    return playerDeadCards;
}

function _cleanCardForSummon(deadCard) {
    const spawnCard = JSON.parse(JSON.stringify(deadCard));
    const spawnRank = getNewSpawnRank(challengeStateData);
    
    spawnCard.spawnRank = spawnRank;
    spawnCard.id = deadCard.baseId + "-"  + playerId + "-" + spawnRank;
    spawnCard.cost = spawnCard.costStart;
    spawnCard.attack = spawnCard.attackStart;
    spawnCard.health = spawnCard.healthStart;
    spawnCard.healthMax = spawnCard.healthStart;
    spawnCard.isFrozen = 0;
    spawnCard.isSilenced = 0;
    // TODO: abilities?
    if (hasCardAbilityOrBuff(spawnCard, CARD_ABILITY_CHARGE)) {
        spawnCard.canAttack = 1;
    } else {
        spawnCard.canAttack = 0;
    }
    
    return spawnCard;
}

/*
 * @param ChallengeCard|null dirtyCard
 */
function _attemptSpawnDeadCard(challengeStateData, playerId, dirtyCard) {
    var move;
    
    if (dirtyCard == null) {
        move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SUMMON_CREATURE_NO_CREATURE,
            attributes: {
                fieldId: playerId,
            },
        };
    } else {
    
        const spawnCard = _cleanCardForSummon(dirtyCard);
        
        const playerState = challengeStateData.current[playerId];
        const playerField = playerState.field;
        
        const availableIndices = [];
        playerField.forEach(function(fieldCard, index) {
            if (fieldCard.id === "EMPTY") {
                availableIndices.push(index);
            }
        });
        
        if (availableIndices.length <= 0) {
            move = {
                playerId: playerId,
                category: MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL,
                attributes: {
                    card: spawnCard,
                    fieldId: playerId,
                },
            };
        } else {
            const randomInt = Math.floor(Math.random() * availableIndices.length);
            const spawnIndex = availableIndices[randomInt];
            playerField[spawnIndex] = spawnCard;
            
            move = {
                playerId: playerId,
                category: MOVE_CATEGORY_SUMMON_CREATURE,
                attributes: {
                    card: spawnCard,
                    fieldId: playerId,
                    fieldIndex: spawnIndex,
                },
            };
        }
    }
    
    addChallengeMove(challengeStateData, move);
    return move.category;
}

function abilityReviveHighestCostCreature(challengeStateData, effect) {
    const playerId = effect.playerId;
    
    const sortedDeadCards = _getDeadCardsByPlayerId(challengeStateData, playerId);
    var highestCost = -1;
    var reviveCard = null;
    
    // We iterated backwards to get the "most-recently" played effect.
    for (var i = sortedDeadCards.length - 1; i >= 0; i -= 1) {
        var currentDeadCard = sortedDeadCards[i];
        var currentCost = currentDeadCard.costStart;
        if (currentCost > highestCost) {
            highestCost = currentCost;
            reviveCard = currentDeadCard;
        }
    }
    
    const spawnResponse = _attemptSpawnDeadCard(challengeStateData, playerId, reviveCard);
    
    return [];
}

function abilityAttackInFront(challengeStateData, effect, amount) {
    if (amount <= 0) {
        setScriptError("Amount should be greater than zero.");
    }
    
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const defendingCard = _getInFrontCardByPlayerIdAndCardId(challengeStateData, playerId, cardId);
    
    var newEffects = [];
    
    if (defendingCard == null) {
        return newEffects;
    };
    
    const damageTaken = damageCard(defendingCard, amount);
    newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, defendingCard, damageTaken));
    
    if (defendingCard.health <= 0) {
        newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, defendingCard));
    }
    
    return newEffects;
}

function abilityDeathRattleAttackFace(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];

    var newEffects = [];
    
    const damageDone = damageFace(opponentState, amount);
        
    newEffects.push({
        playerId: playerId,
        cardId: effect.cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    });
    
    return newEffects;
}

function abilityDeathRattleAttackRandomThree(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === cardId });
    
    const newEffects = [];
    
    for (var i = 0; i < 3; i += 1) {
        newEffects.push({
            playerId: effect.playerId,
            card: card,
            name: EFFECT_RANDOM_TARGET,
            spawnRank: i,
        });
    }
    newEffects.push({
        playerId: effect.playerId,
        cardId: effect.cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    });
    
    return newEffects;
}

function _getRandomTargetableId(playerField) {
    const targetableCards = playerField.filter(function(fieldCard) {
        return fieldCard.id != "EMPTY" && fieldCard.health > 0;
    });
    const targetableIds = targetableCards.map(function(card) { return card.id });
    targetableIds.push(TARGET_ID_FACE);
    const randomInt = Math.floor(Math.random() * targetableIds.length);
    return targetableIds[randomInt];
}

function abilityDeathRattleDrawCard(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    move = drawCardForPlayer(playerId, playerState);
    addChallengeMove(challengeStateData, move);
    
    return [{
        playerId: playerId,
        cardId: cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    }];
}

function abilityDeathRattleDamageAllOpponentCreatures(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
        
    var newEffects = [];
    
    const opponentCards = _getOpponentCreatureCards(challengeStateData, playerId);
    opponentCards.forEach(function(opponentCard) {
        var damageDone = damageCard(opponentCard, amount);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        
        if (opponentCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, opponentCard));
        }
    });
    
    newEffects.push({
        playerId: playerId,
        cardId: cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    });
    
    return newEffects;
}

function abilityDeathRattleDamageAllCreatures(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    var newEffects = [];
    
    const playerCards = _getPlayerCards(challengeStateData, playerId);
    playerCards.forEach(function(playerCard) {
        var damageDone = damageCard(playerCard, amount);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(playerId, playerCard, damageDone));
        
        if (playerCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, playerId, playerCard));
        }
    });
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentCards = _getPlayerCards(challengeStateData, opponentId);
    opponentCards.forEach(function(opponentCard) {
        var damageDone = damageCard(opponentCard, amount);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        
        if (opponentCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, opponentCard));
        }
    });
    
    newEffects.push({
        playerId: playerId,
        cardId: cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    });
    
    return newEffects;
}
            
function buffUnstablePower(challengeStateData, effect) {
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === effect.cardId });
    
    card.health = 0;
    
    var newEffects = [];
    
    newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, playerId, card));

    return newEffects;
}

function getEffectsOnCardDamageDealt(playerId, card, amount) {
    var newEffects = [];
    
    if (hasCardAbilityOrBuff(card, CARD_ABILITY_LIFE_STEAL)) {
        newEffects.push({
            playerId: playerId,
            cardId: card.id,
            name: CARD_ABILITY_LIFE_STEAL,
            spawnRank: card.spawnRank,
            value: amount,
        });
    }
    
    return newEffects;
}

function getEffectsOnCardDamageTaken(playerId, card, amount) {
    var newEffects = [];
    
    if (hasCardAbilityOrBuff(card, CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY)) {
        newEffects.push({
            playerId: playerId,
            cardId: card.id,
            name: CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
            spawnRank: card.spawnRank,
        });
    }
    
    return newEffects;
}

function getEffectsOnCardDeath(challengeStateData, playerId, card) {
    card.deathRank = getNewDeathRank(challengeStateData);
    addChallengeDeadCard(challengeStateData, card);
    
    const newEffects = [];
    
    EFFECTS_DEATH_RATTLE.forEach(function(effectName) {
        if (hasCardAbilityOrBuff(card, effectName)) {
            newEffects.push({
                playerId: playerId,
                cardId: card.id,
                name: effectName,
                spawnRank: card.spawnRank,
            });
        }
    });
    
    // If no death rattle effects, add in card die effect.
    // If there are death rattle effects, they will handle card die.
    if (newEffects.length <= 0) {
        newEffects.push({
            playerId: playerId,
            cardId: card.id,
            name: EFFECT_CARD_DIE,
            spawnRank: card.spawnRank,
        });
    }
    
    return newEffects;
}

function processEndTurn(challengeStateData, playerId) {
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Player ID is invalid.");
    }
    
    const playerField = playerState.field;
    
    const newEffects = [];
    
    playerField.forEach(function(fieldCard) {
        if (fieldCard.id === "EMPTY" || !fieldCard.abilities) {
            return;
        } else if (fieldCard.spawnRank == null) {
            setScriptError("Field card missing spawn rank.");
        }
        
        EFFECTS_END_TURN.forEach(function(effectName) {
            if (hasCardAbilityOrBuff(fieldCard, effectName)) {
                newEffects.push({
                    playerId: playerId,
                    cardId: fieldCard.id,
                    name: effectName,
                    spawnRank: fieldCard.spawnRank,
                });
            }
        });
    });
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    opponentField.forEach(function(fieldCard) {
        if (fieldCard.id === "EMPTY" || !fieldCard.abilities) {
            return;
        } else if (fieldCard.spawnRank == null) {
            setScriptError("Field card missing spawn rank.");
        }
        
        if (hasCardAbilityOrBuff(fieldCard, CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD)) {
            newEffects.push({
                playerId: opponentId,
                cardId: fieldCard.id,
                name: CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD,
                spawnRank: fieldCard.spawnRank,
            });
        }
    });
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

function processStartTurn(challengeStateData, playerId) {
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Player ID is invalid.");
    }
    
    const playerField = playerState.field;
    
    const newEffects = [];
    
    // Draw a card for player to start its turn.
    newEffects.push({
        playerId: playerId,
        name: EFFECT_CHANGE_TURN_DRAW_CARD,
        spawnRank: 0, // Does not matter.
    });
    
    playerField.forEach(function(fieldCard) {
        if (fieldCard.id === "EMPTY" || !fieldCard.abilities) {
            return;
        } else if (fieldCard.spawnRank == null) {
            setScriptError("Field card missing spawn rank.");
        }
        
        EFFECTS_START_TURN.forEach(function(effectName) {
            if (hasCardAbilityOrBuff(fieldCard, effectName)) {
                newEffects.push({
                    playerId: playerId,
                    cardId: fieldCard.id,
                    name: effectName,
                    spawnRank: fieldCard.spawnRank,
                });
            }
        });
    });
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

function processCreaturePlay(challengeStateData, playerId, cardId) {
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Player ID is invalid.");
    }
    
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === cardId });
    if (card == null) {
        setScriptError("Card ID is invalid.");
    }
    
    const newEffects = [];
    
    EFFECTS_BATTLE_CRY.forEach(function(effectName) {
        if (hasCardAbilityOrBuff(card, effectName)) {
            newEffects.push({
                playerId: playerId,
                cardId: card.id,
                name: effectName,
                spawnRank: card.spawnRank,
            });
        }
    });
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

function processCreatureAttack(challengeStateData, playerId, cardId, fieldId, targetId) {
    const attackerState = challengeStateData.current[playerId];
    const defenderState = challengeStateData.current[fieldId];
    
    const attackerField = attackerState.field;
    const defenderField = defenderState.field;
    
    // Find index of attacking card in player field.
    const attackingIndex = attackerField.findIndex(function(card) { return card.id === cardId });
    if (attackingIndex < 0) {
        setScriptError("Invalid cardId parameter.");
    }
    const attackingCard = attackerField[attackingIndex];

    var newEffects = [];
    var attackingDamageDone;
    var defendingDamageDone;
    
    if (targetId === TARGET_ID_FACE) {
        // Note there is no special logic for lethal when hitting face.
        attackingDamageDone = damageFace(defenderState, attackingCard.attack);
    
        newEffects = newEffects.concat(getEffectsOnCardDamageDealt(playerId, attackingCard, attackingDamageDone));
        
        if (defenderState.health <= 0) {
            defenderState.health = 0;
            challenge.winChallenge(Spark.getPlayer());
        }
    } else {
        // Find index of defending card in opponent field.
        const defendingIndex = defenderField.findIndex(function(card) { return card.id === targetId });
        if (defendingIndex < 0) {
            setScriptError("Invalid targetId parameter - card does not exist.");
        }
        defendingCard = defenderField[defendingIndex];
        
        if (hasCardAbilityOrBuff(attackingCard, CARD_ABILITY_LETHAL)) {
            attackingDamageDone = damageCardWithLethal(defendingCard, attackingCard.attack);
        } else {
            attackingDamageDone = damageCard(defendingCard, attackingCard.attack);
        }

        if (hasCardAbilityOrBuff(defendingCard, CARD_ABILITY_LETHAL)) {
            defendingDamageDone = damageCardWithLethal(attackingCard, defendingCard.attack);
        } else {
            defendingDamageDone = damageCard(attackingCard, defendingCard.attack);
        }
    
        newEffects = newEffects.concat(getEffectsOnCardDamageDealt(playerId, attackingCard, attackingDamageDone));
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(fieldId, defendingCard, attackingDamageDone));
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(playerId, attackingCard, defendingDamageDone));
        newEffects = newEffects.concat(getEffectsOnCardDamageDealt(fieldId, defendingCard, defendingDamageDone));
        
        if (attackingCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, playerId, attackingCard));
        }
        if (defendingCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, fieldId, defendingCard));
        }
        
        var adjacentAttack = 0;
        if (hasCardAbilityOrBuff(attackingCard, CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN)) {
            adjacentAttack = 10;
        } else if (hasCardAbilityOrBuff(attackingCard, CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK)) {
            adjacentAttack = attackingCard.attack;
        }
        
        if (adjacentAttack > 0) {
            const adjacentCards = _getAdjacentCardsByPlayerIdAndCardId(challengeStateData, fieldId, targetId);
            adjacentCards.forEach(function(adjacentCard) {
                attackingDamageDone = damageCard(adjacentCard, adjacentAttack);
                newEffects = newEffects.concat(getEffectsOnCardDamageDealt(playerId, attackingCard, attackingDamageDone));
                newEffects = newEffects.concat(getEffectsOnCardDamageTaken(fieldId, adjacentCard, attackingDamageDone));
                
                if (adjacentCard.health <= 0) {
                    newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, fieldId, adjacentCard));
                }
            });
        }
    }
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

// Targeted spells.
const SPELL_NAME_UNSTABLE_POWER = "Unstable Power";
const SPELL_NAME_TOUCH_OF_ZEUS = "Touch of Zeus";
const SPELL_NAME_DEEP_FREEZE = "Deep Freeze";
const SPELL_NAME_WIDESPREAD_FROSTBITE = "Widespread Frostbite";
const SPELL_NAME_DEATH_NOTICE = "Death Notice";

const TARGETED_SPELLS_OPPONENT_ONLY = [
    SPELL_NAME_TOUCH_OF_ZEUS,
    SPELL_NAME_DEEP_FREEZE,
    SPELL_NAME_WIDESPREAD_FROSTBITE,
    SPELL_NAME_DEATH_NOTICE,
];
const TARGETED_SPELLS_FRIENDLY_ONLY = [
    SPELL_NAME_UNSTABLE_POWER,
];
const TARGETED_SPELLS_BOTH = [
    
];

// Untargeted spells.
const SPELL_NAME_BRR_BRR_BLIZZARD = "Brr Brr Blizzard";
const SPELL_NAME_RIOT_UP = "Riot Up";
const SPELL_NAME_RAZE_TO_ASHES = "Raze to Ashes";
const SPELL_NAME_GREEDY_FINGERS = "Greedy Fingers";
const SPELL_NAME_SILENCE_OF_THE_LAMBS = "Silence of the Lambs";
const SPELL_NAME_MUDSLINGING = "Mudslinging";
const SPELL_NAME_SPRAY_N_PRAY = "Spray n' Pray";
const SPELL_NAME_GRAVE_DIGGING = "Grave-digging";

function processSpellTargetedPlay(challengeStateData, playerId, playedCard, fieldId, targetId) {
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Player ID is invalid.");
    }
    
    var damageDone;
    var newEffects = [];
    
    if (TARGETED_SPELLS_OPPONENT_ONLY.indexOf(playedCard.name) >= 0) {
        _processSpellTargetedPlayOpponent(challengeStateData, playerId, playedCard, fieldId, targetId);
    } else if (TARGETED_SPELLS_FRIENDLY_ONLY.indexOf(playedCard.name) >= 0) {
        _processSpellTargetedPlayFriendly(challengeStateData, playerId, playedCard, fieldId, targetId);
    } else if (TARGETED_SPELLS_BOTH.indexOf(playedCard.name) >= 0) {
        setScriptError("Unsupported.");
    } else {
        setScriptError("Spell name not in any targeted spell lists.");
    }
}

function _processSpellTargetedPlayOpponent(challengeStateData, playerId, playedCard, fieldId, targetId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
        
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    if (fieldId !== opponentId) {
        setScriptError("Invalid fieldId parameter.");
    }
    
    // Find index of opponent card in opponent field.
    const opponentIndex = opponentField.findIndex(function(card) { return card.id === targetId });
    if (opponentIndex < 0) {
        setScriptError("Invalid targetId parameter - card does not exist.");
    }
    
    const opponentCard = opponentField[opponentIndex];
    
    var damageDone;
    var newEffects = [];
    
    if (playedCard.name === SPELL_NAME_TOUCH_OF_ZEUS) {
        damageDone = damageCard(opponentCard, 30);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        
        if (opponentCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, opponentCard));
        }
    } else if (playedCard.name === SPELL_NAME_DEEP_FREEZE) {
        damageDone = damageCard(opponentCard, 10);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        
        if (opponentCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, opponentCard));
        } else {
            opponentCard.isFrozen = 1;
        }
    } else if (playedCard.name === SPELL_NAME_WIDESPREAD_FROSTBITE) {
        opponentCard.isFrozen = 2;
        
        const playerIndex = 5 - opponentIndex;
        const oppositeCard = playerField[playerIndex];
        if (oppositeCard.id != "EMPTY") {
            oppositeCard.isFrozen = 2;
        }
        
        if (opponentIndex > 0) {
            const leftCard = opponentField[opponentIndex - 1];
            if (leftCard.id != "EMPTY") {
                leftCard.isFrozen = 1;
            }
        }
        if (opponentIndex < 5) {
            const rightCard = opponentField[opponentIndex + 1];
            if (rightCard.id != "EMPTY") {
                rightCard.isFrozen = 1;
            }
        }
    } else if (playedCard.name === SPELL_NAME_DEATH_NOTICE) {
        damageDone = damageCardMax(opponentCard);
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, opponentCard));
    } else {
        setScriptError("Unrecognized spell card name.");
    }
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

function _processSpellTargetedPlayFriendly(challengeStateData, playerId, playedCard, fieldId, targetId) {
    if (fieldId !== playerId) {
        setScriptError("Invalid fieldId parameter.");
    }
    
    if (playedCard.name === SPELL_NAME_UNSTABLE_POWER) {
        // Give a creature +30, it dies at start of next turn.
        card.attack += 30;
        card.buffs.push({
            category: BUFF_CATEGORY_UNSTABLE_POWER,
            granterId: playedCard.id,
            attack: 30,
            abilities: [],
        });
    } else {
        setScriptError("Unrecognized spell card name.");
    }
}

function processSpellUntargetedPlay(challengeStateData, playerId, playedCard) {
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Player ID is invalid.");
    }
    const playerField = playerState.field;

    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    
    var damageDone;
    var newEffects = [];
    
    if (playedCard.name === SPELL_NAME_BRR_BRR_BLIZZARD) {
        opponentField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }
            
            fieldCard.isFrozen = 1;
        });
    } else if (playedCard.name === SPELL_NAME_RIOT_UP) {
        playerField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }
            
            if (fieldCard.abilities.indexOf(CARD_ABILITY_SHIELD) < 0) {
                fieldCard.abilities.push(CARD_ABILITY_SHIELD);
            }
        });
    } else if (playedCard.name === SPELL_NAME_RAZE_TO_ASHES) {
        opponentField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }
            
            damageDone = damageCard(fieldCard, 50);
            
            newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, fieldCard, damageDone));
            
            if (fieldCard.health <= 0) {
                newEffects = newEffects.concat(getEffectsOnCardDeath(challengeStateData, opponentId, fieldCard));
            }
        });
    } else if (playedCard.name === SPELL_NAME_GREEDY_FINGERS) {
        for (var i = 0; i < 3; i += 1) {
            var move = drawCardForPlayer(playerId, playerState);
            addChallengeMove(challengeStateData, move);
        }
    } else if (playedCard.name === SPELL_NAME_SILENCE_OF_THE_LAMBS) {
        playerField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }
            
            fieldCard.isSilenced = 1;
        });
        opponentField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }
            
            fieldCard.isSilenced = 1;
        });
    } else if (playedCard.name === SPELL_NAME_MUDSLINGING) {
        playerField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }
            
            if (fieldCard.abilities.indexOf(CARD_ABILITY_TAUNT) < 0) {
                fieldCard.abilities.push(CARD_ABILITY_TAUNT);
            }
        });
    } else if (playedCard.name === SPELL_NAME_SPRAY_N_PRAY) {
        for (var i = 0; i < 3; i += 1) {
            newEffects.push({
                playerId: playerId,
                card: playedCard,
                name: EFFECT_RANDOM_TARGET,
                spawnRank: i,
            });
        }
    } else if (playedCard.name === SPELL_NAME_GRAVE_DIGGING) {
        const deadCards = _getDeadCardsByPlayerId(challengeStateData, playerId);
        var reviveCard = null;
        
        if (deadCards.length > 0) {
            reviveCard = deadCards[deadCards.length - 1];
        }
        
        const spawnResponse = _attemptSpawnDeadCard(challengeStateData, playerId, reviveCard);
    } else {
        setScriptError("Unrecognized spell card name.");
    }
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

// } else if (playedCard.name === "Grave-digging") {
//     const moves = challengeState.moves;
//     for (var i = 0; i < moves.length; i += 1) {
//         var currentMove = moves[i];
//         if (
//             currentMove.playerId === playerId &&
//             currentMove.card &&
//             currentMove.card.playerId === playerId &&
//             currentMove.card.health <= 0
//         ) {
            
//         }
//     }
