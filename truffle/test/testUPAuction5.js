const UniformPriceAuction = artifacts.require("./UniformPriceAuction.sol");

contract('UniformPriceAuction', function(accounts) {

  let contract;
  let miner;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  const N = 6;
  const auctionDuration = 3600;
  const minimumBid = 1e16;
  const minimumIncrease = 1e15;

  describe ("shoulds", function() {
    describe ("different bid amounts", function() {
      // BEFORE NOT BEFORE EACH
      before(async function() {
        contract = await UniformPriceAuction.new();
        await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          minimumBid,
          minimumIncrease,
          { from: minter }
        );
        await contract.submitBid(
          0,
          0,
          {
            from: buyer,
            value: minimumBid
          }
        );
        await contract.submitBid(
          0,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease * 10
          }
        );
        await contract.submitBid(
          0,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 20
          }
        );
        await contract.submitBid(
          1,
          0,
          {
            from: buyerTwo,
            value: minimumBid
          }
        );
        await contract.submitBid(
          2,
          0,
          {
            from: buyerTwo,
            value: minimumBid
          }
        );
        await contract.submitBid(
          1,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease
          }
        );
        await contract.submitBid(
          3,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease
          }
        );
        await contract.submitBid(
          4,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease * 2
          }
        );
        await contract.submitBid(
          3,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease * 3
          }
        );
        await contract.submitBid(
          5,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease * 12
          }
        );
      });

      it ("should support indices of three lowest bids", async function() {
        const [
          lowestBidIndexOne,
          lowestBidIndexTwo,
          lowestBidIndexThree
        ] = await contract.indicesOfThreeLowestBids.call();

        assert.equal(lowestBidIndexOne, 2, "lowest bid index is incorrect");
        assert.equal(lowestBidIndexTwo, 1, "lowest bid index is incorrect");
        assert.equal(lowestBidIndexThree, 4, "lowest bid index is incorrect");
      });

      it ("should support highest bid amount", async function() {
        const [highestBidAmount, highestBidBidder] = await contract.highestBid.call();
        assert.equal(highestBidAmount, minimumBid + minimumIncrease * 20);
        assert.equal(highestBidBidder, buyerTwo);
      });
    });

    describe ("same bid amounts", function() {
      // BEFORE NOT BEFORE EACH
      before(async function() {
        contract = await UniformPriceAuction.new();
        await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          minimumBid,
          minimumIncrease,
          { from: minter }
        );
        await contract.submitBid(
          0,
          0,
          {
            from: buyer,
            value: minimumBid
          }
        );
        await contract.submitBid(
          0,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 3
          }
        );
        await contract.submitBid(
          1,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 2
          }
        );
        await contract.submitBid(
          2,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 2
          }
        );
        await contract.submitBid(
          3,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 4
          }
        );
        await contract.submitBid(
          4,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 2
          }
        );
        await contract.submitBid(
          4,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 5
          }
        );
        await contract.submitBid(
          5,
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease * 2
          }
        );
      });

      it ("should support indices of three lowest bids", async function() {
        const [
          lowestBidIndexOne,
          lowestBidIndexTwo,
          lowestBidIndexThree
        ] = await contract.indicesOfThreeLowestBids.call();

        assert.equal(lowestBidIndexOne, 1, "lowest bid index is incorrect");
        assert.equal(lowestBidIndexTwo, 2, "lowest bid index is incorrect");
        assert.equal(lowestBidIndexThree, 5, "lowest bid index is incorrect");
      });

      it ("should support highest bid amount", async function() {
        const [highestBidAmount, highestBidBidder] = await contract.highestBid.call();
        assert.equal(highestBidAmount, minimumBid + minimumIncrease * 5);
        assert.equal(highestBidBidder, buyerTwo);
      });
    });

    describe ("no bids", function() {
      // BEFORE NOT BEFORE EACH
      before(async function() {
        contract = await UniformPriceAuction.new();
        await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          minimumBid,
          minimumIncrease,
          { from: minter }
        );
      });

      it ("should support indices of three lowest bids", async function() {
        const [
          lowestBidIndexOne,
          lowestBidIndexTwo,
          lowestBidIndexThree
        ] = await contract.indicesOfThreeLowestBids.call();
        assert.equal(lowestBidIndexOne, 0, "lowest bid index is incorrect");
        assert.equal(lowestBidIndexTwo, 1, "lowest bid index is incorrect");
        assert.equal(lowestBidIndexThree, 2, "lowest bid index is incorrect");
      });

      it ("should support highest bid amount", async function() {
        const [highestBidAmount, highestBidBidder] = await contract.highestBid.call();
        assert.equal(highestBidAmount, 0);
        assert.equal(highestBidBidder, 0);
      });
    });
  });
});
