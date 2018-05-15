const CardTreasury = artifacts.require("./CardTreasury.sol");

contract('CardTreasury', function(accounts) {

  let contract;

  beforeEach(async function() {
    contract = await CardTreasury.new();
    contract.createTemplate("T1", 2, { from: accounts[0] });
  });

  it ("should allow privileged address to mint a card for self", async function() {
    const recipient = accounts[0];

    const transaction = await contract.mintCard(0, recipient);
    assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
    assert.equal(transaction.logs[1].event, "InstanceMinted", "expected an InstanceMinted event");

    const supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

    const owner = await contract.ownerOf.call(0);
    assert.equal(owner, recipient, "owner is not correct");
  });

  it ("should allow privileged address to mint a card for other", async function() {
    const recipient = accounts[1];

    const transaction = await contract.mintCard(0, recipient);
    assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
    assert.equal(transaction.logs[1].event, "InstanceMinted", "expected an InstanceMinted event");

    const supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of instances is not 1");

    const owner = await contract.ownerOf.call(0);
    assert.equal(owner, recipient, "owner is not correct");
  });

  it ("should not allow unprivileged address to mint a card", async function() {
    let transaction = null;

    try {
      transaction = await contract.mintCard(0, accounts[1], { from: accounts[1] });
    } catch (error) {
      err = error
    }
    assert.equal(transaction, null, "transaction should not exist");

    const supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 0, "supply of instances is not 0");
  });

  it ("should allow privileged address to mint multiple cards", async function() {
    const recipient = accounts[0];
    let owner;
    let transaction;
    let supply;

    transaction = await contract.mintCard(0, recipient, { from: accounts[0] });
    assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
    assert.equal(transaction.logs[1].event, "InstanceMinted", "expected an InstanceMinted event");

    supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 1, "supply of templates is not 1");

    transaction = await contract.mintCard(0, recipient, { from: accounts[0] });
    assert.equal(transaction.logs[0].event, "Transfer", "expected a Transfer event");
    assert.equal(transaction.logs[1].event, "InstanceMinted", "expected an InstanceMinted event");

    supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 2, "supply of templates is not 2");

    owner = await contract.ownerOf.call(0);
    assert.equal(owner, recipient, "owner is not correct");

    owner = await contract.ownerOf.call(1);
    assert.equal(owner, recipient, "owner is not correct");
  });

  it ("should not allow more cards than mint limit to be minted", async function() {
    let transaction;
    let supply;

    await contract.mintCard(0, accounts[0], { from: accounts[0] });
    await contract.mintCard(0, accounts[0], { from: accounts[0] });

    transaction = null;

    try {
      transaction = await contract.mintCard(0, accounts[0], { from: accounts[0] });
    } catch (error) {
      err = error
    }
    assert.equal(transaction, null, "transaction should not exist");

    supply = await contract.instancesSupply.call();
    assert.equal(supply.valueOf(), 2, "supply of templates is not 2");
  });
});