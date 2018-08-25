const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const god = accounts[0];
  const minter = accounts[1];
  const buyer = accounts[2];

  describe ("should nots", function() {
    describe ("set minter", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.mintTemplate(2, 0, 0, 9, "Lux", { from: god });
      });

      it ("should not allow unprivileged address to set minter", async function() {
        let transaction = null;

        try {
          transaction = await contract.setMinter(minter, { from: minter });
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "transaction should not exist");

        const minterAddress = await contract.minter.call();
        assert.equal(minterAddress, 0);
      });

      it ("should not allow mint card without minter set", async function() {
        let transaction = null;

        try {
          transaction = await contract.mintCard(0, 0, god, { from: god });
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "transaction should not exist");

        const minterAddress = await contract.minter.call();
        assert.equal(minterAddress, 0);
      });
    });

    describe ("mint cards", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.setMinter(minter, { from: god });
        await contract.mintTemplate(3, 0, 0, 9, "Lux", { from: god });
        await contract.mintTemplate(4, 0, 0, 9, "Talusreaver", { from: god });
      });

      it ("should not allow non-minter to mint card", async function() {
        let transaction = null;

        try {
          transaction = await contract.mintCard(0, 0, god, { from: god });
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "transaction should not exist");
      });

      it ("should allow minter to mint cards", async function() {
        let transaction = null;

        try {
          transaction = await contract.mintCards([0], god, { from: god });
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "transaction should not exist");
      });
    });
  });

  describe ("shoulds", function() {
    describe ("set minter", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.mintTemplate(3, 0, 0, 9, "Lux", { from: god });
        await contract.mintTemplate(4, 0, 0, 9, "Talusreaver", { from: god });
      });

      it ("should allow privileged address to set minter to self", async function() {
        const transaction = await contract.setMinter(god, { from: god });

        const minterAddress = await contract.minter.call();
        assert.equal(minterAddress, god);
      });

      it ("should allow privileged address to set minter to other", async function() {
        const transaction = await contract.setMinter(minter, { from: god });

        const minterAddress = await contract.minter.call();
        assert.equal(minterAddress, minter);
      });

      it ("should allow privileged address to update minter", async function() {
        let transaction;
        let minterAddress;

        transaction = await contract.setMinter(minter, { from: god });

        minterAddress = await contract.minter.call();
        assert.equal(minterAddress, minter);

        transaction = await contract.setMinter(god, { from: god });

        minterAddress = await contract.minter.call();
        assert.equal(minterAddress, god);
      });
    });

    describe ("mint cards", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.setMinter(minter, { from: god });
        await contract.mintTemplate(3, 0, 0, 9, "Lux", { from: god });
        await contract.mintTemplate(4, 0, 0, 9, "Talusreaver", { from: god });
      });

      it ("should allow minter to mint card", async function() {
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
      });

      it ("should allow minter to mint cards", async function() {
        const recipient = minter;

        transaction = await contract.mintCards([0, 0], recipient, { from: minter });
        assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
        assert.equal(transaction.logs[0].args._from, 0);
        assert.equal(transaction.logs[0].args._to, recipient);
        assert.equal(transaction.logs[0].args._tokenId, 0);
        assert.equal(transaction.logs[1].event, "Transfer", "expected a Transfer event");
        assert.equal(transaction.logs[1].args._from, 0);
        assert.equal(transaction.logs[1].args._to, recipient);
        assert.equal(transaction.logs[1].args._tokenId, 1);
      });
    });
  });
});
