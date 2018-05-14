var CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  beforeEach(async function() {
     contract = await CardTreasury.new();
  });
  
  it ("should start with no templates", async function() {
    const supply = await contract.templatesSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of templates is not 0");
  });

  it ("should start with no instances", async function() {
    const supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of instances is not 0");
  });

  it ("should allow privileged address to create a template", async function() {
    const transaction = await contract.createTemplate("T1", 1, { from: accounts[0] });
    assert.equal(transaction.logs[0].event, "TemplateCreated", "expected a TemplateCreated event");

    const supply = await contract.templatesSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of templates is not 1");
  });

  it ("should not allow unprivileged address to create a template", async function() {
    let transaction = null;

    try {
      transaction = await contract.createTemplate("T1", 1, { from: accounts[1] });
    } catch (error) {
      err = error
    }
    assert.equal(transaction, null, "transaction should not exist");

    const supply = await contract.templatesSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of templates is not 0");
  });

  it ("should allow privileged address to create multiple templates", async function() {
    let transaction;
    let supply;

    transaction = await contract.createTemplate("T1", 1, { from: accounts[0] });
    assert.equal(transaction.logs[0].event, "TemplateCreated", "expected a TemplateCreated event");

    supply = await contract.templatesSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of templates is not 1");

    transaction = await contract.createTemplate("T2", 1, { from: accounts[0] });
    assert.equal(transaction.logs[0].event, "TemplateCreated", "expected a TemplateCreated event");

    supply = await contract.templatesSupply.call();
    assert.equal(supply.valueOf(), 2, "supply of templates is not 2");
  });

  it ("should not allow a template with a mint limit less than one to be created", async function() {
    let transaction = null;

    try {
      transaction = await contract.createTemplate("T1", 0, { from: accounts[0] });
    } catch (error) {
      err = error
    }
    assert.equal(transaction, null, "transaction should not exist");

    const supply = await contract.templatesSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of templates is not 0");
  });

  it ("should store instance mint limit for a new template", async function() {
    let transaction;
    let limit;

    transaction = await contract.createTemplate("T1", 3, { from: accounts[0] });
    limit = await contract.instanceLimit.call(0);
    assert.equal(limit.toNumber(), 3, "instance limit is not 3");

    transaction = await contract.createTemplate("T2", 8, { from: accounts[0] });
    limit = await contract.instanceLimit.call(1);
    assert.equal(limit.toNumber(), 8, "instance limit is not 8");

    // Instance limit of first template should be unchanged.
    limit = await contract.instanceLimit.call(0);
    assert.equal(limit.toNumber(), 3, "instance limit is not 3");
  });
});
