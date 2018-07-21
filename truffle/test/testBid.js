const CardTreasury = artifacts.require("./CardTreasury.sol");
const CardAuction = artifacts.require("./ClockAuction.sol");

contract('CardAuction', function(accounts) {

  let treasury;
  let auction;

  const minter = accounts[0];
  const seller = accounts[1];
  const buyer = accounts[2];

  const tokenId = 0;

  const startingPrice = 1e+4;
  const endingPrice = 1e+4;
  const duration = 3600;
  const ownerCut = 100; // 1%

  describe ("should nots", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, ownerCut);

      await treasury.setSaleAuction(auction.address, { from: minter });

      await treasury.createTemplate(1, 0, 9, "T1", { from: minter });
      await treasury.mintCard(tokenId, seller, { from: minter });

      await treasury.createSaleAuction(
        tokenId,
        startingPrice,
        endingPrice,
        duration,
        { from: seller }
      );
    });

    it ("should not allow bid on token not on auction", async function() {
      let transaction = null;

      try {
        transaction = await auction.bid(1, { from: buyer, value: 1e+4 });
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
    });

    it ("should not allow bid less than current auction price", async function() {
      let transaction = null;

      try {
        transaction = await auction.bid(0, { from: buyer, value: 1e+2 });
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, ownerCut);

      await treasury.setSaleAuction(auction.address, { from: minter });

      await treasury.createTemplate(1, 0, 9, "T1", { from: minter });
      await treasury.mintCard(tokenId, seller, { from: minter });

      await treasury.createSaleAuction(
        tokenId,
        startingPrice,
        endingPrice,
        duration,
        { from: seller }
      );
    });

    it ("should allow bid equal to current auction price", async function() {
      const transaction = await auction.bid(0, { from: buyer, value: 1e+4 });
      const log = transaction.logs[0];

      assert.equal(log.event, "AuctionSuccessful", "expected an AuctionSuccessful event");
      assert.equal(log.args.tokenId, tokenId, "token id is incorrect");
      assert.equal(log.args.totalPrice.toNumber(), startingPrice, "total price is incorrect");
      assert.equal(log.args.winner, buyer, "winner is incorrect");
    });

    it ("should allow bid higher to current auction price", async function() {
      const transaction = await auction.bid(0, { from: buyer, value: 1e+5 });
      const log = transaction.logs[0];

      assert.equal(log.event, "AuctionSuccessful", "expected an AuctionSuccessful event");
      assert.equal(log.args.tokenId, tokenId, "token id is incorrect");
      assert.equal(log.args.totalPrice.toNumber(), startingPrice, "total price is incorrect");
      assert.equal(log.args.winner, buyer, "winner is incorrect");
    });

    it ("should send token to buyer on successful bid", async function() {
      let owner;

      owner = await treasury.ownerOf.call(tokenId);
      assert.equal(owner.valueOf(), auction.address, "owner is incorrect");

      const transaction = await auction.bid(tokenId, { from: buyer, value: 1e+4 });

      owner = await treasury.ownerOf.call(tokenId);
      assert.equal(owner.valueOf(), buyer, "owner is incorrect");
    });

    it ("should retain cut of bid for auction contract", async function() {
      const startingBalance = web3.eth.getBalance(auction.address).toNumber();
      const transaction = await auction.bid(tokenId, { from: buyer, value: 1e+4 });
      const endingBalance = web3.eth.getBalance(auction.address).toNumber();

      assert.equal(startingBalance + (1e+4 * ownerCut / 10000), endingBalance, "ending balance is incorrect");
    });

    it ("should send leftover eth to buyer", async function() {
      const startingBalance = web3.eth.getBalance(buyer).toNumber();
      const transaction = await auction.bid(
        tokenId,
        { from: buyer, gasPrice: 1, value: 1e+5 }
      );
      const gasUsed = transaction.receipt.gasUsed;
      const endingBalance = web3.eth.getBalance(buyer).toNumber();

      assert.equal(
        startingBalance - 1e+4 - (1e+4 * ownerCut / 10000) - gasUsed,
        endingBalance,
        "ending balance is incorrect"
      );
    });
  });
});
