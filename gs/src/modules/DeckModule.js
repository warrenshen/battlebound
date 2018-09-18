// ====================================================================================================
//
// A module of functions related to the PlayerDecks table and in-memory deck arrays.
//
// ====================================================================================================
require("OnChainModule");
require("ChallengeCardModule");
require("InitializePlayerModule");
 
const DEFAULT_CARD_FIELDS = [
    "id",
    "level",
];

/**
 * Fetches Template objects from the DB and uses
 * them to form card objects as shown to the client.
 * 
 * @param cards - array of cards from a PlayerDecks instance
 * @return - array of Card objects
 **/
function getInstancesByCards(cards, cardFields) {
    const API = Spark.getGameDataService();
    
    // Query Template table by `templateIds`.
    const templateIds = cards.map(function(card) { return card.templateId });
    const templates = [];
    
    const templatesDataQuery = API.S("id").in(templateIds);
    const templatesDataQueryResult = API.queryItems("Template", templatesDataQuery);
    const templatesDataQueryResultError = templatesDataQueryResult.error();
    
    if (templatesDataQueryResultError) {
        setScriptError(templatesDataQueryResultError);
    } else {
        const templatesDataCursor = templatesDataQueryResult.cursor();
        while (templatesDataCursor.hasNext()) {
            templates.push(templatesDataCursor.next().getData());
        }
    }
    
    const templateIdToTemplate = {};
    templates.map(function(template) { templateIdToTemplate[template.id] = template });
    
    const templateFieldToInstanceField = {
        category: "category",
        color: "color",
        attack: "attack",
        health: "health",
        cost: "cost",
        name: "name",
        description: "description",
        abilities: "abilities",
    };
    
    const templateRequiredFields = [
        "category",
        // "attack",
        // "health",
        "cost",
        "name",
    ];
    
    // Form final card objects by combining fields of both Card and Template objects.
    const results = cards.map(function(card) {
        const result = {};
        const template = templateIdToTemplate[card.templateId];
        if (!template) {
            setScriptError("Template " + card.templateId + " does not exist.");
        }
        
        cardFields.map(function(field) { result[field] = card[field] });
        Object.keys(templateFieldToInstanceField).map(function(field) {
            if (templateRequiredFields.indexOf(field) >= 0 && template[field] == null) {
                setScriptError("Template " + template.id + " is invalid, " + field + " is missing.");
            }
            result[templateFieldToInstanceField[field]] = template[field];
        });
        
        return result;
    });
    
    return results;
}

/**
 * Fetches Template objects from the DB and uses
 * them to form card objects as shown to the client.
 * 
 * @param cards - array of cards from a PlayerDecks instance
 * @return - array of Card objects
 **/
function _getCardAuctionsByBCards(bCards) {
    const API = Spark.getGameDataService();
    
    // Query Template table by `templateIds`.
    const templateIds = bCards.map(function(card) { return card.templateId });
    const templates = [];
    
    const templatesDataQuery = API.S("id").in(templateIds);
    const templatesDataQueryResult = API.queryItems("Template", templatesDataQuery);
    const templatesDataQueryResultError = templatesDataQueryResult.error();
    
    if (templatesDataQueryResultError) {
        setScriptError(templatesDataQueryResultError);
    } else {
        const templatesDataCursor = templatesDataQueryResult.cursor();
        while (templatesDataCursor.hasNext()) {
            templates.push(templatesDataCursor.next().getData());
        }
    }
    
    const templateIdToTemplate = {};
    templates.map(function(template) { templateIdToTemplate[template.id] = template });
    
    const cardFields = [
        "id",
        "level"
    ];
    const templateFieldToInstanceField = {
        category: "category",
        attack: "attack",
        health: "health",
        cost: "cost",
        name: "name",
        description: "description",
        abilities: "abilities",
        auction: "auction",
    };
    
    // Form final card objects by combining fields of both Card and Template objects.
    const results = bCards.map(function(card) {
        const result = {};
        const template = templateIdToTemplate[card.templateId];
        if (!template) {
            setScriptError("Template " + card.templateId + " does not exist.");
        }
        
        cardFields.map(function(field) { result[field] = card[field] });
        Object.keys(templateFieldToInstanceField).map(function(field) { result[templateFieldToInstanceField[field]] = template[field] });
        
        result.image = template.name.replace(" ", "_");
        return result;
    });
    
    return results;
}

/**
 * Fetches Card objects from the DB by b-card IDs.
 * 
 * @param bCardIds - array of b-card IDs from a PlayerDecks instance
 * @return - array of Card objects
 **/
