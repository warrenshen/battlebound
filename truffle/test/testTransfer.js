const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  describe ("validations", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      contract.createTemplate("T1", 1, { from: accounts[0] });
      contract.mintCard(0, accounts[0]);
    });

    it ("should not allow transfer of non-existing card", async function() {
      const sender = accounts[0];
      const recipient = accounts[1];
      const cardId = 1;

      let transaction = null;

      try {
        transaction = await contract.transfer(recipient, cardId, { from: sender });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");
    });

    it ("should not allow non-owner to transfer card", async function() {
      const realOwner = accounts[0];
      const sender = accounts[1];
      const recipient = accounts[2];
      const cardId = 0;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, realOwner, "owner is not correct");
      assert.notEqual(owner, sender, "owner is not correct");

      let transaction = null;

      try {
        transaction = await contract.transfer(recipient, cardId, { from: sender });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, realOwner, "owner is not correct");
      assert.notEqual(owner, recipient, "owner is not correct");
    });

    it ("should not allow owner to transfer card to 0", async function() {
      const sender = accounts[0];
      const recipient = 0;
      const cardId = 0;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      let transaction = null;

      try {
        transaction = await contract.transfer(recipient, cardId, { from: sender });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");
      assert.notEqual(owner, recipient, "owner is not correct");
    });

    it ("should not allow owner to transfer to cards contract", async function() {
      const sender = accounts[0];
      const recipient = contract.address;
      const cardId = 0;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      let transaction = null;

      try {
        transaction = await contract.transfer(recipient, cardId, { from: sender });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");
    });
  });

  describe ("a -> b, a -> c", function() {
    before(async function() {
      contract = await CardTreasury.new();
      contract.createTemplate("T1", 1, { from: accounts[0] });
      contract.createTemplate("T2", 1, { from: accounts[0] });
      contract.mintCard(0, accounts[0]);
      contract.mintCard(1, accounts[0]);
    });

    it ("should allow owner to transfer card to other", async function() {
      const sender = accounts[0];
      const recipient = accounts[1];
      const cardId = 0;
      let owner;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      const transaction = await contract.transfer(recipient, cardId, { from: sender });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");
    });

    it ("should allow owner to transfer card to another other", async function() {
      const sender = accounts[0];
      const recipient = accounts[2];
      const cardId = 1;
      let owner;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      const transaction = await contract.transfer(recipient, cardId, { from: sender });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");
    });
  });

  describe ("a -> b -> a", function() {
    before(async function() {
      contract = await CardTreasury.new();
      contract.createTemplate("T1", 1, { from: accounts[0] });
      contract.mintCard(0, accounts[0]);
    });

    it ("should allow two addresses to transfer back and forth", async function() {
      const cardId = 0;
      let sender = accounts[0];
      let recipient = accounts[1];
      let owner;
      let transaction;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, accounts[0], "owner is not correct");

      transaction = await contract.transfer(recipient, cardId, { from: sender });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");

      sender = accounts[1];
      recipient = accounts[0];

      transaction = await contract.transfer(recipient, cardId, { from: sender });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");
    });
  });
});