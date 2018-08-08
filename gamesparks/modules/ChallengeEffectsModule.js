// ====================================================================================================
//
// Cloud Code for ChallengeEffectsModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================

// 0 = normal, 1 = test.
var GLOBAL_ENVIRONMENT = 0;

const EFFECT_PLAYER_AVATAR_DIE = "EFFECT_PLAYER_AVATAR_DIE";
const EFFECT_CARD_DIE = "EFFECT_CARD_DIE";
const EFFECT_CARD_DIE_AFTER_DEATH_RATTLE = "EFFECT_CARD_DIE_AFTER_DEATH_RATTLE";
const EFFECT_RANDOM_TARGET = "EFFECT_RANDOM_TARGET";
const EFFECT_CHANGE_TURN_DRAW_CARD = "EFFECT_START_TURN_DRAW_CARD"; // When opponent ends their turn.
const EFFECT_SUMMON_CREATURE = "EFFECT_SUMMON_CREATURE";

const EFFECT_H_PRIORITY_ORDER = [
    CARD_ABILITY_LIFE_STEAL,
    CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
    EFFECT_CARD_DIE,
    EFFECT_SUMMON_CREATURE,
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
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_RESUMMON,
    CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS,
    CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS,
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
    CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_DRAW_CARD,
    CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY,
    CARD_ABILITY_DEATH_RATTLE_RESUMMON,
    CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS,
    CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS,
];

function addToQueues(newEffects) {
    if (!Array.isArray(newEffects)) {
        setScriptError("New effects param is not an array.");
        setScriptData("newEffects", newEffects);
    }

    newEffects.forEach(function(effect) {
        if (effect.spawnRank == null || effect.spawnRank < 0) {
            setScriptError("Effect with invalid spawn rank found.");
        }
        if (effect.playerId == null) {
            setScriptError("Effect with invalid player ID found.");
        }
        if (effect.name == null) {
            setScriptError("Effect with invalid name found.");
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
        case EFFECT_SUMMON_CREATURE:
            newEffects = effectSummonCreature(challengeStateData, effect);
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
    const cardId = effect.cardId;

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });

    if (playerIndex < 0) {
        setScriptError("Cannot do die for invalid card ID: " + cardId);
    }

    playerField[playerIndex] = { id: "EMPTY" };

    return [];
}

function effectSummonCreature(challengeStateData, effect) {
    const playerId = effect.playerId;
    const fieldIndex = effect.fieldIndex;
    const cleanCard = effect.card;

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;

    var move;
    if (fieldIndex === -1) {
        move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL,
            attributes: {
                card: cleanCard,
                fieldId: playerId,
            },
        };
    } else {
        if (playerField[fieldIndex].id != "EMPTY") {
            setScriptError("Cannot summon on existing card.");
        }

        playerField[fieldIndex] = cleanCard;

        move = {
            playerId: playerId,
            category: MOVE_CATEGORY_SUMMON_CREATURE,
            attributes: {
                card: cleanCard,
                fieldId: playerId,
                fieldIndex: fieldIndex,
            },
        };
    }

    addChallengeMove(challengeStateData, move);

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
    } else if (card.name === SPELL_NAME_BOMBS_AWAY) {
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
        damageDone = damageFace(opponentState, 10);
        newEffects = newEffects.concat(_getEffectsOnFaceDamageTaken(challengeStateData, opponentId, opponentState, damageDone));
    } else {
        const defendingCard = opponentField.find(function(fieldCard) { return fieldCard.id === randomOpponentTargetableId });
        if (defendingCard == null) {
            setScriptError("Defending card with card ID " + randomOpponentTargetableId + " does not exist.")
        }
        damageDone = damageCard(defendingCard, 10);
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, defendingCard, damageDone));
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
        newEffects = newEffects.concat(_getEffectsOnFaceDamageTaken(challengeStateData, opponentId, opponentState, damageDone));
    } else {
        const defendingCard = opponentField.find(function(fieldCard) { return fieldCard.id === randomOpponentTargetableId });
        if (defendingCard == null) {
            setScriptError("Defending card with card ID " + randomOpponentTargetableId + " does not exist.")
        }
        damageDone = damageCard(defendingCard, 10);
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, defendingCard, damageDone));
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
        case EFFECT_PLAYER_AVATAR_DIE:
            newEffects = effectPlayerAvatarDie(challengeStateData, effect);
            break;
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
        case CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY:
            newEffects = abilityDeathRattleAttackFace(challengeStateData, effect, 20);
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
        case CARD_ABILITY_DEATH_RATTLE_RESUMMON:
            newEffects = abilityDeathRattleResummon(challengeStateData, effect);
            break;
        case CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS:
            newEffects = abilityDeathRattleSummonDuskDwellers(challengeStateData, effect);
            break;
        case CARD_ABILITY_DEATH_RATTLE_SUMMON_SUMMONED_DRAGONS:
            newEffects = abilityDeathRattleSummonSummonedDragons(challengeStateData, effect);
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

