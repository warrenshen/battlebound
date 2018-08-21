const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];

  describe ("should nots", function() {
    before(async function() {
      contract = await CardTreasury.new();
    });

    it ("should not allow get on non-existing card", async function() {
      let response = null;

      try {
        await contract.getCard.call(0, { from: minter });
      } catch (error) {
        err = error
      }
      assert.equal(response, null, "response should not exist");

      try {
        await contract.getCard.call(1, { from: accounts[1] });
      } catch (error) {
        err = error
      }
      assert.equal(response, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    before(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(2, 0, 0, 8, "Lux", { from: minter });
      await contract.mintTemplate(1, 1, 1, 9, "Talusreaver", { from: minter });
      await contract.mintCards([0, 1], [0, 1], minter, { from: minter });
      await contract.mintCard(0, 1, minter, { from: minter });
    });

    it ("should return correct card information", async function() {
      let response;
      let generation;
      let power;
      let name;

      response = await contract.getCard.call(0, { from: minter });
      [generation, category, power, name, variation] = response;
      assert.equal(generation, 0, "response generation is incorrect");
      assert.equal(category, 0, "response category is incorrect");
      assert.equal(power, 8, "response power is incorrect");
      assert.equal(name, "Lux", "response name is incorrect");
      assert.equal(variation, 0, "response variation is incorrect");

      response = await contract.getCard.call(1, { from: minter });
      [generation, category, power, name, variation] = response;
      assert.equal(generation, 1, "response generation is incorrect");
      assert.equal(category, 1, "response category is incorrect");
      assert.equal(power, 9, "response power is incorrect");
      assert.equal(name, "Talusreaver", "response name is incorrect");
      assert.equal(variation, 1, "response variation is incorrect");

      response = await contract.getCard.call(2, { from: minter });
      [generation, category, power, name, variation] = response;
      assert.equal(generation, 0, "response generation is incorrect");
      assert.equal(category, 0, "response category is incorrect");
      assert.equal(power, 8, "response power is incorrect");
      assert.equal(name, "Lux", "response name is incorrect");
      assert.equal(variation, 1, "response variation is incorrect");
    });
  });
});
