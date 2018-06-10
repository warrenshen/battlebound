// ====================================================================================================
//
// Cloud Code for AuctionModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("OnChainModule");

function syncAuctionByTokenId(tokenId) {
    const auctionOnChain = fetchAuctionByCardInt(tokenId);
    
    const API = Spark.getGameDataService();
    
    const bCardId = "B" + tokenId.toString();
    const cardDataItem = API.getItem("Card", bCardId).document();
    
    const cardData = cardDataItem.getData();
    if (!cardData.auction) {
        cardData.auction = {};
    }
    const auction = cardData.auction;
    
    auction.seller = auctionOnChain.seller;
    auction.startingPrice = auctionOnChain.startingPrice;
    auction.endingPrice = auctionOnChain.endingPrice;
    auction.duration = auctionOnChain.duration;
    auction.startedAt = auctionOnChain.startedAt;
    
    const error = cardDataItem.persistor().persist().error();
    if (error) {
        Spark.setScriptError("ERROR", error);
        Spark.exit();
    }
}
