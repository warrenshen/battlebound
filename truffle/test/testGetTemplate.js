const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  describe ("should nots", function() {
    before(async function() {
      contract = await CardTreasury.new();
    });

    it ("should not allow get on non-existing template", async function() {
      let response = null;

      try {
        await contract.getTemplate.call(0, { from: accounts[0] });
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
      await contract.createTemplate(1, 0, 8, "T1", { from: accounts[0] });
      await contract.createTemplate(2, 1, 9, "T2", { from: accounts[0] });
      // Create a card for the sake of testing function when cards exist.
      await contract.mintCard(0, accounts[0], { from: accounts[0] });
    });

    it ("should return correct template information", async function() {
      let response;
      let generation;
      let power;
      let name;

      response = await contract.getTemplate.call(0, { from: accounts[0] });
      [generation, power, name] = response;
      assert.equal(generation, 0, "response generation is incorrect");
      assert.equal(power, 8, "response generation is incorrect");
      assert.equal(name, "T1", "response name is incorrect");

      response = await contract.getTemplate.call(1, { from: accounts[0] });
      [category, name] = response;
      [generation, power, name] = response;
      assert.equal(generation, 1, "response generation is incorrect");
      assert.equal(power, 9, "response generation is incorrect");
      assert.equal(name, "T2", "response name is incorrect");
    });
  });
});
