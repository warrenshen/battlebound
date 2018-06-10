const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.createTemplate(1, 0, 9, "T1", { from: accounts[0] });
      await contract.mintCard(0, accounts[1], { from: accounts[0] });
    });

    it ("should not allow non-owner of card to approve account address", async function() {
      let transaction;

      try {
        transaction = await contract.approve(
          accounts[2],
          0,
          { from: accounts[2] }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
    });

    it ("should not allow non-owner of card to approve contract address", async function() {
      let transaction;

      try {
        transaction = await contract.approve(
          contract.address,
          0,
          { from: accounts[2] }
        );
      } catch (error) {
        err = error;
      }
      assert.equal(transaction, null, "transaction should not exist");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      contract = await CardTreasury.new();
      await contract.createTemplate(1, 0, 9, "T1", { from: accounts[0] });
      await contract.mintCard(0, accounts[1], { from: accounts[0] });
    });

    it ("should allow owner of card to approve account address", async function() {
      const transaction = await contract.approve(
        accounts[2],
        0,
        { from: accounts[1] }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "Approval", "expected an Approval event");
      assert.equal(log.args.owner, accounts[1], "Approval event owner is incorrect");
      assert.equal(log.args.approved, accounts[2], "Approval event approved is incorrect");
      assert.equal(log.args.tokenId, 0, "Approval event token id is incorrect");
    });

    it ("should allow owner of card to approve contract address", async function() {
      const transaction = await contract.approve(
        contract.address,
        0,
        { from: accounts[1] }
      );
      const log = transaction.logs[0];
      assert.equal(log.event, "Approval", "expected an Approval event");
      assert.equal(log.args.owner, accounts[1], "Approval event owner is incorrect");
      assert.equal(log.args.approved, contract.address, "Approval event approved is incorrect");
      assert.equal(log.args.tokenId, 0, "Approval event token id is incorrect");
    });
  });
});
