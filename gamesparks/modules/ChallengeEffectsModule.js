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
const EFFECT_DEATH_RATTLE_ATTACK_RANDOM = "EFFECT_DEATH_RATTLE_ATTACK_RANDOM";

const EFFECT_H_PRIORITY_ORDER = [
    CARD_ABILITY_LIFE_STEAL,
    CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
    EFFECT_CARD_DIE,
];

const EFFECT_M_PRIORITY_ORDER = [
    EFFECT_DEATH_RATTLE_ATTACK_RANDOM,
    EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
];

const EFFECT_L_PRIORITY_ORDER = [
    BUFF_CATEGORY_UNSTABLE_POWER,
    
    CARD_ABILITY_END_TURN_HEAL_TEN,
    CARD_ABILITY_END_TURN_HEAL_TWENTY,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_END_TURN_DRAW_CARD,
    
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
    
    CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
    CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
    
    EFFECT_PLAYER_AVATAR_DIE,
];

const EFFECTS_END_TURN = [
    CARD_ABILITY_END_TURN_HEAL_TEN,
    CARD_ABILITY_END_TURN_HEAL_TWENTY,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_END_TURN_DRAW_CARD,
];

const EFFECTS_START_TURN = [
    BUFF_CATEGORY_UNSTABLE_POWER,
];

const EFFECTS_BATTLE_CRY = [
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN,
    CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY,
    CARD_ABILITY_BATTLE_CRY_DRAW_CARD,
];

const EFFECTS_DEATH_RATTLE = [
    CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN,
    CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
];

function hasCardAbilityOrBuff(card, abilityOrBuff) {
    if (card.abilities.indexOf(abilityOrBuff) >= 0) {
        return true;
    }
    
    // TODO: what to do about multiple buffs of same type?
    card.buffs.forEach(function(buff) {
        if (buff.category === abilityOrBuff) {
            return true;
        }
    });
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
        case EFFECT_CARD_DIE:
            newEffects = effectCardDie(challengeStateData, effect);
            break;
        default:
            setScriptError("Effect not supported.");
            break;
    }
    
    return newEffects;
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
        case EFFECT_DEATH_RATTLE_ATTACK_RANDOM:
            newEffects = effectDeathRattleAttackRandom(challengeStateData, effect)
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

function effectDeathRattleAttackRandom(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;
    
    const playerState = challengeStateData.current[playerId];   
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === cardId });
    if (card == null) {
        setScriptError("Effect card ID is invalid.");
    }
    
    if (card.name != "Bombshell Bombadier")
    {
        setScriptError("Invalid card for death rattle attack random.");
    }
    
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    if (opponentState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    const opponentField = opponentState.field;
    
    var newEffects = [];
    
    const randomOpponentTargetableId = _getRandomTargetableId(opponentField);

    const move = {
        playerId: playerId,
        category: MOVE_CATEGORY_DEATH_RATTLE_ATTACK_RANDOM_TARGET,
        attributes: {
            cardId: cardId,
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
            newEffects = newEffects.concat(getEffectsOnCardDeath(opponentId, defendingCard));
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
    
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === effect.cardId });
    if (card == null) {
        setScriptError("Effect card ID is invalid.");
    }
    
    var newEffects;
    
    switch (effect.name) {
        case CARD_ABILITY_END_TURN_HEAL_TEN:
            newEffects = abilityEndTurnHeal(challengeStateData, effect, 10);
            break;
        case CARD_ABILITY_END_TURN_HEAL_TWENTY:
            newEffects = abilityEndTurnHeal(challengeStateData, effect, 20);
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
        case CARD_ABILITY_END_TURN_DRAW_CARD:
        case CARD_ABILITY_BATTLE_CRY_DRAW_CARD:
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

function abilityAttackInFront(challengeStateData, effect, amount) {
    if (amount <= 0) {
        setScriptError("Amount should be greater than zero.");
    }
    
    const playerId = effect.playerId;
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === effect.cardId });
    
    const opponentIndex = 5 - playerIndex;
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];
    const opponentField = opponentState.field;
    const defendingCard = opponentField[opponentIndex];
    
    var newEffects = [];
    
    if (defendingCard.id === "EMPTY") {
        return newEffects;
    };
    
    const damageTaken = damageCard(defendingCard, amount);
    newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, defendingCard, damageTaken));
    
    if (defendingCard.health <= 0) {
        newEffects = newEffects.concat(getEffectsOnCardDeath(opponentId, defendingCard));
    }
    
    return newEffects;
}