function getBCardsByBCardIds(bCardIds) {
    const API = Spark.getGameDataService();
    const bCards = [];

    const dataQuery = API.S("id").in(bCardIds);
    const dataQueryResult = API.queryItems("Card", dataQuery);
    const dataQueryResultError = dataQueryResult.error();
 
    if (dataQueryResultError) {
        setScriptError(dataQueryResultError);
    } else {
        const dataCursor = dataQueryResult.cursor();
        while (dataCursor.hasNext()) {
            bCards.push(dataCursor.next().getData());
        }
    }

    return bCards;
}

function createBCardByBCardId(bCardId) {
    const API = Spark.getGameDataService();

    const cardInt = parseInt(bCardId.substring(1));
    const templateInt = fetchTemplateIntByCardInt(cardInt);
    if (!Number.isInteger(templateInt)) {
        setScriptError("Invalid templateInt for cardInt.");
    }
    const templateId = "B" + templateInt.toString();
    
    const cardDataItem = API.createItem("Card", bCardId);
    const cardData = cardDataItem.getData();
    
    cardData.id = bCardId;
    cardData.templateId = templateId;
    cardData.level = 1;
    cardData.exp = 0;
    cardData.expMax = 5;
    
    const error = cardDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
    
    return cardDataItem;
}

function _getBCardIdsFromChainByPlayer(player) {
    const address = player.getPrivateData("address");
    if (address) {
        const bCardInts = fetchCardIdsByAddress(address);
        return bCardInts.map(function(cardInt) { return "B" + cardInt.toString() });
    } else {
        return [];
    }
}

function _getBCardsByPlayer(player) {
    const bCardIds = _getBCardIdsFromChainByPlayer(player);
    const bCards = getBCardsByBCardIds(bCardIds);

    if (bCardIds.length !== bCards.length) {
        const existingBCardIds = bCards.map(function(bCard) { return bCard.id });
        const missingBCardIds = bCardIds.filter(function(bCardId) { return existingBCardIds.indexOf(bCardId) < 0 });
        
        missingBCardIds.forEach(function(bCardId) {
            bCards.push(createBCardByBCardId(bCardId).getData());
        });
    }

    return bCards;
}

/**
 * Sync the given player's PlayerDecks instance with the
 * cards on the blockchain, updating the `bCardIds` and `deckByName`
 * fields on the PlayerDecks instance.
 * 
 * Returns whether a change has happened to the `deckByName` field
 * as a result of the blockchain sync.
 * 
 * @param player - GS player
 * @return array - array of b-cards player owns
 **/
function syncPlayerDecksByPlayer(player) {
    const API = Spark.getGameDataService();
        
    const playerId = player.getPlayerId();
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    const decksData = decksDataItem.getData();
    const cCardIds = Object.keys(decksData.cardByCardId);
    
    const bCards = _getBCardsByPlayer(player);
    const bCardIds = bCards.map(function(bCard) { return bCard.id });
    decksData.bCardIds = bCardIds;

    // Filter out bad card IDs from player's decks.
    const deckByName = decksData.deckByName;
    Object.keys(deckByName).forEach(function(deckName) {
        var oldCardIds = deckByName[deckName];
        var newCardIds = oldCardIds.filter(function(cardId) {
            return bCardIds.indexOf(cardId) >= 0 || cCardIds.indexOf(cardId) >= 0;
        })
        deckByName[deckName] = newCardIds;
    });
    
    const error = decksDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
    
    return bCards;
}

/**
 * @param playerId - string
 * @return array - array of string deck names
 **/
function getDecksByPlayer(playerId) {
    const decksData = getPlayerDecksByPlayerId(playerId);
    const deckByName = decksData.deckByName;
    return Object.keys(deckByName);
}

/**
 * Returns array of card objects of all cards player owns.
 * 
 * @param player - GS player
 * @return array - two element array: [array of card objects, map of deck name => array of card ids]
 **/
function getCardsAndDecksByPlayer(player) {
    // The term instance = card + template combined.
    const decksData = getPlayerDecksByPlayerId(player.getPlayerId());
    
    const deckByName = decksData.deckByName;
    const cardByCardId = decksData.cardByCardId;
    
    const bCardIds = decksData.bCardIds;
    const cCardIds = Object.keys(cardByCardId);
    
    const bCards = syncPlayerDecksByPlayer(player);
    const cCards = cCardIds.map(function(cardId) {
        return cardByCardId[cardId]; 
    });
    
    // Array of instances of all cards of player;
    const instances = getInstancesByCards(bCards.concat(cCards), DEFAULT_CARD_FIELDS);
    const sortedInstances = instances.slice().sort(function(a, b) {
        if (a.id.indexOf("B") === 0 && b.id.indexOf("B") < 0) {
            return -1;
        } else if (a.id.indexOf("B") < 0 && b.id.indexOf("B") === 0) {
            return 1;
        } else {
            return parseInt(a.id.substring(1)) < parseInt(b.id.substring(1)) ? -1 : 1;
        }
    });
    
    return [sortedInstances, deckByName];
}

