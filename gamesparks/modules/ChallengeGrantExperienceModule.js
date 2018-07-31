// ====================================================================================================
//
// This module contains a function to be called in ChallengeWon and ChallengeLost user messages.
// It gives the player's cards that were active during the challenge experience.
// It expects two variables to be defined, `challengeStateData` to access the challenge's moves
// and `playerId` to know which player's cards to add experience to.
//
// ====================================================================================================
require("DeckModule");

const MATCH_TYPE_CASUAL = 0;
const MATCH_TYPE_RANKED = 1;

const SHORT_CODE_TO_MATCH_TYPE = {
    CasualChallenge: MATCH_TYPE_CASUAL,
    RankedChallenge: MATCH_TYPE_RANKED,
};

const LEVEL_TO_EXP_MAX = {
    1: 5,
    2: 10,
    3: 50,
    4: 250,
    5: 1000
};

/**
 * @return array - array of Card-like objects with levelPrevious and expPrevious fields.
 **/ 
function grantExperienceByPlayerAndChallenge(playerId, challengeId) {
    const API = Spark.getGameDataService();

    const challenge = Spark.getChallenge(challengeId);
    if (challenge == null) {
        setScriptError("Invalid challenge ID.");
    }
    
    // This lock call MUST be before the `challenge.getPrivateData` call below.
    Spark.lockKey(challengeId, 3000);
    const challengeStateData = challenge.getPrivateData("data");
    const expCardIds = challengeStateData.expCardIdsByPlayerId[playerId];
    
    if (challengeStateData.isFinalByPlayerId[playerId] == 1) {
        setScriptErrorWithUnlockKey(challengeId, "Challenge already processed - cannot grant experience again.");
    } else {
        challengeStateData.isFinalByPlayerId[playerId] = 1;
        challenge.setPrivateData("data", challengeStateData);
        Spark.unlockKeyFully(challengeId);
    }
    
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    const decksData = decksDataItem.getData();
    
    const cardsDataQuery = API.S("id").in(decksData.bCardIds);
    const cardsDataQueryResult = API.queryItems("Card", cardsDataQuery);
    const cardsDataQueryResultError = cardsDataQueryResult.error();
    
    const bCards = [];
    
    if (cardsDataQueryResultError) {
        setScriptError(cardsDataQueryResultError);
    } else {
        const cardsDataCursor = cardsDataQueryResult.cursor();
        while (cardsDataCursor.hasNext()) {
            bCards.push(cardsDataCursor.next().getData());
        }
    }
    
    const response = handleGrantExperience(expCardIds, decksData, bCards);
    const newDecksData = response[0];
    const newBCards = response[1];
    const expCards = response[2];
    
    newBCards.forEach(function(newBCard) {
        var bCardId = newBCard.id;
        
        var cardDataItem = API.getItem("Card", bCardId);
        var cardData = cardDataItem.getData();
        
        var error = cardDataItem.persistor().persist().error();
        if (error) {
            setScriptError(error);
        }
    });
    
    decksData.cardByCardId = newDecksData.cardByCardId;
    var error = decksDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
    
    const CARD_FIELDS = [
        "id",
        "level",
        "levelPrevious",
        "exp",
        "expMax",
        "expPrevious",
    ];
    
    return getInstancesByCards(expCards, CARD_FIELDS);
}

/**
 * @return array - array of three elements [PlayerDecks instance, array of b-cards, array of exp cards]
 **/
function handleGrantExperience(
    expCardIds,
    decksData,
    bCards
) {
    const newDecksData = JSON.parse(JSON.stringify(decksData));
    
    const bCardIdToBCard = {};
    bCards.forEach(function(bCard) { bCardIdToBCard[bCard.id] = bCard });
    
    const expBCardIds = expCardIds.filter(function(expCardId) { return expCardId.indexOf("B") === 0 });
    const expCCardIds = expCardIds.filter(function(expCardId) { return expCardId.indexOf("C") === 0 });
    
    const expCards = [];
    const newBCards = [];
    
    expBCardIds.forEach(function(bCardId) {
        if (!bCardIdToBCard[bCardId]) {
            Spark.getLog().info("Card ID " + bCardId + " missing." + JSON.stringify(bCardIdToBCard));
            return;
        }
        
        var bCard = bCardIdToBCard[bCardId];
        
        var expCard = {};
        expCard.id = bCard.id;
        expCard.levelPrevious = bCard.level;
        expCard.expPrevious = bCard.exp || 0;
        
        if (!bCard.exp) {
            bCard.exp = 0;
        }
        if (!bCard.expMax) {
            bCard.expMax = LEVEL_TO_EXP_MAX[1];
        }
        
        bCard.exp += 1;
        if (bCard.exp >= bCard.expMax) {
            bCard.level += 1;
            bCard.exp = 0;
            bCard.expMax = LEVEL_TO_EXP_MAX[bCard.level];
        }
        
        expCard.level = bCard.level;
        expCard.exp = bCard.exp;
        expCard.expMax = bCard.expMax;
        
        expCards.push(expCard);
        newBCards.push(bCard);
    });
    
    expCCardIds.forEach(function(cardId) {
        if (!newDecksData.cardByCardId[cardId]) {
            Spark.getLog().info("Card ID " + cardId + " missing." + JSON.stringify(newDecksData));
            return;
        }
        
        var cCard = newDecksData.cardByCardId[cardId];
        
        var expCard = {};
        expCard.id = cCard.id;
        expCard.templateId = cCard.templateId;
        expCard.levelPrevious = cCard.level;
        expCard.expPrevious = cCard.exp || 0;
        
        if (!cCard.exp) {
            cCard.exp = 0;
        }
        if (!cCard.expMax) {
            cCard.expMax = LEVEL_TO_EXP_MAX[1];
        }
        
        cCard.exp += 1;
        if (cCard.exp >= cCard.expMax) {
            cCard.level += 1;
            cCard.exp = 0;
            cCard.expMax = LEVEL_TO_EXP_MAX[cCard.level];
        }
        
        expCard.level = cCard.level;
        expCard.exp = cCard.exp;
        expCard.expMax = cCard.expMax;
        
        expCards.push(expCard);
    });

    return [newDecksData, newBCards, expCards];
}
