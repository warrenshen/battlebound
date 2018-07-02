// ====================================================================================================
//
// This module contains a function to be called in ChallengeWon and ChallengeLost user messages.
// It gives the player's cards that were active during the challenge experience.
// It expects two variables to be defined, `challengeStateData` to access the challenge's moves
// and `playerId` to know which player's cards to add experience to.
//
// ====================================================================================================
require("DeckModule");

/**
 * @return array - array of Card-like objects with levelPrevious and expPrevious fields.
 **/ 
function grantExperienceByPlayerAndChallenge(playerId, challengeId)
{
    const API = Spark.getGameDataService();

    const challenge = Spark.getChallenge(challengeId);
    
    const challengeStateDataItem = API.getItem("ChallengeState", challengeId).document();
    
    if (challengeStateDataItem === null) {
        setScriptError("ChallengeState does not exist.");
    }
    
    const challengeStateData = challengeStateDataItem.getData();
    
    const moves = challengeStateData.moves;
    const playerMoves = moves.filter(function(move) { return move.playerId === playerId });
    
    const expMoveCategories = [
        MOVE_CATEGORY_PLAY_SPELL_GENERAL,
        MOVE_CATEGORY_PLAY_SPELL_TARGETED,
        MOVE_CATEGORY_CARD_ATTACK,
    ];
    
    const expMoves = playerMoves.filter(function(move) { return expMoveCategories.indexOf(move.category) >= 0 });
    const expCardIds = expMoves.map(function(move) { return move.attributes.cardId });
    
    const bExpCardIds = expCardIds.filter(function(cardId) { return cardId.indexOf("B") === 0 });
    const cExpCardIds = expCardIds.filter(function(cardId) { return cardId.indexOf("C") === 0 });
    
    const resultCards = [];
    
    bExpCardIds.forEach(function(cardId) {
        const cardDataItem = API.getItem("Card", bCardId);
        const cardData = cardDataItem.getData();
        
        const resultCard = {};
        resultCard.id = cardData.id;
        resultCard.levelPrevious = cardData.level;
        resultCard.expPrevious = cardData.exp;
        
        if (!cardData.exp) {
            cardData.exp = 0;
        }
        if (!cardData.expMax) {
            cardData.expMax = 10; // TODO;
        }
        
        cardData.exp += 1;
        if (cardData.exp >= cardData.expMax) {
            cardData.level += 1;
            cardData.exp = 0;
        }
        
        resultCard.level = cardData.level;
        resultCard.exp = cardData.exp;
        
        resultCards.push(resultCard);
        
        const error = cardDataItem.persistor().persist().error();
        if (error) {
            setScriptError(error);
        }
    });
    
    const decksDataItem = API.getItem("PlayerDecks", playerId).document();
    const decksData = decksDataItem.getData();
    cExpCardIds.forEach(function(cardId) {
        const cardData = decksData.cardByCardId[cardId];
        
        const resultCard = {};
        resultCard.id = cardData.id;
        resultCard.templateId = resultCard.templateId;
        resultCard.levelPrevious = cardData.level;
        resultCard.expPrevious = cardData.exp;
        
        if (!cardData.exp) {
            cardData.exp = 0;
        }
        if (!cardData.expMax) {
            cardData.expMax = 10; // TODO;
        }
        
        cardData.exp += 1;
        if (cardData.exp >= cardData.expMax) {
            cardData.level += 1;
            cardData.exp = 0;
        }
        
        resultCard.level = cardData.level;
        resultCard.exp = cardData.exp;
        
        resultCards.push(resultCard);
    });
    
    const error = decksDataItem.persistor().persist().error();
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
