const UniformPriceAuction = artifacts.require("./UniformPriceAuction.sol");

contract('UniformPriceAuction', function(accounts) {

  let contract;

  const minter = accounts[0];
  const bidder = accounts[1];

  const N = 6;
  const auctionDuration = 3600;
  const minimumBid = 1e17;
  const minimumIncrease = 1e16;

  describe ("should nots", function() {
    before(async function() {
      contract = await UniformPriceAuction.new();
    });

    it ("should not allow initialize with invalid N", async function() {
      let transaction = null;

      try {
        transaction = await contract.initializeAuction(
          "UniformPriceAuction",
          0,
          auctionDuration,
          minimumBid,
          minimumIncrease,
          { from: minter }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow initialize with invalid duration", async function() {
      let transaction = null;

      try {
        transaction = await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          0,
          minimumBid,
          minimumIncrease,
          { from: minter }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow initialize with invalid minimum bid", async function() {
      let transaction = null;

      try {
        transaction = await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          0,
          minimumIncrease,
          { from: minter }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow initialize with invalid minimum increase", async function() {
      let transaction = null;

      try {
        transaction = await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          minimumBid,
          0,
          { from: minter }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow unprivileged address to initialize auction", async function() {
      let transaction = null;

      try {
        transaction = await contract.initializeAuction(
          "UniformPriceAuction",
          N,
          auctionDuration,
          minimumBid,
          minimumIncrease,
          { from: bidder }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    it ("should initialize with valid params", async function() {
      const transaction = await contract.initializeAuction(
        "UniformPriceAuction",
        N,
        auctionDuration,
        minimumBid,
        minimumIncrease,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const name = await contract.name.call();
      assert.equal(name, "UniformPriceAuction", "name is not correct");

      const duration = await contract.duration.call();
      assert.equal(duration, auctionDuration);

      const blockStart = await contract.blockStart.call();
      assert.equal(blockStart > 0, true, "block start should be greater than 0");

      const auctionEndBlock = await contract.auctionEndBlock.call();
      assert.equal(auctionEndBlock.valueOf(), parseInt(blockStart.valueOf()) + parseInt(duration.valueOf()));
    });
  });
});
