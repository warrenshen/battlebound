const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  describe ("new contract", function() {
    before(async function() {
      contract = await CardTreasury.new();
   });

    it ("should start with no templates", async function() {
      const supply = await contract.templateSupply.call();
      assert.equal(supply.toNumber(), 0, "supply of templates is not 0");
    });

    it ("should start with no instances", async function() {
      const supply = await contract.totalSupply.call();
      assert.equal(supply.toNumber(), 0, "supply of instances is not 0");
    });

    it ("should support erc interfaces", async function() {
      let isSupported;

      isSupported = await contract.supportsInterface.call("0x01ffc9a7");
      assert.equal(isSupported, true);

      isSupported = await contract.supportsInterface.call("0x80ac58cd");
      assert.equal(isSupported, true);

      isSupported = await contract.supportsInterface.call("0x4f558e79");
      assert.equal(isSupported, true);

      isSupported = await contract.supportsInterface.call("0x80ac58ce");
      assert.equal(isSupported, false);
    });
  });

  describe ("contract with multiple instance owners", function() {
    before(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(2, 0, 0, 9, "Lux", { from: minter });
      await contract.mintTemplate(3, 0, 0, 9, "Talusreaver", { from: minter });
      await contract.setMinter(minter, { from: minter });
      await contract.mintCard(0, 0, buyerTwo, { from: minter });
      await contract.mintCard(1, 1, buyer, { from: minter });
      await contract.mintCard(0, 1, buyer, { from: minter });
    });

    it ("should return correct supply of templates", async function() {
      const supply = await contract.templateSupply.call();
      assert.equal(supply.toNumber(), 2, "supply of templates is not 2");
    });

    it ("should return correct supply of instances", async function() {
      const supply = await contract.totalSupply.call();
      assert.equal(supply.toNumber(), 3, "supply of instances is not 3");
    });

    it ("should return owner of instances correctly", async function() {
      let owner;

      owner = await contract.ownerOf.call(0);
      assert.equal(owner.valueOf(), buyerTwo, "owner is incorrect");

      owner = await contract.ownerOf.call(1);
      assert.equal(owner.valueOf(), buyer, "owner is incorrect");

      owner = await contract.ownerOf.call(2);
      assert.equal(owner.valueOf(), buyer, "owner is incorrect");
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

      balance = await contract.balanceOf.call(minter);
      assert.equal(balance.toNumber(), 0, "balance is incorrect");

      balance = await contract.balanceOf.call(buyer);
      assert.equal(balance.toNumber(), 2, "balance is incorrect");

      balance = await contract.balanceOf.call(buyerTwo);
      assert.equal(balance.toNumber(), 1, "balance is incorrect");
    });

    it ("should return tokens of owner correctly", async function() {
      let tokens;

      tokens = await contract.tokensOfOwner.call(minter);
      assert.equal(tokens.length, 0, "number of tokens is incorrect");

      tokens = await contract.tokensOfOwner.call(buyer);
      assert.equal(tokens.length, 2, "number of tokens is incorrect");
      assert.equal(tokens[0], 1, "token is incorrect");
      assert.equal(tokens[1], 2, "token is incorrect");

      tokens = await contract.tokensOfOwner.call(buyerTwo);
      assert.equal(tokens.length, 1, "number of tokens is incorrect");
      assert.equal(tokens[0], 0, "token is incorrect");
    });

    it ("should support erc interfaces", async function() {
      let isSupported;

      isSupported = await contract.supportsInterface.call("0x01ffc9a7");
      assert.equal(isSupported, true);

      isSupported = await contract.supportsInterface.call("0x80ac58cd");
      assert.equal(isSupported, true);

      isSupported = await contract.supportsInterface.call("0x80ac58ce");
      assert.equal(isSupported, false);
    });
  });
});