function effectPlayerAvatarDie(challengeStateData, effect) {
    const playerId = effect.playerId;
    const loserId = playerId;
    const winnerId = challengeStateData.opponentIdByPlayerId[loserId];

    winChallenge(challengeStateData, winnerId);

    return [];
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

function _getInFrontIndexByPlayerIdAndCardId(challengeStateData, playerId, cardId) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const playerIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });
    return 5 - playerIndex;
}

function _getInFrontCardByPlayerIdAndCardId(challengeStateData, playerId, cardId) {
    const opponentIndex = _getInFrontIndexByPlayerIdAndCardId(challengeStateData, playerId, cardId);
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

    const damageTaken = damageFace(playerState, amount);
    return _getEffectsOnFaceDamageTaken(challengeStateData, playerId, playerState, damageTaken);
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
        silenceCard(opponentCard);
    });

    return [];
}

function _getDeadCardsByPlayerId(challengeStateData, playerId) {
    const deadCards = challengeStateData.deadCards;
    return deadCards.filter(function(deadCard) {
        return deadCard.playerId === playerId;
    });
}

function _cleanCardForSummon(challengeStateData, dirtyCard) {
    const playerId = dirtyCard.playerId;
    const spawnCard = JSON.parse(JSON.stringify(dirtyCard));
    const spawnRank = getNewSpawnRank(challengeStateData);
    spawnCard.spawnRank = spawnRank;

    const cardRank = challengeStateData.current[playerId].cardCount;
    challengeStateData.current[playerId].cardCount += 1;

    spawnCard.id = playerId + "-" + cardRank;
    spawnCard.cost = spawnCard.costStart;
    spawnCard.attack = spawnCard.attackStart;
    spawnCard.health = spawnCard.healthStart;
    spawnCard.healthMax = spawnCard.healthStart;
    spawnCard.isFrozen = 0;
    spawnCard.isSilenced = 0;
    
    spawnCard.abilities = spawnCard.abilitiesStart;
    spawnCard.buffsHand = [];
    spawnCard.buffsField = [];

    if (hasCardAbilityOrBuff(spawnCard, CARD_ABILITY_CHARGE)) {
        spawnCard.canAttack = 1;
    } else {
        spawnCard.canAttack = 0;
    }

    return spawnCard;
}

/*
 * @param string playerId - new owner of card (player doing the converting)
 */