function getPlayerDecksByPlayerId(playerId) {
    const API = Spark.getGameDataService();
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    if (decksDataItem == null) {
        setScriptError("PlayerDecks does not exist.");
    }
    
    return decksDataItem.getData();
}

/**
 * Returns array of challenge card objects of active deck of player.
 * 
 * @param playerId - GS player ID
 * @return array - array of challenge card objects
 **/
function getActiveDeckByPlayerId(playerId) {
    const player = Spark.loadPlayer(playerId);
    const activeDeck = player.getPrivateData("activeDeck");
    
    const API = Spark.getGameDataService();
    var decksDataItem = API.getItem("PlayerDecks", playerId).document();
    if (decksDataItem == null) {
        decksDataItem = initializePlayerByPlayerId(playerId);
    }
    
    const decksData = decksDataItem.getData();
    const deckByName = decksData.deckByName;

    const cardIds = deckByName[activeDeck];
    const cardByCardId = decksData.cardByCardId;
    
    const bCardIds = cardIds.filter(function(cardId) { return cardId.indexOf("B") === 0 });
    const cCardIds = cardIds.filter(function(cardId) { return cardId.indexOf("C") === 0 });
    
    if (bCardIds.length + cCardIds.length !== cardIds.length) {
        setScriptError("Invalid card ID present in deck.");
    }
    
    const bCards = getBCardsByBCardIds(bCardIds);
    const cCards = cCardIds.map(function(cardId) {
        return cardByCardId[cardId]; 
    });
    
    return getChallengeDeckByPlayerIdAndCards(playerId, bCards.concat(cCards));
}

/**
 * Returns array of challenge card objects from card objects.
 * 
 * @param playerId - GS player ID
 * @param cards - array of card objects
 * 
 * @return array - array of challenge card objects
 **/
function getChallengeDeckByPlayerIdAndCards(playerId, cards) {
    const instances = getInstancesByCards(cards, DEFAULT_CARD_FIELDS);
    instances.forEach(function(instance, index) {
        const name = instance.name;
        const level = instance.level;
        
        // if (level < 1 || level > 6) {
        //     setScriptError("Invalid card level.");
        // }
        
        // const levelToStats = CARD_NAME_TO_LEVEL_TO_STATS[name];
        // if (levelToStats != null) {
        //     const stats = levelToStats[level];
        //     instance.cost = stats.cost;
        //     instance.attack = stats.attack;
        //     instance.health = stats.health;
        //     instance.abilities = stats.abilities;
        // }
        
        const baseId = instance.id;
        instance.baseId = baseId;
        instance.playerId = playerId;
        instance.id = playerId + "-" + index;
        instance.attackStart = instance.attack;
        instance.costStart = instance.cost;
        instance.healthStart = instance.health;
        instance.healthMax = instance.health;
        
        if (instance.abilities == null) {
            instance.abilities = [];
        }
        instance.abilitiesStart = instance.abilities;
        
        instance.buffsHand = [];
        instance.buffsField = [];
    });
    
    return instances;
}

/**
 * Returns array of card objects of blockchain cards player owns.
 * 
 * @param player - GS player
 * @return array - array of card objects
 **/
function getBCardsByPlayer(player) {
    // The term instance = card + template combined.
    const bCards = syncPlayerDecksByPlayer(player);
    
    // Array of instances of all cards of player;
    const instances = _getCardAuctionsByBCards(bCards);
    const sortedInstances = instances.slice().sort(function(a, b) {
        return parseInt(a.id.substring(1)) < parseInt(b.id.substring(1)) ? -1 : 1;
    });
    
    return sortedInstances;
}

