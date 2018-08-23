const LootBox = artifacts.require("./LootBox.sol");
const BlockMiner = artifacts.require("./BlockMiner.sol");

contract('LootBox', function(accounts) {

  let contract;
  let miner;
  let blocksPerDay;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  const categoryOnePrice = 12e+15;
  const categoryTwoPrice = 24e+15;
  const categoryThreePrice = 48e+15;

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await LootBox.new();
    });

    it ("should not allow buy box with insufficent payment", async function() {
      let transaction = null;

      try {
        transaction = await contract.buyBoxes(
          buyer,
          buyer,
          0,
          1,
          {
            from: buyer,
            value: 12e+5,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    // BEFORE NOT BEFORE EACH
    before(async function() {
      contract = await LootBox.new();
      miner = await BlockMiner.new();
      blocksPerDay = await contract.blocksPerDay.call();
    });

    it ("should have full discount for one box on first day", async function() {
      const price = await contract.calculatePrice.call(
        categoryOnePrice,
        1
      );
      assert.equal(price, categoryOnePrice * (1 - 0.21));
    });

    it ("should have full discount for multiple boxes on first day", async function() {
      const price = await contract.calculatePrice.call(
        categoryOnePrice,
        6
      );
      assert.equal(price, categoryOnePrice * (1 - 0.21) * 6);

      for (var i = 0; i < blocksPerDay / 2; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should still have full discount for multiple boxes after half first day", async function() {
      const price = await contract.calculatePrice.call(
        categoryOnePrice,
        6
      );
      assert.equal(price, categoryOnePrice * (1 - 0.21) * 6);

      for (var i = 0; i < blocksPerDay / 2; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have less than full discount for multiple boxes after first day", async function() {
      const price = await contract.calculatePrice.call(
        categoryThreePrice,
        6
      );
      assert.equal(price, categoryThreePrice * (1 - 0.20) * 6);

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have even less than full discount for multiple boxes after first two days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        6
      );
      assert.equal(price, categoryTwoPrice * (1 - 0.19) * 6);

      for (var i = 0; i < blocksPerDay * 19; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have no discount for multiple boxes after 19 days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        6
      );
      assert.equal(price, categoryTwoPrice * 6);
    });
  });
});