function _cleanCardForConvert(challengeStateData, dirtyCard, playerId) {
    const spawnCard = JSON.parse(JSON.stringify(dirtyCard));

    const cardRank = challengeStateData.current[playerId].cardCount;
    challengeStateData.current[playerId].cardCount += 1;

    spawnCard.id = playerId + "-" + cardRank;
    spawnCard.playerId = playerId;

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
function _getEffectsForReviveCardRandomLocation(challengeStateData, playerId, dirtyCard) {
	const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;

    const availableIndices = [];
    playerField.forEach(function(fieldCard, index) {
        if (fieldCard.id === "EMPTY") {
            availableIndices.push(index);
        }
	});

	const fieldIndex = _selectRandom(availableIndices);
	const cleanCard = _cleanCardForSummon(challengeStateData, dirtyCard);
    return _getEffectsForSummonCard(challengeStateData, playerId, cleanCard, fieldIndex);
}

function _getEffectsForConvertCard(challengeStateData, playerId, dirtyCard) {
    const targetIndex = _getInFrontIndexByPlayerIdAndCardId(challengeStateData, dirtyCard.playerId, dirtyCard.id);
    const closestIndex = _selectClosestAvailableIndex(challengeStateData, playerId, targetIndex);
	const cleanCard = _cleanCardForConvert(challengeStateData, dirtyCard, playerId);
    return _getEffectsForSummonCard(challengeStateData, playerId, cleanCard, targetIndex);
}

function _getEffectsForSummonCard(challengeStateData, playerId, cleanCard, fieldIndex) {
	return [{
		playerId: playerId,
		name: EFFECT_SUMMON_CREATURE,
		card: cleanCard,
		fieldIndex: fieldIndex,
		spawnRank: 0, // TODO: ??
	}];
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

    return _getEffectsForReviveCardRandomLocation(challengeStateData, playerId, reviveCard);
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
    newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, defendingCard, damageTaken));

    return newEffects;
}

function abilityDeathRattleAttackFace(challengeStateData, effect, amount) {
    const playerId = effect.playerId;
    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentState = challengeStateData.current[opponentId];

    var newEffects = [];

    const damageTaken = damageFace(opponentState, amount);
    newEffects = newEffects.concat(_getEffectsOnFaceDamageTaken(challengeStateData, opponentId, opponentState, damageTaken));

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

function _selectClosestAvailableIndex(challengeStateData, playerId, index) {
    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    
    const offsets = [0, 1, -1, 2, -2, 3, -3, 4, -4, 5, -5];
    var closestIndex = -1;
    offsets.forEach(function(offset) {
        const currentIndex = index + offset;
        if (currentIndex < 0 || currentIndex > 5) {
            return;
        } else if (playerField[currentIndex].id === "EMPTY" && closestIndex < 0) {
            closestIndex = currentIndex;
        }
    });
    return closestIndex;
}

function _selectRandom(targets) {
    if (!Array.isArray(targets) || targets.length <= 0) {
        setScriptError("Invalid parameter given to function.");
    }

    if (GLOBAL_ENVIRONMENT === 1) {
        return targets[0];
    } else {
        const randomInt = Math.floor(Math.random() * targets.length);
        return targets[randomInt];
    }
}

function _getRandomTargetableId(playerField) {
    const targetableCards = playerField.filter(function(fieldCard) {
        return fieldCard.id != "EMPTY" && fieldCard.health > 0;
    });
    const targetableIds = targetableCards.map(function(card) { return card.id });
    targetableIds.push(TARGET_ID_FACE);
    return _selectRandom(targetableIds);
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
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, opponentCard, damageDone));
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
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, playerCard, damageDone));
    });

    const opponentId = challengeStateData.opponentIdByPlayerId[playerId];
    const opponentCards = _getPlayerCards(challengeStateData, opponentId);
    opponentCards.forEach(function(opponentCard) {
        var damageDone = damageCard(opponentCard, amount);
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, opponentCard, damageDone));
    });

    newEffects.push({
        playerId: playerId,
        cardId: cardId,
        name: EFFECT_CARD_DIE_AFTER_DEATH_RATTLE,
        spawnRank: effect.spawnRank,
    });

    return newEffects;
}

function abilityDeathRattleResummon(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const fieldIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });
    const dirtyCard = playerField[fieldIndex];

    return [
        {
            playerId: playerId,
            cardId: cardId,
            name: EFFECT_CARD_DIE,
            spawnRank: dirtyCard.spawnRank,
        },
        {
            playerId: playerId,
            card: _cleanCardForSummon(challengeStateData, dirtyCard),
            name: EFFECT_SUMMON_CREATURE,
            spawnRank: dirtyCard.spawnRank,
            fieldIndex: fieldIndex,
        },
    ];
}

