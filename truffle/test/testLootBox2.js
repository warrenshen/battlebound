const LootBox = artifacts.require("./LootBox.sol");
const BlockMiner = artifacts.require("./BlockMiner.sol");

contract('LootBox', function(accounts) {

  let contract;
  let miner;
  let blocksPerDay;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  let categoryZeroPrice;
  let categoryOnePrice;
  let categoryTwoPrice;
  let categoryThreePrice;

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await LootBox.new();
      blocksPerDay = await contract.blocksPerDay.call();
      categoryZeroPrice = await contract.priceByCategory.call(0);
      categoryOnePrice = await contract.priceByCategory.call(1);
      categoryTwoPrice = await contract.priceByCategory.call(2);
      categoryThreePrice = await contract.priceByCategory.call(3);
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
            value: 12e+3,
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
    });

    it ("should have full discount for one box on first day", async function() {
      const price = await contract.calculatePrice.call(
        categoryOnePrice,
        1
      );
      assert.equal(price, categoryOnePrice * (1 - 0.21));
    });

    it ("should have full discount for multiple box on first day", async function() {
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

    it ("should have less than full discount for one boxes after first day", async function() {
      const price = await contract.calculatePrice.call(
        categoryOnePrice,
        1
      );
      assert.equal(price, categoryOnePrice * (1 - 0.18));

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have even less than full discount for one box after first two days", async function() {
      const price = await contract.calculatePrice.call(
        categoryOnePrice,
        1
      );
      assert.equal(price, categoryOnePrice * (1 - 0.15));

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have correct discount for one box after first three days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        1
      );
      assert.equal(price, categoryTwoPrice * (1 - 0.12));

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have correct discount for multiple boxes after first four days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        6
      );
      assert.equal(price, categoryTwoPrice * (1 - 0.09) * 6);

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have correct discount for multiple boxes after first five days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        6
      );
      assert.equal(price, categoryTwoPrice * (1 - 0.06) * 6);

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have correct discount for multiple boxes after first six days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        6
      );
      assert.equal(price, categoryTwoPrice * (1 - 0.03) * 6);

      for (var i = 0; i < blocksPerDay; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should have no discount for multiple boxes after 7 days", async function() {
      const price = await contract.calculatePrice.call(
        categoryTwoPrice,
        6
      );
      assert.equal(price, categoryTwoPrice * 6);
    });
  });
});
