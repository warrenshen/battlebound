const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];

  describe ("should nots", function() {
    before(async function() {
      contract = await CardTreasury.new();
    });

    it ("should not allow get on non-existing template", async function() {
      let response = null;

      try {
        await contract.getTemplate.call(0, { from: minter });
      } catch (error) {
        err = error
      }
      assert.equal(response, null, "response should not exist");

      try {
        await contract.getTemplate.call(1, { from: accounts[1] });
      } catch (error) {
        err = error
      }
      assert.equal(response, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    before(async function() {
      contract = await CardTreasury.new();
      await contract.mintTemplate(1, 0, 0, 8, "Lux", { from: minter });
      await contract.mintTemplate(2, 0, 0, 8, "Talusreaver", { from: minter });
      // Create a card for the sake of testing function when cards exist.
      await contract.mintCard(0, 0, minter, { from: minter });
    });

    it ("should return correct template information", async function() {
      let response;
      let generation;
      let power;
      let name;

      response = await contract.getTemplate.call(0, { from: minter });
      [generation, category, power, name] = response;
      assert.equal(generation, 0, "response generation is incorrect");
      assert.equal(category, 0, "response category is incorrect");
      assert.equal(power, 8, "response power is incorrect");
      assert.equal(name, "Lux", "response name is incorrect");

      response = await contract.getTemplate.call(1, { from: minter });
      [generation, category, power, name] = response;
      assert.equal(generation, 0, "response generation is incorrect");
      assert.equal(category, 0, "response category is incorrect");
      assert.equal(power, 8, "response power is incorrect");
      assert.equal(name, "Talusreaver", "response name is incorrect");
    });
  });
});
