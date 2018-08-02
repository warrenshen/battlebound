// ====================================================================================================
//
// Cloud Code for AuctionModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("DeckModule");

function findAuctionsExceptSeller(address) {
    address = prefixHex(address.toLowerCase());

    const API = Spark.getGameDataService();
    
    const cardsDataQuery = API.S("seller").ne(address);
    const cardsDataQueryResult = API.queryItems("Card", cardsDataQuery);
    const cardsDataQueryResultError = cardsDataQueryResult.error();
    
    const cards = [];
    
    if (cardsDataQueryResultError) {
        setScriptError(cardsDataQueryResultError);
    } else {
        const cardsDataCursor = cardsDataQueryResult.cursor();
        while (cardsDataCursor.hasNext()) {
            cards.push(cardsDataCursor.next().getData());
        }
    }
    
    const AUCTIONABLE_CARD_FIELDS = [
        "id",
        "level",
        "auction",
        "seller",
    ];
    const instances = getInstancesByCards(cards, AUCTIONABLE_CARD_FIELDS);
    return instances.filter(function(instance) {
        return instance.seller != "0x" && instance.seller != null;
    });
}


function findAuctionsBySeller(address) {
    address = prefixHex(address.toLowerCase());
    
    const API = Spark.getGameDataService();
    
    const cardsDataQuery = API.S("seller").eq(address);
    const cardsDataQueryResult = API.queryItems("Card", cardsDataQuery);
    const cardsDataQueryResultError = cardsDataQueryResult.error();
    
    const cards = [];
    
    if (cardsDataQueryResultError) {
        setScriptError(cardsDataQueryResultError);
    } else {
        const cardsDataCursor = cardsDataQueryResult.cursor();
        while (cardsDataCursor.hasNext()) {
            cards.push(cardsDataCursor.next().getData());
        }
    }
    
    const AUCTIONABLE_CARD_FIELDS = [
        "id",
        "level",
        "auction",
        "seller",
    ];
    const instances = getInstancesByCards(cards, AUCTIONABLE_CARD_FIELDS);
    return instances;
}

function removeAuctionByTokenId(tokenId) {
    const API = Spark.getGameDataService();
    
    const bCardId = "B" + tokenId.toString();
    const cardDataItem = API.getItem("Card", bCardId).document();
    
    const cardData = cardDataItem.getData();
    cardData.seller = null;
    cardData.auction = null;
    
    const error = cardDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
}

function syncAuctionByTokenId(tokenId) {
    const auctionOnChain = fetchAuctionByCardInt(tokenId);
    
    const API = Spark.getGameDataService();
    
    const bCardId = "B" + tokenId.toString();
    var cardDataItem = API.getItem("Card", bCardId).document();
    if (cardDataItem === null) {
        cardDataItem = createBCardByBCardId(bCardId);
    }
    const cardData = cardDataItem.getData();
    
    // We put the auction seller on the card directly
    // so we can query it (it is an indexed field).
    cardData.seller = prefixHex(auctionOnChain.seller.toLowerCase());
        
    if (!cardData.auction) {
        cardData.auction = {};
    }
    const auction = cardData.auction;
    
    auction.startingPrice = auctionOnChain.startingPrice;
    auction.endingPrice = auctionOnChain.endingPrice;
    auction.duration = auctionOnChain.duration;
    auction.startedAt = auctionOnChain.startedAt;
    
    const error = cardDataItem.persistor().persist().error();
    if (error) {
        setScriptError(error);
    }
}
