const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];

  beforeEach(async function() {
     contract = await CardTreasury.new();
  });

  it ("should allow privileged address to mint a template", async function() {
    const transaction = await contract.mintTemplate(1, 0, 0, 9, "Lux", { from: minter });
    assert.equal(transaction.logs[0].event, "TemplateMinted");
    assert.equal(transaction.logs[0].args.templateId, 0);

    const supply = await contract.templateSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of templates is not 1");
  });

  it ("should not allow unprivileged address to create a template", async function() {
    let transaction = null;

    try {
      transaction = await contract.mintTemplate(1, 0, 0, 9, "Lux", { from: buyer });
    } catch (error) {
      err = error
    }
    assert.equal(transaction, null, "transaction should not exist");

    const supply = await contract.templateSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of templates is not 0");
  });

  it ("should allow privileged address to create multiple templates", async function() {
    let transaction;
    let supply;

    transaction = await contract.mintTemplate(1, 0, 0, 9, "Lux", { from: minter });
    assert.equal(transaction.logs[0].event, "TemplateMinted");
    assert.equal(transaction.logs[0].args.templateId, 0);

    supply = await contract.templateSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of templates is not 1");

    transaction = await contract.mintTemplate(1, 0, 0, 9, "Talusreaver", { from: minter });
    assert.equal(transaction.logs[0].event, "TemplateMinted");
    assert.equal(transaction.logs[0].args.templateId, 1);

    supply = await contract.templateSupply.call();
    assert.equal(supply.valueOf(), 2, "supply of templates is not 2");
  });

  it ("should not allow a template with a mint limit less than one to be created", async function() {
    let transaction = null;

    try {
      transaction = await contract.mintTemplate(0, 0, 0, 9, "Lux", { from: minter });
    } catch (error) {
      err = error
    }
    assert.equal(transaction, null, "transaction should not exist");

    const supply = await contract.templateSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of templates is not 0");
  });

  it ("should store instance mint limit for a new template", async function() {
    let transaction;
    let limit;

    transaction = await contract.mintTemplate(3, 0, 0, 9, "Lux", { from: minter });
    assert.equal(transaction.logs[0].event, "TemplateMinted");
    assert.equal(transaction.logs[0].args.templateId, 0);

    limit = await contract.instanceLimit.call(0);
    assert.equal(limit.toNumber(), 3, "instance limit is not 3");

    transaction = await contract.mintTemplate(8, 0, 0, 9, "Talusreaver", { from: minter });
    assert.equal(transaction.logs[0].event, "TemplateMinted");
    assert.equal(transaction.logs[0].args.templateId, 1);

    limit = await contract.instanceLimit.call(1);
    assert.equal(limit.toNumber(), 8, "instance limit is not 8");

    // Instance limit of first template should be unchanged.
    limit = await contract.instanceLimit.call(0);
    assert.equal(limit.toNumber(), 3, "instance limit is not 3");
  });
});
