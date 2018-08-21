const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(2, 0, 0, 9, "Lux", { from: minter });
    });

    it ("should not allow unprivileged address to mint a card", async function() {
      let transaction = null;

      try {
        transaction = await contract.mintCard(0, 0, buyer, { from: buyer });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 0, "supply of instances is not 0");
    });

    it ("should not allow a card with invalid template to be minted", async function() {
      let transaction;
      let supply;

      transaction = null;

      try {
        transaction = await contract.mintCard(1, 0, minter, { from: minter });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 0, "supply of templates is not 0");
    });

    it ("should not allow more cards than mint limit to be minted", async function() {
      let transaction;
      let supply;

      await contract.mintCard(0, 0, minter, { from: minter });
      await contract.mintCard(0, 0, minter, { from: minter });

      transaction = null;

      try {
        transaction = await contract.mintCard(0, minter, { from: minter });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of templates is not 2");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(3, 0, 0, 9, "Lux", { from: minter });
      await contract.mintTemplate(4, 0, 0, 9, "Talusreaver", { from: minter });
    });

    it ("should allow privileged address to mint a card for self", async function() {
      const recipient = minter;

      const transaction = await contract.mintCard(0, 0, recipient);
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");
      assert.equal(transaction.logs[1].args.owner, recipient);
      assert.equal(transaction.logs[1].args.cardId, 0);

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      const owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");
    });

    it ("should allow privileged address to mint a card for other", async function() {
      const recipient = accounts[1];

      const transaction = await contract.mintCard(0, 0, recipient);
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");
      assert.equal(transaction.logs[1].args.owner, recipient);
      assert.equal(transaction.logs[1].args.cardId, 0);

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      const owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");
    });

    it ("should allow privileged address to mint multiple cards", async function() {
      let recipient = minter;
      let owner;
      let transaction;
      let supply;

      transaction = await contract.mintCard(0, 0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");
      assert.equal(transaction.logs[1].args.owner, recipient);
      assert.equal(transaction.logs[1].args.cardId, 0);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      transaction = await contract.mintCard(0, 0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");
      assert.equal(transaction.logs[1].args.owner, recipient);
      assert.equal(transaction.logs[1].args.cardId, 1);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of instances is not 2");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner, recipient, "owner is not correct");

      recipient = buyer;

      transaction = await contract.mintCard(0, 0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");
      assert.equal(transaction.logs[1].args.owner, recipient);
      assert.equal(transaction.logs[1].args.cardId, 2);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 3, "supply of instances is not 3");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner, recipient, "owner is not correct");
    });

    it ("should allow privileged address to mint multiple cards with multiple templates", async function() {
      let owner;
      let transaction;
      let supply;

      transaction = await contract.mintCard(0, 0, minter, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      transaction = await contract.mintCard(1, 0, buyer, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of instances is not 2");

      transaction = await contract.mintCard(0, 0, minter, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 3, "supply of instances is not 3");

      transaction = await contract.mintCard(1, 0, minter, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].event, "CardMinted", "expected an CardMinted event");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 4, "supply of instances is not 4");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, minter, "owner is not correct");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner, buyer, "owner is not correct");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner, minter, "owner is not correct");

      owner = await contract.ownerOf.call(3);
      assert.equal(owner, minter, "owner is not correct");
    });
  });
});