function abilityDeathRattleSummonDuskDwellers(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const fieldIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });
    const card = playerField[fieldIndex];

    const dirtyCard = {
        playerId: playerId,
        name: "Dusk Dweller",
        costStart: 30,
        attackStart: 20,
        healthStart: 10,
        abilitiesStart: [CARD_ABILITY_DEATH_RATTLE_RESUMMON],
        description: "",
    };
    const closestIndex = _selectClosestAvailableIndex(challengeStateData, playerId, fieldIndex);

    return [
        {
            playerId: playerId,
            cardId: cardId,
            name: EFFECT_CARD_DIE,
            spawnRank: card.spawnRank,
        },
        {
            playerId: playerId,
            card: _cleanCardForSummon(challengeStateData, dirtyCard),
            name: EFFECT_SUMMON_CREATURE,
            spawnRank: card.spawnRank,
            fieldIndex: fieldIndex,
        },
        {
            playerId: playerId,
            card: _cleanCardForSummon(challengeStateData, dirtyCard),
            name: EFFECT_SUMMON_CREATURE,
            spawnRank: card.spawnRank,
            fieldIndex: closestIndex,
        },
    ];
}

function abilityDeathRattleSummonSummonedDragons(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const fieldIndex = playerField.findIndex(function(fieldCard) { return fieldCard.id === cardId });
    const card = playerField[fieldIndex];

    const dirtyCard = {
        playerId: playerId,
        name: "Summoned Dragon",
        costStart: 60,
        attackStart: 40,
        healthStart: 50,
        abilitiesStart: [CARD_ABILITY_DEATH_RATTLE_SUMMON_DUSK_DWELLERS],
        description: "",
    };
    const closestIndex = _selectClosestAvailableIndex(challengeStateData, playerId, fieldIndex);

    return [
        {
            playerId: playerId,
            cardId: cardId,
            name: EFFECT_CARD_DIE,
            spawnRank: card.spawnRank,
        },
        {
            playerId: playerId,
            card: _cleanCardForSummon(challengeStateData, dirtyCard),
            name: EFFECT_SUMMON_CREATURE,
            spawnRank: card.spawnRank,
            fieldIndex: fieldIndex,
        },
        {
            playerId: playerId,
            card: _cleanCardForSummon(challengeStateData, dirtyCard),
            name: EFFECT_SUMMON_CREATURE,
            spawnRank: card.spawnRank,
            fieldIndex: closestIndex,
        },
    ];
}

function buffUnstablePower(challengeStateData, effect) {
    const playerId = effect.playerId;
    const cardId = effect.cardId;

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;
    const card = playerField.find(function(fieldCard) { return fieldCard.id === cardId });

    card.health = 0;

    var newEffects = _getEffectsOnCardDeath(challengeStateData, card);
    return newEffects;
}

function _addExpCard(challengeStateData, card) {
    const playerId = card.playerId;
    const baseId = card.baseId;

    if (baseId) {
        const expCardIds = challengeStateData.expCardIdsByPlayerId[playerId];
        if (expCardIds.indexOf(baseId) < 0) {
            expCardIds.push(baseId);
        }
    }
}

function _getEffectsOnCardDamageDealt(challengeStateData, card, amount) {
    _addExpCard(challengeStateData, card);

    var newEffects = [];

    if (hasCardAbilityOrBuff(card, CARD_ABILITY_LIFE_STEAL)) {
        newEffects.push({
            playerId: card.playerId,
            cardId: card.id,
            name: CARD_ABILITY_LIFE_STEAL,
            spawnRank: card.spawnRank,
            value: amount,
        });
    }

    return newEffects;
}

