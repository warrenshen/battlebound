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
        transaction = await contract.mintCards([0], [0], buyer, { from: buyer });
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
        transaction = await contract.mintCards([1], [0], buyer, { from: buyer });
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

      await contract.mintCards([0], buyer, { from: minter });
      await contract.mintCards([0], buyer, { from: minter });

      transaction = null;

      try {
        transaction = await contract.mintCards([0], buyer, { from: buyer });
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
      await contract.mintTemplate(8, 0, 0, 9, "Talusreaver", { from: minter });
      await contract.setMinter(minter, { from: minter });
    });

    it ("should allow privileged address to mint a card for self", async function() {
      const recipient = minter;

      const transaction = await contract.mintCards([0], recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      const limit = await contract.mintLimitByTemplate.call(0);
      assert.equal(limit.toNumber(), 3);

      const count = await contract.mintCountByTemplate.call(0);
      assert.equal(count.toNumber(), 1);

      const owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");
    });

    it ("should allow privileged address to mint a card for other", async function() {
      const recipient = buyer;

      const transaction = await contract.mintCards([0], recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);

      const supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

      const limit = await contract.mintLimitByTemplate.call(0);
      assert.equal(limit.toNumber(), 3);

      const count = await contract.mintCountByTemplate.call(0);
      assert.equal(count.toNumber(), 1);

      const owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");
    });

    it ("should allow privileged address to mint multiple cards", async function() {
      let recipient = minter;
      let owner;
      let transaction;
      let supply;

      transaction = await contract.mintCards([0, 0], recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);
      assert.equal(transaction.logs[1].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].args._from, 0);
      assert.equal(transaction.logs[1].args._to, recipient);
      assert.equal(transaction.logs[1].args._tokenId, 1);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 2, "supply of instances is not 1");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner, recipient, "owner is not correct");

      recipient = buyer;

      transaction = await contract.mintCards([0], recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 2);

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 3, "supply of instances is not 3");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner, recipient, "owner is not correct");

      const limit = await contract.mintLimitByTemplate.call(0);
      assert.equal(limit.toNumber(), 3);

      const count = await contract.mintCountByTemplate.call(0);
      assert.equal(count.toNumber(), 3);
    });

    it ("should allow privileged address to mint multiple cards with multiple templates", async function() {
      let recipient = minter;
      let owner;
      let transaction;
      let supply;
      let exists;
      let balance;

      transaction = await contract.mintCards([0, 0, 0], recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 0);
      assert.equal(transaction.logs[1].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].args._from, 0);
      assert.equal(transaction.logs[1].args._to, recipient);
      assert.equal(transaction.logs[1].args._tokenId, 1);
      assert.equal(transaction.logs[2].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[2].args._from, 0);
      assert.equal(transaction.logs[2].args._to, recipient);
      assert.equal(transaction.logs[2].args._tokenId, 2);

      exists = await contract.exists.call(0);
      assert.equal(exists, true, "exists is not correct");

      exists = await contract.exists.call(1);
      assert.equal(exists, true, "exists is not correct");

      exists = await contract.exists.call(2);
      assert.equal(exists, true, "exists is not correct");

      supply = await contract.totalSupply.call();
      assert.equal(supply.valueOf(), 3, "supply of instances is not 1");

      owner = await contract.ownerOf.call(0);
      assert.equal(owner, recipient, "owner is not correct");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner, recipient, "owner is not correct");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner, recipient, "owner is not correct");

      balance = await contract.balanceOf.call(recipient);
      assert.equal(balance, 3, "owner is not correct");

      recipient = buyer;

      transaction = await contract.mintCards([1, 1, 1, 1, 1, 1, 1, 1], recipient, { from: minter });
      assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[0].args._from, 0);
      assert.equal(transaction.logs[0].args._to, recipient);
      assert.equal(transaction.logs[0].args._tokenId, 3);
      assert.equal(transaction.logs[1].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[1].args._from, 0);
      assert.equal(transaction.logs[1].args._to, recipient);
      assert.equal(transaction.logs[1].args._tokenId, 4);
      assert.equal(transaction.logs[2].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[2].args._from, 0);
      assert.equal(transaction.logs[2].args._to, recipient);
      assert.equal(transaction.logs[2].args._tokenId, 5);
      assert.equal(transaction.logs[3].event, "Transfer", "expected a Transfer event");
      assert.equal(transaction.logs[3].args._from, 0);
      assert.equal(transaction.logs[3].args._to, recipient);
      assert.equal(transaction.logs[3].args._tokenId, 6);

      const gasUsed = transaction.receipt.gasUsed;

      exists = await contract.exists.call(3);
      assert.equal(exists, true, "exists is not correct");

      exists = await contract.exists.call(4);
      assert.equal(exists, true, "exists is not correct");

      exists = await contract.exists.call(5);
      assert.equal(exists, true, "exists is not correct");

      balance = await contract.balanceOf.call(recipient);
      assert.equal(balance, 8, "owner is not correct");
    });
  });
});
