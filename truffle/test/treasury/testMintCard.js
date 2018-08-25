const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(2, 0, 0, 9, "Lux", { from: minter });
      await contract.setMinter(minter, { from: minter });
    });

    it ("should not allow unprivileged address to mint a card", async function() {
      let transaction = null;

      try {
        transaction = await contract.mintCard(0, buyer, { from: buyer });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 0, "supply of instances is not 0");

      const exists = await contract.exists.call(0);
      assert.equal(exists, false, "exists is not correct");
    });

    it ("should not allow a card with invalid template to be minted", async function() {
      let transaction;
      let supply;

      transaction = null;

      try {
        transaction = await contract.mintCard(1, minter, { from: minter });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 0, "supply of templates is not 0");

      const exists = await contract.exists.call(0);
      assert.equal(exists, false, "exists is not correct");
    });

    it ("should not allow more cards than mint limit to be minted", async function() {
      let transaction;
      let supply;
      let exists;

      await contract.mintCard(0, minter, { from: minter });
      await contract.mintCard(0, minter, { from: minter });

      transaction = null;

      try {
        transaction = await contract.mintCard(0, minter, { from: minter });
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "transaction should not exist");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of templates is not 2");

      exists = await contract.exists.call(0);
      assert.equal(exists, true, "exists is not correct");

      exists = await contract.exists.call(1);
      assert.equal(exists, true, "exists is not correct");

      exists = await contract.exists.call(2);
      assert.equal(exists, false, "exists is not correct");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(3, 0, 0, 9, "Lux", { from: minter });
      await contract.mintTemplate(4, 0, 0, 9, "Talusreaver", { from: minter });
      await contract.mintTemplate(4096, 0, 0, 9, "Guppea", { from: minter });
      await contract.setMinter(minter, { from: minter });
    });

    it ("should allow privileged address to mint a card for self", async function() {
      const recipient = minter;

      const transaction = await contract.mintCard(0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      const owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");

      const exists = await contract.exists.call(0);
      assert.equal(exists, true, "exists is not correct");
    });

    it ("should allow privileged address to mint a card for other", async function() {
      const recipient = accounts[1];

      const transaction = await contract.mintCard(0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      const owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");

      const exists = await contract.exists.call(0);
      assert.equal(exists, true, "exists is not correct");
    });

    it ("should allow privileged address to mint multiple cards", async function() {
      let recipient = minter;
      let owner;
      let transaction;
      let supply;
      let exists;

      transaction = await contract.mintCard(0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      exists = await contract.exists.call(0);
      assert.equal(exists, true, "exists is not correct");

      transaction = await contract.mintCard(0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 1);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of instances is not 2");

      exists = await contract.exists.call(1);
      assert.equal(exists, true, "exists is not correct");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner, recipient, "owner is not correct");

      recipient = buyer;

      transaction = await contract.mintCard(0, recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 2);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 3, "supply of instances is not 3");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner, recipient, "owner is not correct");

      exists = await contract.exists.call(2);
      assert.equal(exists, true, "exists is not correct");
    });

    it ("should allow privileged address to mint multiple cards with multiple templates", async function() {
      let owner;
      let transaction;
      let supply;
      let exists;

      transaction = await contract.mintCard(0, minter, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, minter);
      assert.equal(transaction.logs[0].args._tokenId, 0);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      exists = await contract.exists.call(0);
      assert.equal(exists, true, "exists is not correct");

      transaction = await contract.mintCard(1, buyer, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, buyer);
      assert.equal(transaction.logs[0].args._tokenId, 1);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of instances is not 2");

      exists = await contract.exists.call(1);
      assert.equal(exists, true, "exists is not correct");

      transaction = await contract.mintCard(0, minter, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, minter);
      assert.equal(transaction.logs[0].args._tokenId, 2);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 3, "supply of instances is not 3");

      exists = await contract.exists.call(2);
      assert.equal(exists, true, "exists is not correct");

      transaction = await contract.mintCard(1, minter, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, minter);
      assert.equal(transaction.logs[0].args._tokenId, 3);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 4, "supply of instances is not 4");

      exists = await contract.exists.call(3);
      assert.equal(exists, true, "exists is not correct");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, minter, "owner is not correct");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner, buyer, "owner is not correct");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner, minter, "owner is not correct");

      owner = await contract.ownerOf.call(3);
      assert.equal(owner, minter, "owner is not correct");
    });

    it ("should allow mint many cards", async function() {
      let transaction;

      for (var i = 0; i < 256; i += 1) {
        transaction = await contract.mintCard(2, minter, { from: minter });
        assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
        assert.equal(transaction.logs[0].args._from, 0);
        assert.equal(transaction.logs[0].args._to, minter);
        assert.equal(transaction.logs[0].args._tokenId, i);
      }

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 256);
    });
  });
});
