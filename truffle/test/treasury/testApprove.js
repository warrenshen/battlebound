const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(1, 0, 0, 8, "T1", { from: minter });
      await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
      await contract.setMinter(minter, { from: minter });
      await contract.mintCard(0, buyer, { from: minter });
    });

    it ("should not allow non-owner of card to approve account address", async function() {
      let transaction;

      try {
        transaction = await contract.approve(
          buyerTwo,
          0,
          { from: buyerTwo }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");

      const approved = await contract.getApproved(0);
      assert.equal(approved, 0);
    });

    it ("should not allow non-owner of card to approve contract address", async function() {
      let transaction;

      try {
        transaction = await contract.approve(
          contract.address,
          0,
          { from: buyerTwo }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");

      const approved = await contract.getApproved(0);
      assert.equal(approved, 0);
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
      await contract.setMinter(minter, { from: minter });
      await contract.mintCard(0, buyer, { from: minter });
    });

    it ("should allow owner of card to approve account address", async function() {
      const transaction = await contract.approve(
        buyerTwo,
        0,
        { from: buyer }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "Approval", "expected an Approval event");
      assert.equal(log.args._owner, buyer, "Approval event owner is incorrect");
      assert.equal(log.args._approved, buyerTwo, "Approval event approved is incorrect");
      assert.equal(log.args._tokenId, 0, "Approval event token id is incorrect");

      const approved = await contract.getApproved(0);
      assert.equal(approved, buyerTwo);
    });

    it ("should allow owner of card to approve contract address", async function() {
      const transaction = await contract.approve(
        contract.address,
        0,
        { from: buyer }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "Approval", "expected an Approval event");
      assert.equal(log.args._owner, buyer, "Approval event owner is incorrect");
      assert.equal(log.args._approved, contract.address, "Approval event approved is incorrect");
      assert.equal(log.args._tokenId, 0, "Approval event token id is incorrect");

      const approved = await contract.getApproved(0);
      assert.equal(approved, contract.address);
    });
  });
});