const CARD_ABILITY_TO_DESCRIPTION = {};
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_CHARGE] = "Charge";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_TAUNT] = "Taunt";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_SHIELD] = "Shield";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BOOST_FRIENDLY_ATTACK_BY_TEN] = "Boost";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_DRAW_CARD] = "Warcry: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_LIFE_STEAL] = "Lifesteal";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_DRAW_CARD] = "Deathwish: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_HEAL_TEN] = "End turn: Heal 10 hp";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_HEAL_TWENTY] = "End turn: Heal 20 hp";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_DRAW_CARD] = "End turn: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN] = "Deathwish: Deal 10 dmg to enemy";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_EACH_KILL_DRAW_CARD] = "Each kill: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_ATTACK_RANDOM_THREE_BY_TWENTY] = "Deathwish: Deal 20 dmg to three random targetables";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TEN] = "End turn: Deal 10 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_ATTACK_IN_FRONT_BY_TWENTY] = "End turn: Deal 20 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TEN] = "Warcry: Deal 10 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_ATTACK_IN_FRONT_BY_TWENTY] = "Warcry: Deal 20 dmg to creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DAMAGE_TAKEN_DAMAGE_PLAYER_FACE_BY_THIRTY] = "On take damage: Deal 30 dmg to yourself";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_SILENCE_IN_FRONT] = "Warcry: Silence creature in front";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_FRIENDLY_MAX] = "Warcry: Heal friendly creatures to full health";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_TAUNT_ADJACENT_FRIENDLY] = "Warcry: Grant Taunt to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_LETHAL] = "Lethal";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_TEN] = "On attack: Deal 10 dmg to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_ATTACK_DAMAGE_ADJACENT_BY_ATTACK] = "On attack: Deal 20 dmg to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_DAMAGE_PLAYER_FACE_BY_TWENTY] = "Warcry: Deal 20 dmg to yourself";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_END_TURN_BOTH_PLAYERS_DRAW_CARD] = "End turn [BOTH]: Draw a card";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_TWENTY] = "Warcry: Heal 20 hp to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_ADJACENT_BY_FOURTY] = "Warcry: Heal 40 hp to adjacent creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_HEAL_ALL_CREATURES_BY_FOURTY] = "Warcry: Heal 40 hp to all creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_REVIVE_HIGHEST_COST_CREATURE] = "Warcry: Revive your most recent highest cost dead creature";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_OPPONENT_CREATURES_BY_TWENTY] = "Deathwish: Deal 20 dmg to all enemy creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_BATTLE_CRY_SILENCE_ALL_OPPONENT_CREATURES] = "Warcry: Silence all enemy creatures";
CARD_ABILITY_TO_DESCRIPTION[CARD_ABILITY_DEATH_RATTLE_DAMAGE_ALL_CREATURES_BY_THIRTY] = "Deathwish: Deal 30 dmg to all creatures";

const CARD_NAME_TO_LEVEL_TO_STATS = {};

CARD_NAME_TO_LEVEL_TO_STATS[CARD_NAME_FIREBUG_CATELYN] = {
    1: {
        cost: 20,
        attack: 20,
        health: 10,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN],
    },
    2: {
        cost: 20,
        attack: 24,
        health: 10,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN],
    },
    3: {
        cost: 20,
        attack: 24,
        health: 14,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TEN],
    },
    4: {
        cost: 20,
        attack: 28,
        health: 14,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY],
    },
    5: {
        cost: 20,
        attack: 28,
        health: 18,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY],
    },
    6: {
        cost: 20,
        attack: 30,
        health: 30,
        abilities: [CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_BY_TWENTY],
    },
};

CARD_NAME_TO_LEVEL_TO_STATS[CARD_NAME_MARSHWATER_SQUEALER] = {
    1: {
        cost: 20,
        attack: 20,
        health: 30,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    2: {
        cost: 20,
        attack: 20,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    3: {
        cost: 20,
        attack: 24,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    4: {
        cost: 20,
        attack: 24,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    5: {
        cost: 20,
        attack: 28,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    6: {
        cost: 20,
        attack: 30,
        health: 40,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
};

CARD_NAME_TO_LEVEL_TO_STATS[CARD_NAME_MARSHWATER_SQUEALER] = {
    1: {
        cost: 20,
        attack: 20,
        health: 30,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    2: {
        cost: 20,
        attack: 20,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    3: {
        cost: 20,
        attack: 24,
        health: 34,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    4: {
        cost: 20,
        attack: 24,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    5: {
        cost: 20,
        attack: 28,
        health: 38,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
    6: {
        cost: 20,
        attack: 30,
        health: 40,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TEN],
    },
};

CARD_NAME_TO_LEVEL_TO_STATS[CARD_NAME_WATERBORNE_RAZORBACK] = {
    1: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    2: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    3: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    4: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    5: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
    6: {
        cost: 40,
        attack: 30,
        health: 60,
        abilities: [CARD_ABILITY_END_TURN_HEAL_TWENTY],
    },
};

CARD_NAME_TO_LEVEL_TO_STATS[CARD_NAME_BLESSED_NEWBORN] = {
    1: {
        cost: 20,
        attack: 10,
        health: 10,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    2: {
        cost: 20,
        attack: 12,
        health: 12,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    3: {
        cost: 20,
        attack: 14,
        health: 14,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    4: {
        cost: 20,
        attack: 16,
        health: 16,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    5: {
        cost: 20,
        attack: 18,
        health: 18,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
    6: {
        cost: 20,
        attack: 20,
        health: 20,
        abilities: [CARD_ABILITY_BATTLE_CRY_DRAW_CARD],
    },
};
