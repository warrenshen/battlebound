const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  describe ("should nots", function() {
    describe ("set approval for all", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
        await contract.setMinter(minter, { from: minter });
        await contract.mintCard(0, 0, buyer, { from: minter });
      });

      it ("should not allow operator to be self", async function() {
        let transaction = null;

        try {
          transaction = await contract.setApprovalForAll(buyer, { from: buyer });
        } catch (error) {
          err = error;
        }
        assert.equal(transaction, null, "transaction should not exist");
      });
    });

    describe ("use set approval for all", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
        await contract.setMinter(minter, { from: minter });
        await contract.mintCard(0, 0, buyer, { from: minter });
        await contract.setApprovalForAll(
          buyerTwo,
          true,
          { from: buyer }
        );
      });

      it ("should not allow transfer from by operator with wrong from", async function() {
        let transaction = null;

        try {
          transaction = await contract.transferFrom(
            buyerTwo,
            buyerTwo,
            0,
            { from: buyerTwo }
          );
        } catch (error) {
          err = error;
        }
        assert.equal(transaction, null, "transaction should not exist");
      });
    });
  });

  describe ("shoulds", function() {
    describe ("set approval for all", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
        await contract.setMinter(minter, { from: minter });
        await contract.mintCard(0, 0, buyer, { from: minter });
      });

      it ("should allow set approval by all to true", async function() {
        const transaction = await contract.setApprovalForAll(
          buyerTwo,
          true,
          { from: buyer }
        );

        const log = transaction.logs[0];
        assert.equal(log.event, "ApprovalForAll", "expected an ApprovalForAll event");
        assert.equal(log.args._owner, buyer, "Approval event owner is incorrect");
        assert.equal(log.args._operator, buyerTwo, "Approval event operator is incorrect");
        assert.equal(log.args._approved, true, "Approval event approved is incorrect");

        const approved = await contract.getApproved(0);
        assert.equal(approved, 0);

        const isApprovedForAll = await contract.isApprovedForAll(buyer, buyerTwo);
        assert.equal(isApprovedForAll, true);
      });

      it ("should allow set approval by all to false", async function() {
        const transaction = await contract.setApprovalForAll(
          buyerTwo,
          false,
          { from: buyer }
        );

        const log = transaction.logs[0];
        assert.equal(log.event, "ApprovalForAll", "expected an ApprovalForAll event");
        assert.equal(log.args._owner, buyer, "ApprovalForAll event owner is incorrect");
        assert.equal(log.args._operator, buyerTwo, "ApprovalForAll event operator is incorrect");
        assert.equal(log.args._approved, false, "ApprovalForAll event approved is incorrect");

        const approved = await contract.getApproved(0);
        assert.equal(approved, 0);

        const isApprovedForAll = await contract.isApprovedForAll(buyer, buyerTwo);
        assert.equal(isApprovedForAll, false);
      });
    });

    describe ("use set approval for all", function() {
      beforeEach(async function() {
        contract = await CardTreasury.new();
        await contract.mintTemplate(1, 0, 0, 9, "T1", { from: minter });
        await contract.setMinter(minter, { from: minter });
        await contract.mintCard(0, 0, buyer, { from: minter });
        await contract.setApprovalForAll(
          buyerTwo,
          true,
          { from: buyer }
        );
      });

      it ("should allow approval by operator", async function() {
        const transaction = await contract.approve(
          buyerTwo,
          0,
          { from: buyerTwo }
        );

        const log = transaction.logs[0];
        assert.equal(log.event, "Approval", "expected an Approval event");
        assert.equal(log.args._owner, buyer, "Approval event owner is incorrect");
        assert.equal(log.args._approved, buyerTwo, "Approval event operator is incorrect");
        assert.equal(log.args._tokenId, 0, "Approval event approved is incorrect");

        const approved = await contract.getApproved(0);
        assert.equal(approved, buyerTwo);

        const isApprovedForAll = await contract.isApprovedForAll(buyer, buyerTwo);
        assert.equal(isApprovedForAll, true);
      });

      it ("should allow transfer from by operator", async function() {
        const transaction = await contract.transferFrom(
          buyer,
          buyerTwo,
          0,
          { from: buyerTwo }
        );

        const log = transaction.logs[0];
        assert.equal(log.event, "Transfer", "expected an Transfer event");
        assert.equal(log.args._from, buyer, "Transfer event owner is incorrect");
        assert.equal(log.args._to, buyerTwo, "Transfer event operator is incorrect");
        assert.equal(log.args._tokenId, 0, "Transfer event approved is incorrect");

        const approved = await contract.getApproved(0);
        assert.equal(approved, 0);

        const isApprovedForAll = await contract.isApprovedForAll(buyer, buyerTwo);
        assert.equal(isApprovedForAll, true);
      });
    });
  });
});