function _getEffectsOnCardDamageTaken(challengeStateData, card, amount) {
    var newEffects = [];

    if (hasCardAbilityOrBuff(card, CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY) && amount > 0) {
        newEffects.push({
            playerId: card.playerId,
            cardId: card.id,
            name: CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY,
            spawnRank: card.spawnRank,
        });
    }

    if (card.health <= 0) {
        newEffects = newEffects.concat(_getEffectsOnCardDeath(challengeStateData, card));
    }

    return newEffects;
}

function _getEffectsOnCardDeath(challengeStateData, card) {
    card.deathRank = getNewDeathRank(challengeStateData);
    addChallengeDeadCard(challengeStateData, card);

    const newEffects = [];

    EFFECTS_DEATH_RATTLE.forEach(function(effectName) {
        if (hasCardAbilityOrBuff(card, effectName)) {
            newEffects.push({
                playerId: card.playerId,
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
            playerId: card.playerId,
            cardId: card.id,
            name: EFFECT_CARD_DIE,
            spawnRank: card.spawnRank,
        });
    }

    return newEffects;
}

function _getEffectsOnFaceDamageTaken(challengeStateData, playerId, playerState, amount) {
    if (playerState.health <= 0) {
        return [{
            playerId: playerId,
            cardId: null,
            name: EFFECT_PLAYER_AVATAR_DIE,
            spawnRank: 0,
        }];
    }

    return [];
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
        newEffects = newEffects.concat(_getEffectsOnCardDamageDealt(challengeStateData, attackingCard, attackingDamageDone));
        newEffects = newEffects.concat(_getEffectsOnFaceDamageTaken(challengeStateData, fieldId, defenderState, attackingDamageDone));
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

        newEffects = newEffects.concat(_getEffectsOnCardDamageDealt(challengeStateData, attackingCard, attackingDamageDone));
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, defendingCard, attackingDamageDone));

        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, attackingCard, defendingDamageDone));
        newEffects = newEffects.concat(_getEffectsOnCardDamageDealt(challengeStateData, defendingCard, defendingDamageDone));

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
                newEffects = newEffects.concat(_getEffectsOnCardDamageDealt(challengeStateData, attackingCard, attackingDamageDone));
                newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, adjacentCard, attackingDamageDone));
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
const SPELL_NAME_DEATH_NOTE = "Death Note";
const SPELL_NAME_BESTOWED_VIGOR = "Bestowed Vigor";
const SPELL_NAME_CONDEMN = "Condemn";

const TARGETED_SPELLS_OPPONENT_ONLY = [
    SPELL_NAME_TOUCH_OF_ZEUS,
    SPELL_NAME_DEEP_FREEZE,
    SPELL_NAME_WIDESPREAD_FROSTBITE,
    SPELL_NAME_DEATH_NOTE,
    SPELL_NAME_CONDEMN,
];
const TARGETED_SPELLS_FRIENDLY_ONLY = [
    SPELL_NAME_UNSTABLE_POWER,
    SPELL_NAME_BESTOWED_VIGOR,
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
const SPELL_NAME_BOMBS_AWAY = "Bombs Away";
const SPELL_NAME_GRAVE_DIGGING = "Grave-digging";
const SPELL_NAME_THE_SEVEN = "The Seven";
const SPELL_NAME_BATTLE_ROYALE = "Battle Royale";

function processSpellTargetedPlay(challengeStateData, playerId, playedCard, fieldId, targetId) {
    _addExpCard(challengeStateData, playedCard);

    if (TARGETED_SPELLS_OPPONENT_ONLY.indexOf(playedCard.name) >= 0) {
        _processSpellTargetedPlayOpponent(challengeStateData, playerId, playedCard, fieldId, targetId);
    } else if (TARGETED_SPELLS_FRIENDLY_ONLY.indexOf(playedCard.name) >= 0) {
        _processSpellTargetedPlayFriendly(challengeStateData, playerId, playedCard, fieldId, targetId);
    } else if (TARGETED_SPELLS_BOTH.indexOf(playedCard.name) >= 0) {
        setScriptError("Unsupported.");
    } else {
        setScriptError("Spell name not in any targeted spell lists: " + playedCard.name);
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
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, opponentCard, damageDone));
    } else if (playedCard.name === SPELL_NAME_DEEP_FREEZE) {
        damageDone = damageCard(opponentCard, 10);
        opponentCard.isFrozen = 1;
        newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, opponentCard, damageDone));
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
    } else if (playedCard.name === SPELL_NAME_DEATH_NOTE) {
        damageCardMax(opponentCard);
        newEffects = newEffects.concat(_getEffectsOnCardDeath(challengeStateData, opponentCard));
    } else if (playedCard.name === SPELL_NAME_CONDEMN) {
        silenceCard(opponentCard);
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

    const playerState = challengeStateData.current[playerId];
    const playerField = playerState.field;

    const fieldIndex = playerField.findIndex(function(card) { return card.id === targetId });
    if (fieldIndex < 0) {
        setScriptError("Invalid targetId parameter - card does not exist.");
    }

    const card = playerField[fieldIndex];

    if (playedCard.name === SPELL_NAME_UNSTABLE_POWER) {
        // Give a creature +30, it dies at start of next turn.
        card.attack += 30;
        card.buffsField.push(BUFF_CATEGORY_UNSTABLE_POWER);
    } else if (playedCard.name === SPELL_NAME_BESTOWED_VIGOR) {
        card.attack += 20;
        card.health += 10;
        card.healthMax += 10;
        card.buffsField.push(BUFF_CATEGORY_BESTOWED_VIGOR);
    } else {
        setScriptError("Unrecognized spell card name.");
    }
}

