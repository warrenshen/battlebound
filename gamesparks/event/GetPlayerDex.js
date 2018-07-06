// ====================================================================================================
//
// Cloud Code for GetPlayerDex, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("DeckModule");

const ALL_TEMPLATE_IDS = [
    "C0",
    "C1",
    "C2",
    "C3",
    "C4",
    "C5",
    "C6",
    "C7",
    "C8",
    "C9",
    "B0",
    "B1",
    "B2",
    "B3",
    "B4",
    "B5",
    "B6",
    "B7",
    "B8",
    "B9"
    // ...
];

const DEX_TEMPLATE_UNKNOWN = 0;
const DEX_TEMPLATE_KNOWN = 1;
const DEX_TEMPLATE_OWNED_BEFORE = 2;
const DEX_TEMPLATE_OWNED_NOW = 3;

const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const API = Spark.getGameDataService();
var dexDataItem = API.getItem("Dex", playerId).document();
if (dexDataItem === null) {
    dexDataItem = API.createItem("Dex", playerId);
}

const dexData = dexDataItem.getData();

// Ensure that dex has all templates in it.
// If it does not, add new templates with status "template is unknown".
ALL_TEMPLATE_IDS.forEach(function(templateId) {
    if (dexData[templateId] == null) {
        dexData[templateId] = {
            dexStatus: DEX_TEMPLATE_UNKNOWN,
        };
    } else if (dexData[templateId].dexStatus == null) {
        dexData[templateId].dexStatus = DEX_TEMPLATE_UNKNOWN;
    }
});

const decksData = getPlayerDecksByPlayer(player);
const cardByCardId = decksData.cardByCardId;
const bCardIds = decksData.bCardIds;
const cCardIds = Object.keys(cardByCardId);

const bCards = getBCardsByBCardIds(bCardIds);
const cCards = cCardIds.map(function(cardId) {
    return cardByCardId[cardId]; 
});

const ownedCards = bCards.concat(cCards);
ownedCards.forEach(function(card) {
    const templateId = card.templateId;
    const dexCard = dexData[templateId];
    
    if (dexCard == null) {
        setScriptError("Player owns card not in all templates list.");
    }
    
    if (dexCard.dexStatus !== DEX_TEMPLATE_OWNED_NOW) {
        dexCard.dexStatus = DEX_TEMPLATE_OWNED_NOW;
    }
    
    dexData[templateId] = dexCard;
});

const error = dexDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

/*
 * DexCard: {
 *   dexStatus: int,
 *     - 0 = template is unknown
 *     - 1 = template is known
 *     - 2 = template has been owned before
 *     - 3 = template is owned now
 *   templateId: string,
 * }
 *
 * DexCard: {
 *   status: int,
 *     - 0 = template is unknown
 *     - 1 = template is known
 *     - 2 = template has been owned before
 *     - 3 = template is owned now
 *   template: Template,
 * }
 */

const DEX_CARD_FIELDS = [
    "dexStatus",
    "templateId",
];

const templateIds = Object.keys(dexData);
const dexCards = templateIds.map(function(templateId) {
    return {
        templateId: templateId,
        dexStatus: dexData[templateId].dexStatus,
    };
});

const dexInstances = getInstancesByCards(dexCards, DEX_CARD_FIELDS);

Spark.setScriptData("dexCards", dexInstances);
