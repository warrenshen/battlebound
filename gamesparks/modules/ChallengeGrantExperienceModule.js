// ====================================================================================================
//
// This module contains a function to be called in ChallengeWon and ChallengeLost user messages.
// It gives the player's cards that were active during the challenge experience.
// It expects two variables to be defined, `challengeStateData` to access the challenge's moves
// and `playerId` to know which player's cards to add experience to.
//
// ====================================================================================================
require("DeckModule");
require("ChallengeMovesModule");

/**
 * @return array - array of Card-like objects with levelPrevious and expPrevious fields.
 **/ 
function grantExperienceByPlayerAndChallenge(playerId, challengeId) {
    const API = Spark.getGameDataService();

    const challenge = Spark.getChallenge(challengeId);
    const challengeStateData = challenge.getPrivateData("data");
    
    if (challengeStateData.isFinal == 1) {
        setScriptError("Challenge already processed - cannot grant experience again.");
    } else {
        challengeStateData.isFinal = 1;
        challenge.setPrivateData("data", challengeStateData);
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
    
    const response = handleGrantExperience(challengeStateData, playerId, decksData, bCards);
    const newDecksData = response[0];
    const newBCards = response[1];
    const resultCards = response[2];
    
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
    
    return getInstancesByCards(resultCards, CARD_FIELDS);
}

/**
 * @return array - array of three elements [PlayerDecks instance, array of b-cards, array of exp cards]
 **/
function handleGrantExperience(
    challengeStateData,
    playerId,
    decksData,
    bCards
) {
    const newDecksData = JSON.parse(JSON.stringify(decksData));
    
    const bCardIdToBCard = {};
    bCards.forEach(function(bCard) { bCardIdToBCard[bCard.id] = bCard });
    
    const moves = challengeStateData.moves;
    const playerMoves = moves.filter(function(move) { return move.playerId === playerId });
    
    const expMoveCategories = [
        MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        MOVE_CATEGORY_PLAY_SPELL_UNTARGETED,
        MOVE_CATEGORY_CARD_ATTACK,
    ];
    
    const expMoves = playerMoves.filter(function(move) { return expMoveCategories.indexOf(move.category) >= 0 });
    const expCardIds = expMoves.map(function(move) {
        var challengeCardId = move.attributes.cardId;
        var splitResponse = challengeCardId.split("-");
        if (splitResponse.length != 3) {
            setScriptError("Split response of challenge card ID is not length 3.");
        }
        
        return splitResponse[0];
    });
    
    // The following logic is so we can dedup card IDs.
    const expBCardIdsDict = {};
    expCardIds.forEach(function(cardId) {
        if (cardId.indexOf("B") === 0) {
            expBCardIdsDict[cardId] = 0;
        }
    });
    const expCCardIdsDict = {};
    expCardIds.forEach(function(cardId) {
        if (cardId.indexOf("C") === 0) {
            expCCardIdsDict[cardId] = 0;
        }
    });
    
    const expBCardIds = Object.keys(expBCardIdsDict);
    const expCCardIds = Object.keys(expCCardIdsDict);
    
    const expCards = [];
    const newBCards = [];
    
    expBCardIds.forEach(function(bCardId) {
        if (!bCardIdToBCard[bCardId]) {
            setScriptError("Card ID " + bCardId + " missing." + JSON.stringify(bCardIdToBCard));
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
            bCard.expMax = 10; // TODO;
        }
        
        bCard.exp += 1;
        if (bCard.exp >= bCard.expMax) {
            bCard.level += 1;
            bCard.exp = 0;
        }
        
        expCard.level = bCard.level;
        expCard.exp = bCard.exp;
        expCard.expMax = bCard.expMax;
        
        expCards.push(expCard);
        newBCards.push(bCard);
    });
    
    expCCardIds.forEach(function(cardId) {
        if (!newDecksData.cardByCardId[cardId]) {
            setScriptError("Card ID " + cardId + " missing." + JSON.stringify(newDecksData));
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
            cCard.expMax = 10; // TODO;
        }
        
        cCard.exp += 1;
        if (cCard.exp >= cCard.expMax) {
            cCard.level += 1;
            cCard.exp = 0;
        }
        
        expCard.level = cCard.level;
        expCard.exp = cCard.exp;
        expCard.expMax = cCard.expMax;
        
        expCards.push(expCard);
    });

    return [newDecksData, newBCards, expCards];
}
