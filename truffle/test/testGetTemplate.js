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
      contract.createTemplate(1, "T1", 1, { from: accounts[0] });
      contract.createTemplate(2, "T2", 2, { from: accounts[0] });
    });

    it ("should return correct template information", async function() {
      let response;
      let category;
      let name;

      response = await contract.getTemplate.call(0, { from: accounts[0] });
      [category, name] = response;
      assert.equal(category, 1, "response category is incorrect");
      assert.equal(name, "T1", "response name is incorrect");

      response = await contract.getTemplate.call(1, { from: accounts[0] });
      [category, name] = response;
      assert.equal(category, 2, "response category is incorrect");
      assert.equal(name, "T2", "response name is incorrect");
    });
  });
});
