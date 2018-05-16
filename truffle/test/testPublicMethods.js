const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  describe ("new contract", function() {
    before(async function() {
      contract = await CardTreasury.new();
   });

    it ("should start with no templates", async function() {
      const supply = await contract.templatesSupply.call();
      assert.equal(supply.toNumber(), 0, "supply of templates is not 0");
    });

    it ("should start with no instances", async function() {
      const supply = await contract.instancesSupply.call();
      assert.equal(supply.toNumber(), 0, "supply of instances is not 0");
    });
  });

  describe ("contract with multiple instance owners", function() {
    before(async function() {
      contract = await CardTreasury.new();
      await contract.createTemplate(2, 0, 9, "T1", { from: accounts[0] });
      await contract.createTemplate(1, 0, 9, "T2", { from: accounts[0] });
      await contract.mintCard(0, accounts[2], { from: accounts[0] });
      await contract.mintCard(1, accounts[1], { from: accounts[0] });
      await contract.mintCard(0, accounts[1], { from: accounts[0] });
    });

    it ("should return correct supply of templates", async function() {
      const supply = await contract.templatesSupply.call();
      assert.equal(supply.toNumber(), 2, "supply of templates is not 2");
    });

    it ("should return correct supply of instances", async function() {
      const supply = await contract.instancesSupply.call();
      assert.equal(supply.toNumber(), 3, "supply of instances is not 3");
    });

    it ("should return owner of instances correctly", async function() {
      let owner;

      owner = await contract.ownerOf.call(0);
      assert.equal(owner.valueOf(), accounts[2], "owner is incorrect");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner.valueOf(), accounts[1], "owner is incorrect");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner.valueOf(), accounts[1], "owner is incorrect");
    });

    it ("should not return owner for non-existing card", async function() {
      let owner = null;

      try {
        owner = await contract.ownerOf.call(3);
      } catch (error) {
        err = error
      }
      assert.equal(owner, null, "owner should not exist");
    });

    it ("should return balance of owner correctly", async function() {
      let balance;

      balance = await contract.balanceOf.call(accounts[0]);
      assert.equal(balance.toNumber(), 0, "balance is incorrect");

      balance = await contract.balanceOf.call(accounts[1]);
      assert.equal(balance.toNumber(), 2, "balance is incorrect");

      balance = await contract.balanceOf.call(accounts[2]);
      assert.equal(balance.toNumber(), 1, "balance is incorrect");
    });

    it ("should return tokens of owner correctly", async function() {
      let tokens;

      tokens = await contract.tokensOfOwner.call(accounts[0]);
      assert.equal(tokens.length, 0, "number of tokens is incorrect");

      tokens = await contract.tokensOfOwner.call(accounts[1]);
      assert.equal(tokens.length, 2, "number of tokens is incorrect");
      assert.equal(tokens[0], 1, "token is incorrect");
      assert.equal(tokens[1], 2, "token is incorrect");

      tokens = await contract.tokensOfOwner.call(accounts[2]);
      assert.equal(tokens.length, 1, "number of tokens is incorrect");
      assert.equal(tokens[0], 0, "token is incorrect");
    });
  });
});
