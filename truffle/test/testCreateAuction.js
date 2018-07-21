const CardTreasury = artifacts.require("./CardTreasury.sol");
const CardAuction = artifacts.require("./ClockAuction.sol");

contract('CardAuction', function(accounts) {

  let treasury;
  let auction;

  const minter = accounts[0];
  const seller = accounts[1];

  const tokenId = 0;

  const startingPrice = 1000;
  const endingPrice = 100;
  const duration = 3600;
  const ownerCut = 100; // 1%

  describe ("should nots", function() {
    beforeEach(async function() {
      treasury = await CardTreasury.new();
      auction = await CardAuction.new(treasury.address, ownerCut);

      await treasury.createTemplate(1, 0, 9, "T1", { from: minter });
      await treasury.mintCard(0, seller, { from: minter });
    });

    it ("should not allow normal account to create an auction", async function() {
      let transaction = null;

      try {
        transaction = await auction.createAuction(
          tokenId,
          startingPrice,
          endingPrice,
          duration,
          seller,
          { from: seller }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");

      let price = null;

      try {
        price = await auction.getPrice.call(tokenId);
      } catch (error) {
        err = error;
      }
      assert.equal(price, null, "price should not exist");
    });

    it ("should not allow normal account to create an auction even with approval", async function() {
      let transaction = null;

      await treasury.approve(
        auction.address,
        tokenId,
        { from: seller }
      );

      try {
        transaction = await auction.createAuction(
          tokenId,
          startingPrice,
          endingPrice,
          duration,
          seller,
          { from: seller }
        );
      } catch (error) {
        err = error;
      }

      let price = null;

      try {
        price = await auction.getPrice.call(tokenId);
      } catch (error) {
        err = error;
      }
      assert.equal(price, null, "price should not exist");
    });
  });
});
