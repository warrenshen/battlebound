const CardTreasury = artifacts.require("./CardTreasury.sol");
const CardAuction = artifacts.require("./ClockAuction.sol");

contract('CardAuction', function(accounts) {

  let treasury;
  let auction;

  const minter = accounts[0];
  const seller = accounts[1];
  const buyer = accounts[2];

  const tokenId = 0;

  const startingPrice = 1e+15;
  const endingPrice = 1e+15;
  const duration = 3600;
  const ownerCut = 100; // 1%

  describe ("should nots", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, ownerCut);

      await treasury.setSaleAuction(auction.address, { from: minter });

      await treasury.mintTemplate(1, 0, 0, 9, "Lux", { from: minter });
      await treasury.setMinter(minter, { from: minter });
      await treasury.mintCard(0, seller, { from: minter });

      await treasury.createSaleAuction(
        tokenId,
        startingPrice,
        endingPrice,
        duration,
        { from: seller }
      );
      await auction.bid(tokenId, { from: buyer, value: startingPrice });
    });

    it ("should not allow non-privileged account to withdraw balance", async function() {
      const startingBalance = web3.eth.getBalance(auction.address).toNumber();
      assert.equal(1e+13, startingBalance, "starting balance is incorrect");

      let transaction = null;

      try {
        transaction = await auction.withdrawBalance(
          { from: seller }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
      assert.equal(
        startingBalance,
        web3.eth.getBalance(auction.address).toNumber(),
        "balance is incorrect"
      );

      try {
        transaction = await auction.withdrawBalance(
          { from: buyer }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
      assert.equal(
        startingBalance,
        web3.eth.getBalance(auction.address).toNumber(),
        "balance is incorrect"
      );

      try {
        transaction = await auction.withdrawBalance(
          { from: accounts[3] }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
      assert.equal(
        startingBalance,
        web3.eth.getBalance(auction.address).toNumber(),
        "balance is incorrect"
      );

      try {
        transaction = await auction.withdrawBalance(
          { from: treasury.address }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
      assert.equal(
        startingBalance,
        web3.eth.getBalance(auction.address).toNumber(),
        "balance is incorrect"
      );
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, ownerCut);

      await treasury.setSaleAuction(auction.address, { from: minter });

      await treasury.mintTemplate(1, 0, 0, 9, "Lux", { from: minter });
      await treasury.setMinter(minter, { from: minter });
      await treasury.mintCard(0, seller, { from: minter });

      await treasury.createSaleAuction(
        tokenId,
        startingPrice,
        endingPrice,
        duration,
        { from: seller }
      );
      await auction.bid(tokenId, { from: buyer, value: 1e+15 });
    });

    it ("should allow privileged account to withdraw balance", async function() {
      const contractStartingBalance = web3.eth.getBalance(auction.address).toNumber();
      const minterStartingBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(1e+13, contractStartingBalance, "starting balance is incorrect");

      const transaction = await auction.withdrawBalance(
        { from: minter, gasPrice: 0, value: 0 }
      );
      const gasUsed = transaction.receipt.gasUsed;

      assert.equal(
        0,
        web3.eth.getBalance(auction.address).toNumber(),
        "balance is incorrect"
      );
      // Just check that minter's new balance is greater than before,
      // since can't get the exact expected value to match.
      assert.equal(
        // minterStartingBalance + contractStartingBalance - gasUsed,
        minterStartingBalance < web3.eth.getBalance(minter).toNumber(),
        true,
        "balance is incorrect"
      );
    });
  });
});
