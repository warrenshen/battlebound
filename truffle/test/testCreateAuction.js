const CardTreasury = artifacts.require("./CardTreasury.sol");
const CardAuction = artifacts.require("./ClockAuction.sol");

contract('CardAuction', function(accounts) {

  let treasury;
  let auction;

  describe ("should nots", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, 100);

      await treasury.createTemplate(1, 0, 9, "T1", { from: accounts[0] });
      await treasury.mintCard(0, accounts[1], { from: accounts[0] });
    });

    it ("should not allow auction without approval", async function() {
      let transaction = null;
      const cardId = 0;

      try {
        transaction = await auction.createAuction(
          cardId,
          1000,
          100,
          3600,
          accounts[1],
          { from: accounts[1] }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");

      try {
        transaction = await auction.getAuction.call(cardId);
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
    });

    it ("should not allow non-owner of card to create an auction for card", async function() {
      let transaction = null;
      const cardId = 0;

      await treasury.approve(
        auction.address,
        cardId,
        { from: accounts[1] }
      );

      try {
        transaction = await auction.createAuction(
          cardId,
          1000,
          100,
          3600,
          accounts[2],
          { from: accounts[2] }
        );
      } catch (error) {
        err = error;
      }

      try {
        transaction = await auction.getAuction.call(cardId);
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, 100);

      await treasury.createTemplate(1, 0, 9, "T1", { from: accounts[0] });
      await treasury.mintCard(0, accounts[1], { from: accounts[0] });
    });

    it ("should allow owner of card to create an auction for card", async function() {
      const cardId = 0;
      let transaction;

      transaction = await treasury.approve(
        auction.address,
        cardId,
        { from: accounts[1] }
      );

      const startingPrice = 1000;
      const endingPrice = 100;
      const duration = 3600;

      transaction = await auction.createAuction(
        cardId,
        startingPrice,
        endingPrice,
        duration,
        accounts[1],
        { from: accounts[1] }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "AuctionCreated", "expected an AuctionCreated event");
      assert.equal(log.args.tokenId, cardId, "AuctionCreated event token id is incorrect");
      assert.equal(log.args.startingPrice, startingPrice, "AuctionCreated event starting price is incorrect");
      assert.equal(log.args.endingPrice, endingPrice, "AuctionCreated event ending price is incorrect");
      assert.equal(log.args.duration, duration, "AuctionCreated event duration is incorrect");
    });
  });
});