function abilityDeathRattleAttackFace(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    
    const opponentState = challengeStateData.current[opponentId];
    if (opponentState == null) {
        setScriptError("Effect player ID is invalid.");
    }

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
    const newEffects = [];
    
    for (var i = 0; i < 3; i += 1) {
        newEffects.push({
            playerId: effect.playerId,
            cardId: effect.cardId,
            name: EFFECT_DEATH_RATTLE_ATTACK_RANDOM,
            spawnRank: effect.spawnRank,
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
    const playerState = challengeStateData.current[playerId];
    if (playerState == null) {
        setScriptError("Effect player ID is invalid.");
    }
    
    move = drawCardForPlayer(playerId, playerState);
    addChallengeMove(challengeStateData, move);
    
    return [{
        playerId: playerId,
        cardId: effect.cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    }];
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
    
    newEffects = newEffects.concat(getEffectsOnCardDeath(playerId, card));

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
    
    return newEffects;
}

function getEffectsOnCardDeath(playerId, card) {
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
    
    // MORE PLAYER STATE UPDATES //
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
        
        attackingDamageDone = damageCard(defendingCard, attackingCard.attack);
        defendingDamageDone = damageCard(attackingCard, defendingCard.attack);
    
        newEffects = newEffects.concat(getEffectsOnCardDamageDealt(playerId, attackingCard, attackingDamageDone));
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(fieldId, defendingCard, attackingDamageDone));
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(playerId, attackingCard, defendingDamageDone));
        newEffects = newEffects.concat(getEffectsOnCardDamageDealt(fieldId, defendingCard, defendingDamageDone));
        
        if (attackingCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(playerId, attackingCard));
        }
        if (defendingCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(fieldId, defendingCard));
        }
    }
    
    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}

// Targeted spells.
const SPELL_NAME_UNSTABLE_POWER = "Unstable Power";
const SPELL_NAME_TOUCH_OF_ZEUS = "Touch of Zeus";
const SPELL_NAME_FREEZE = "Freeze";
const SPELL_NAME_WIDESPREAD_FROSTBITE = "Widespread Frostbite";

// Untargeted spells.
const SPELL_NAME_BRR_BRR_BLIZZARD = "Brr Brr Blizzard";
const SPELL_NAME_RIOT_UP = "Riot Up";
const SPELL_NAME_RAZE_TO_ASHES = "Raze to Ashes";

const TARGETED_SPELLS_OPPONENT_ONLY = [
    SPELL_NAME_TOUCH_OF_ZEUS,
    SPELL_NAME_FREEZE,
    SPELL_NAME_WIDESPREAD_FROSTBITE,
];
const TARGETED_SPELLS_FRIENDLY_ONLY = [
    SPELL_NAME_UNSTABLE_POWER,
];
const TARGETED_SPELLS_BOTH = [
    
];

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
    const newEffects = [];
    
    if (playedCard.name === SPELL_NAME_TOUCH_OF_ZEUS) {
        damageDone = damageCard(opponentCard, 20);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        
        if (opponentCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(opponentId, opponentCard));
        }
    } else if (playedCard.name === SPELL_NAME_FREEZE) {
        damageDone = damageCard(opponentCard, 10);
        
        newEffects = newEffects.concat(getEffectsOnCardDamageTaken(opponentId, opponentCard, damageDone));
        
        if (opponentCard.health <= 0) {
            newEffects = newEffects.concat(getEffectsOnCardDeath(opponentId, opponentCard));
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
                newEffects = newEffects.concat(getEffectsOnCardDeath(opponentId, fieldCard));
            }
        });
    } else {
        setScriptError("Unrecognized spell card name.");
    }
}

// if (playedCard.name === "Blizzard") {
//     // For now, only spell that exists is to damage all enemy cards by 1.
//     opponentField.forEach(function(card) { damageCard(card, 2) });
// } else if (playedCard.name === "Greedy Fingers") {
//     for (var i = 0; i < 2; i += 1) {
//         move = drawCardForPlayer(playerId, playerState);
//         addChallengeMove(challengeStateData, move);
//     }
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
// } else {
//     setScriptError("Unrecognized spell card name.");
// }
    