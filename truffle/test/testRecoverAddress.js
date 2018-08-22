const AddressVerification = artifacts.require("./AddressVerification.sol");

contract('AddressVerification', function(accounts) {

  let contract;
  const h = '0x4622181004442308388000833281088716594456860636412994598425273769';

  before(async function() {
    contract = await AddressVerification.new();
  });

  it ("should recover address when signature is correct", async function() {
    const signature = "0x8fba503f72dd8f43c4d9ea17d26813fa20f9ae2c7ca0d6bccfd14a0b1e78a9b0610f8504e4c93c0af46a99b0c22b35c0f6ff400ff83c7144fad687651eced11a1c".slice(2);

    const r = `0x${signature.slice(0, 64)}`;
    const s = `0x${signature.slice(64, 128)}`;
    const v = web3.toDecimal("0x" + signature.slice(128, 130));

    const address = await contract.recoverAddress.call(h, v, r, s);
    assert.equal(address, "0x627306090abaB3A6e1400e9345bC60c78a8BEf57".toLowerCase(), "addresses should match");
  });

  it ("should not recover address when nonce is incorrect", async function() {
    const signature = web3.eth.sign(accounts[1], h).slice(2);

    const r = `0x${signature.slice(0, 64)}`;
    const s = `0x${signature.slice(64, 128)}`;
    const v = web3.toDecimal(signature.slice(128, 130)) + 27;

    const address = await contract.recoverAddress.call(h, v, r, s);
    assert.notEqual(address, accounts[0], "addresses should not match");
  });

  it ("should not recover address when signature is incorrect", async function() {
    const signature = web3.eth.sign(accounts[1], h).slice(2);

    const r = `0x${signature.slice(0, 64)}`;
    const s = `0x${signature.slice(64, 128)}`;
    const v = web3.toDecimal(signature.slice(128, 130)) + 27;

    const address = await contract.recoverAddress.call(h, v, r, s);
    assert.notEqual(address, accounts[0], "addresses should not match");
  });

  it ("should recover address when nonce and signature are correct", async function() {
    const signature = web3.eth.sign(accounts[0], h).slice(2);

    const r = `0x${signature.slice(0, 64)}`;
    const s = `0x${signature.slice(64, 128)}`;
    const v = web3.toDecimal(signature.slice(128, 130)) + 27;

    const address = await contract.recoverAddress.call(h, v, r, s);
    assert.equal(address, accounts[0], "addresses should match");
  });
});
