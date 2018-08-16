const UniformPriceAuction = artifacts.require("./UniformPriceAuction.sol");
const BlockMiner = artifacts.require("./BlockMiner.sol");

contract('UniformPriceAuction', function(accounts) {

  let contract;
  let miner;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  const N = 3;
  const minimumBid = 1e17;
  const minimumIncrease = 1e16;

  describe ("should nots", function() {
    describe ("not in last hour", function() {
      const auctionDuration = 310;
      beforeEach(async function() {
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

      it ("should not increase duration", async function() {
        const transaction = await contract.submitBid(
          0,
          {
            from: buyer,
            value: minimumBid
          }
        );

        const duration = await contract.duration.call();
        assert.equal(duration, auctionDuration);
      });
    });
  });

  describe ("shoulds", function() {
    describe ("almost last hour", function() {
      const auctionDuration = 310;
      // BEFORE NOT BEFORE EACH
      before(async function() {
        contract = await UniformPriceAuction.new();
        miner = await BlockMiner.new();
        await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          minimumBid,
          minimumIncrease,
          { from: minter }
        );
      });

      it ("should not allow fulfill bid when auction not over", async function() {
        let transaction = null;

        try {
          transaction = await contract.fulfillBid(
            0,
            { from: minter }
          );
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "response should not exist");
      });

      it ("should not allow update fulfill price when auction not over", async function() {
        let transaction = null;

        try {
          transaction = await contract.updateFulfillPrice(
            0,
            { from: minter }
          );
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "response should not exist");
      });

      it ("should not increase duration", async function() {
        let transaction;

        transaction = await contract.submitBid(
          0,
          {
            from: buyer,
            value: minimumBid
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const duration = await contract.duration.call();
        assert.equal(duration, auctionDuration);

        const blockStart = await contract.blockStart.call();
        assert.equal(blockStart > 0, true, "block start should be greater than 0");

        const auctionEndBlock = await contract.auctionEndBlock.call();
        assert.equal(auctionEndBlock.valueOf(), parseInt(blockStart.valueOf()) + parseInt(duration.valueOf()));

        for (var i = 0; i < 10; i += 1) {
          transaction = await miner.mine();
        }
      });

      it ("should increase duration", async function() {
        const transaction = await contract.submitBid(
          1,
          {
            from: buyer,
            value: minimumBid
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const duration = await contract.duration.call();
        assert.equal(duration, auctionDuration + 300);

        const blockStart = await contract.blockStart.call();
        assert.equal(blockStart > 0, true, "block start should be greater than 0");

        const auctionEndBlock = await contract.auctionEndBlock.call();
        assert.equal(auctionEndBlock.valueOf(), parseInt(blockStart.valueOf()) + parseInt(duration.valueOf()));
      });
    });

    describe ("in last hour", function() {
      const auctionDuration = 150;
      beforeEach(async function() {
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

      it ("should increase duration", async function() {
        const transaction = await contract.submitBid(
          0,
          {
            from: buyer,
            value: minimumBid
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const duration = await contract.duration.call();
        assert.equal(duration, auctionDuration + 300);
      });
    });
  });
});