function processSpellUntargetedPlay(challengeStateData, playerId, playedCard) {
    _addExpCard(challengeStateData, playedCard);

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
            newEffects = newEffects.concat(_getEffectsOnCardDamageTaken(challengeStateData, fieldCard, damageDone));
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

            silenceCard(fieldCard);
        });
        opponentField.forEach(function(fieldCard) {
            if (fieldCard.id === "EMPTY") {
                return;
            }

            silenceCard(fieldCard);
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
    } else if (playedCard.name === SPELL_NAME_BOMBS_AWAY) {
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
        if (deadCards.length > 0) {
			const deadCard = deadCards[deadCards.length - 1];
			newEffects = newEffects.concat(_getEffectsForReviveCardRandomLocation(challengeStateData, playerId, deadCard));
        }
    } else if (playedCard.name === SPELL_NAME_THE_SEVEN) {
        const opponentCards = _getOpponentCreatureCards(challengeStateData, playerId);
        if (opponentCards.length > 0) {
            var randomCard = _selectRandom(opponentCards);
            var move = {
                playerId: playerId,
                category: MOVE_CATEGORY_CONVERT_CREATURE,
                attributes: {
                    fieldId: randomCard.playerId,
                    targetId: randomCard.id,
                },
            };
            addChallengeMove(challengeStateData, move);
            newEffects = newEffects.concat(_getEffectsForConvertCard(challengeStateData, playerId, randomCard));
        }
    } else if (playedCard.name === SPELL_NAME_BATTLE_ROYALE) {
        const creatureCards = _getAllCreatureCards(challengeStateData, playerId);
        if (creatureCards.length > 0) {
            var randomCard = _selectRandom(creatureCards);
            var move = {
                playerId: playerId,
                category: MOVE_CATEGORY_RANDOM_TARGET,
                attributes: {
                    card: playedCard,
                    fieldId: randomCard.playerId,
                    targetId: randomCard.id,
                },
            };
            addChallengeMove(challengeStateData, move);

            creatureCards.forEach(function(creatureCard) {
                if (creatureCard.playerId === randomCard.playerId && creatureCard.id === randomCard.id) {
                    return;
                }
                damageCardMax(creatureCard);
                newEffects = newEffects.concat(_getEffectsOnCardDeath(challengeStateData, creatureCard));
            });
        }
    } else {
        setScriptError("Unrecognized spell card name.");
    }

    const queues = addToQueues(newEffects);
    processEffectQueues(challengeStateData, queues[0], queues[1], queues[2]);
}
