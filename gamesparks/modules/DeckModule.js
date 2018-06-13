// ====================================================================================================
//
// A module of functions related to the PlayerDecks table and in-memory deck arrays.
//
// ====================================================================================================
require("OnChainModule");
 
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
        Spark.setScriptError("ERROR", templatesDataQueryResultError);
        Spark.exit();
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
        attack: "attack",
        health: "health",
        manaCost: "cost",
        name: "name",
        description: "description",
        abilities: "abilities",
    };
    
    // Form final card objects by combining fields of both Card and Template objects.
    const results = cards.map(function(card) {
        const result = {};
        const template = templateIdToTemplate[card.templateId];
        if (!template) {
            Spark.setScriptError("ERROR", "Template " + card.templateId + " does not exist.");
            Spark.exit();
        }
        
        cardFields.map(function(field) { result[field] = card[field] });
        Object.keys(templateFieldToInstanceField).map(function(field) { result[templateFieldToInstanceField[field]] = template[field] });
        
        result.image = template.name.replace(" ", "_");
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
        Spark.setScriptError("ERROR", templatesDataQueryResultError);
        Spark.exit();
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
        manaCost: "cost",
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
            Spark.setScriptError("ERROR", "Template " + card.templateId + " does not exist.");
            Spark.exit();
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
        Spark.setScriptError("ERROR", dataQueryResultError);
        Spark.exit();
    } else {
        const dataCursor = dataQueryResult.cursor();
        while (dataCursor.hasNext()) {
            bCards.push(dataCursor.next().getData());
        }
    }
    
    return bCards;
}

function _createBCardByBCardId(bCardId) {
    const API = Spark.getGameDataService();

    const cardInt = parseInt(bCardId.substring(1));
    const templateInt = fetchTemplateIntByCardInt(cardInt);
    if (!Number.isInteger(templateInt)) {
        Spark.setScriptError("ERROR", "Invalid templateInt for cardInt.");
        Spark.exit();
    }
    const templateId = "B" + templateInt.toString();
    
    const cardDataItem = API.createItem("Card", bCardId);
    const cardData = cardDataItem.getData();
    
    cardData.id = bCardId;
    cardData.templateId = templateId;
    cardData.level = 0;
    
    const error = cardDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
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

function _getBCardIdsByPlayer(player) {
    const bCardIds = _getBCardIdsFromChainByPlayer(player);
    const bCards = getBCardsByBCardIds(bCardIds);
        
    if (bCardIds.length !== bCards.length) {
        const existingBCardIds = bCards.map(function(bCard) { return bCard.id });
        const missingBCardIds = bCardIds.filter(function(bCardId) { return existingBCardIds.indexOf(bCardId) < 0 });
        
        missingBCardIds.forEach(function(bCardId) {
            _createBCardByBCardId(bCardId);
        });
    }

    return bCardIds;
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
 * @return bool - whether player deck(s) have changed after sync
 **/
function syncPlayerDecksByPlayer(player) {
    const API = Spark.getGameDataService();
        
    const playerId = player.getPlayerId();
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    const decksData = decksDataItem.getData();
    const cCardIds = Object.keys(decksData.cardByCardId);
    const bCardIds = _getBCardIdsByPlayer(player);
    
    decksData.bCardIds = bCardIds;
    var isChanged = false;
    // Filter out bad card IDs from player's decks.
    const deckByName = decksData.deckByName;
    Object.keys(deckByName).forEach(function(deckName) {
        const oldCardIds = deckByName[deckName];
        const newCardIds = oldCardIds.filter(function(cardId) {
            return bCardIds.indexOf(cardId) >= 0 || cardId.indexOf("C") === 0;
        })
        deckByName[deckName] = newCardIds;
        if (oldCardIds.length !== newCardIds.length) {
            isChanged = true;
        }
    });
    
    const error = decksDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
    
    return isChanged;
}

/**
 * Returns array of card objects of all cards player owns.
 * 
 * @param player - GS player
 * @return array - two element array: [array of card objects, map of deck name => array of card ids]
 **/
function getCardsAndDecksByPlayer(player) {
    // The term instance = card + template combined.
    const API = Spark.getGameDataService();
    const playerId = player.getPlayerId();
    
    const isChanged = syncPlayerDecksByPlayer(player);
    
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    const decksData = decksDataItem.getData();
    const deckByName = decksData.deckByName;
    const cardByCardId = decksData.cardByCardId;
    
    const bCardIds = decksData.bCardIds;
    const cCardIds = Object.keys(cardByCardId);
    
    const bCards = getBCardsByBCardIds(bCardIds);
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

/**
 * Returns array of card objects of active deck of player.
 * 
 * @param playerId - GS player ID
 * @return array - array of card objects
 **/
function getActiveDeckByPlayerId(playerId) {
    const API = Spark.getGameDataService();
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    
    const decksData = decksDataItem.getData();
    const deckByName = decksData.deckByName;
    const activeDeck = decksData.activeDeck;
    const cardIds = deckByName[activeDeck];
    const cardByCardId = decksData.cardByCardId;
    
    const bCardIds = cardIds.filter(function(cardId) { return cardId.indexOf("B") === 0 });
    const cCardIds = cardIds.filter(function(cardId) { return cardId.indexOf("C") === 0 });;
    
    if (bCardIds.length + cCardIds.length !== cardIds.length) {
        Spark.setScriptError("ERROR", "Invalid card ID present in deck.");
        Spark.exit();
    }
    
    const bCards = getBCardsByBCardIds(bCardIds);
    const cCards = cCardIds.map(function(cardId) {
        return cardByCardId[cardId]; 
    });
    
    const instances = getInstancesByCards(bCards.concat(cCards), DEFAULT_CARD_FIELDS);
    instances.forEach(function(instance) {
        instance.attackStart = instance.attack;
        instance.healthStart = instance.health;
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
    const API = Spark.getGameDataService();
    const playerId = player.getPlayerId();
    
    const isChanged = syncPlayerDecksByPlayer(player);
    
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    const decksData = decksDataItem.getData();
    
    const bCardIds = decksData.bCardIds;
    const bCards = getBCardsByBCardIds(bCardIds);
    
    // Array of instances of all cards of player;
    const instances = _getCardAuctionsByBCards(bCards);
    const sortedInstances = instances.slice().sort(function(a, b) {
        return parseInt(a.id.substring(1)) < parseInt(b.id.substring(1)) ? -1 : 1;
    });
    
    return sortedInstances;
}

/**
 * @param deck - a non-empty array of Card objects.
 * @return - a two element array: drawn Card object + array of remaining Card objects
 **/
function drawCard(deck) {
    if (!Array.isArray(deck) || deck.length === 0) {
        Spark.setScriptError("ERROR", "Invalid deck parameter.");
        Spark.exit();
    }
    const deckSize = deck.length;
    const randomIndex = Math.floor(Math.random() * deckSize);
    return [deck[randomIndex], deck.slice(0, randomIndex).concat(deck.slice(randomIndex + 1))];
}
