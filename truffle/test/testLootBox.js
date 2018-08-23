const LootBox = artifacts.require("./LootBox.sol");

contract('LootBox', function(accounts) {

  let contract;
  let categoryZeroPrice;
  let categoryOnePrice;
  let categoryTwoPrice;
  let categoryThreePrice;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await LootBox.new();
      categoryZeroPrice = await contract.priceByCategory.call(0);
      categoryOnePrice = await contract.priceByCategory.call(1);
      categoryTwoPrice = await contract.priceByCategory.call(2);
      categoryThreePrice = await contract.priceByCategory.call(3);
    });

    it ("should not allow buy boxes with no owner", async function() {
      let transaction = null;

      try {
        transaction = await contract.buyBoxes(
          0,
          0,
          0,
          1,
          {
            from: buyer,
            value: categoryZeroPrice,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow buy boxes with 0 quantity", async function() {
      let transaction = null;

      try {
        transaction = await contract.buyBoxes(
          buyer,
          0,
          0,
          0,
          {
            from: buyer,
            value: categoryZeroPrice,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow buy boxes with 0 value", async function() {
      let transaction = null;

      try {
        transaction = await contract.buyBoxes(
          buyer,
          0,
          0,
          1,
          { from: buyer }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow buy boxes with referrer as self", async function() {
      let transaction = null;

      try {
        transaction = await contract.buyBoxes(
          buyer,
          buyer,
          0,
          1,
          {
            from: buyer,
            value: categoryZeroPrice,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow buy more than 20 boxes", async function() {
      let transaction = null;

      try {
        transaction = await contract.buyBoxes(
          buyer,
          buyer,
          0,
          21,
          {
            from: buyer,
            value: categoryZeroPrice * 21,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      contract = await LootBox.new();
    });

    it ("should allow buy one category 0 box", async function() {
      const transaction = await contract.buyBoxes(
        buyer,
        0,
        0,
        1,
        {
          from: buyer,
          value: categoryZeroPrice,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyer);
      assert.equal(transaction.logs[0].args._referrer, 0);
      assert.equal(transaction.logs[0].args._category, 0);
      assert.equal(transaction.logs[0].args._quantity, 1);

      const blockStart = await contract.blockStart.call();
      assert.equal(blockStart > 0, true);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 1);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy six category 0 boxes", async function() {
      const transaction = await contract.buyBoxes(
        buyer,
        0,
        0,
        6,
        {
          from: buyer,
          value: categoryZeroPrice * 6,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyer);
      assert.equal(transaction.logs[0].args._referrer, 0);
      assert.equal(transaction.logs[0].args._category, 0);
      assert.equal(transaction.logs[0].args._quantity, 6);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 6);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy one category 1 box", async function() {
      const transaction = await contract.buyBoxes(
        buyer,
        0,
        0,
        1,
        {
          from: buyer,
          value: categoryZeroPrice,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyer);
      assert.equal(transaction.logs[0].args._referrer, 0);
      assert.equal(transaction.logs[0].args._category, 0);
      assert.equal(transaction.logs[0].args._quantity, 1);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 1);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy one category 2 box with referrer", async function() {
      const transaction = await contract.buyBoxes(
        buyer,
        buyerTwo,
        2,
        1,
        {
          from: buyer,
          value: categoryTwoPrice,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyer);
      assert.equal(transaction.logs[0].args._referrer, buyerTwo);
      assert.equal(transaction.logs[0].args._category, 2);
      assert.equal(transaction.logs[0].args._quantity, 1);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 1);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy 18 category 2 boxes with referrer", async function() {
      const transaction = await contract.buyBoxes(
        buyerTwo,
        buyer,
        2,
        18,
        {
          from: buyerTwo,
          value: categoryTwoPrice * 18,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyerTwo);
      assert.equal(transaction.logs[0].args._referrer, buyer);
      assert.equal(transaction.logs[0].args._category, 2);
      assert.equal(transaction.logs[0].args._quantity, 18);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 18);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy one category 1 box for someone else", async function() {
      const transaction = await contract.buyBoxes(
        buyerTwo,
        0,
        1,
        1,
        {
          from: buyer,
          value: categoryOnePrice,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyerTwo);
      assert.equal(transaction.logs[0].args._referrer, 0);
      assert.equal(transaction.logs[0].args._category, 1);
      assert.equal(transaction.logs[0].args._quantity, 1);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 1);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy 20 category 1 boxes for someone else", async function() {
      const transaction = await contract.buyBoxes(
        buyerTwo,
        0,
        1,
        20,
        {
          from: buyer,
          value: categoryOnePrice * 20,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyerTwo);
      assert.equal(transaction.logs[0].args._referrer, 0);
      assert.equal(transaction.logs[0].args._category, 1);
      assert.equal(transaction.logs[0].args._quantity, 20);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 20);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });

    it ("should allow buy 1 category 3 box with referrer", async function() {
      const transaction = await contract.buyBoxes(
        buyer,
        buyerTwo,
        3,
        1,
        {
          from: buyer,
          value: categoryThreePrice,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BoxesBought", "expected a BoxesBought event");
      assert.equal(transaction.logs[0].args._owner, buyer);
      assert.equal(transaction.logs[0].args._referrer, buyerTwo);
      assert.equal(transaction.logs[0].args._category, 3);
      assert.equal(transaction.logs[0].args._quantity, 1);

      const boughtCount = await contract.boughtCount.call();
      assert.equal(boughtCount, 1);

      const balance = await contract.getBalance.call();
      assert.equal(balance > 0, true);
    });
  });
});
