const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
      await contract.setMinter(minter, { from: minter });
      await contract.mintCard(0, minter);
    });

    it ("should not allow transfer of non-existing card", async function() {
      const sender = minter;
      const recipient = buyer;
      const cardId = 1;

      let transaction = null;

      try {
        transaction = await contract.transferFrom(
          sender,
          recipient,
          cardId,
          { from: sender }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, minter, "owner is not correct");
      assert.notEqual(owner, recipient, "owner is not correct");
    });

    it ("should not allow non-owner to transfer card", async function() {
      const realOwner = minter;
      const sender = buyer;
      const recipient = buyerTwo;
      const cardId = 0;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, realOwner, "owner is not correct");
      assert.notEqual(owner, sender, "owner is not correct");

      let transaction = null;

      try {
        transaction = await contract.transferFrom(
          sender,
          recipient,
          cardId,
          { from: sender }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, realOwner, "owner is not correct");
      assert.notEqual(owner, recipient, "owner is not correct");
    });

    it ("should not allow owner to transfer card to 0", async function() {
      const sender = minter;
      const recipient = 0;
      const cardId = 0;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      let transaction = null;

      try {
        transaction = await contract.transferFrom(
          sender,
          recipient,
          cardId,
          { from: sender }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");
      assert.notEqual(owner, recipient, "owner is not correct");
    });

    it ("should not allow owner to transfer to cards contract", async function() {
      const sender = minter;
      const recipient = contract.address;
      const cardId = 0;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      let transaction = null;

      try {
        transaction = await contract.transferFrom(
          sender,
          recipient,
          cardId,
          { from: sender }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");
    });
  });

  describe ("a -> b, a -> c", function() {
    // BEFORE NOT BEFORE EACH
    before(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
      await contract.mintTemplate(1, 0, 0, 9, "T2", { from: minter });
      await contract.setMinter(minter, { from: minter });
      await contract.mintCard(0, minter);
      await contract.mintCard(1, minter);
    });

    it ("should allow owner to transfer card to other", async function() {
      const sender = minter;
      const recipient = buyer;
      const cardId = 0;
      let owner;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      const transaction = await contract.transferFrom(
        sender,
        recipient,
        cardId,
        { from: sender }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "Transfer", "expected a Transfer event");
      assert.equal(log.args._from, sender, "Transfer event from is incorrect");
      assert.equal(log.args._to, recipient, "Transfer event to is incorrect");
      assert.equal(log.args._tokenId, cardId, "Transfer event token id is incorrect");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");

      const balance = await contract.balanceOf.call(sender);
      assert.equal(balance, 1, "balance is not correct");
    });

    it ("should allow owner to transfer card to another other", async function() {
      const sender = minter;
      const recipient = buyerTwo;
      const cardId = 1;
      let owner;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, sender, "owner is not correct");

      const transaction = await contract.transferFrom(
        sender,
        recipient,
        cardId,
        { from: sender }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "Transfer", "expected a Transfer event");
      assert.equal(log.args._from, sender, "Transfer event from is incorrect");
      assert.equal(log.args._to, recipient, "Transfer event to is incorrect");
      assert.equal(log.args._tokenId, cardId, "Transfer event token id is incorrect");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");

      const balance = await contract.balanceOf.call(sender);
      assert.equal(balance, 0, "balance is not correct");
    });
  });

  describe ("a -> b -> a", function() {
    before(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
      await contract.setMinter(minter, { from: minter });
      await contract.mintCard(0, minter);
    });

    it ("should allow two addresses to transfer back and forth", async function() {
      const cardId = 0;
      let sender = minter;
      let recipient = buyer;
      let owner;
      let transaction;
      let log;

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, minter, "owner is not correct");

      transaction = await contract.transferFrom(
        sender,
        recipient,
        cardId,
        { from: sender }
      );
      log = transaction.logs[0];
      assert.equal(log.event, "Transfer", "expected a Transfer event");
      assert.equal(log.args._from, sender, "Transfer event from is incorrect");
      assert.equal(log.args._to, recipient, "Transfer event to is incorrect");
      assert.equal(log.args._tokenId, cardId, "Transfer event token id is incorrect");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");

      sender = buyer;
      recipient = minter;

      transaction = await contract.transferFrom(
        sender,
        recipient,
        cardId,
        { from: sender }
      );
      log = transaction.logs[0];
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(log.event, "Transfer", "expected a Transfer event");
      assert.equal(log.args._from, sender, "Transfer event from is incorrect");
      assert.equal(log.args._to, recipient, "Transfer event to is incorrect");
      assert.equal(log.args._tokenId, cardId, "Transfer event token id is incorrect");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");

      sender = minter;
      recipient = buyer;

      transaction = await contract.transferFrom(
        sender,
        recipient,
        cardId,
        { from: sender }
      );
      log = transaction.logs[0];
      assert.equal(log.event, "Transfer", "expected a Transfer event");
      assert.equal(log.args._from, sender, "Transfer event from is incorrect");
      assert.equal(log.args._to, recipient, "Transfer event to is incorrect");
      assert.equal(log.args._tokenId, cardId, "Transfer event token id is incorrect");

      owner = await contract.ownerOf.call(cardId);
      assert.equal(owner, recipient, "owner is not correct");
    });
  });
});
