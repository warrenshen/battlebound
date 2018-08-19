const UniformPriceAuction = artifacts.require("./UniformPriceAuction.sol");
const BlockMiner = artifacts.require("./BlockMiner.sol");

contract('UniformPriceAuction', function(accounts) {

  let contract;
  let miner;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  const N = 3;
  const minimumBid = 1e17;
  const minimumIncrease = 1e16;

  describe ("should nots", function() {
    const auctionDuration = 310;

    // BEFORE NOT BEFORE EACH
    before(async function() {
      contract = await UniformPriceAuction.new();
      miner = await BlockMiner.new();
      await contract.initializeAuction(
        "UniformPriceAuction",
        N,
        auctionDuration,
        minimumBid,
        minimumIncrease,
        { from: minter }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyer,
          value: minimumBid
        }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyer,
          value: minimumBid + minimumIncrease * 10
        }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyerTwo,
          value: minimumBid + minimumIncrease * 20
        }
      );
      await contract.submitBid(
        1,
        0,
        {
          from: buyerTwo,
          value: minimumBid
        }
      );
      await contract.submitBid(
        2,
        0,
        {
          from: buyerTwo,
          value: minimumBid
        }
      );
      await contract.submitBid(
        1,
        0,
        {
          from: buyer,
          value: minimumBid + minimumIncrease
        }
      );

      for (var i = 0; i < 400; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should not allow further bids", async function() {
      const contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 3 + minimumIncrease * 21);

      let transaction = null;

      try {
        transaction = await contract.submitBid(
          1,
          0,
          {
            from: buyer,
            value: minimumBid + minimumIncrease * 3
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow unprivileged address to fulfill bid", async function() {
      let transaction = null;

      try {
        transaction = await contract.fulfillBid(
          0,
          { from: buyer }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow fulfill bid with invalid index", async function() {
      let transaction = null;

      try {
        transaction = await contract.fulfillBid(
          N + 1,
          { from: minter }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    const auctionDuration = 310;

    // BEFORE NOT BEFORE EACH
    before(async function() {
      contract = await UniformPriceAuction.new();
      miner = await BlockMiner.new();
      await contract.initializeAuction(
        "UniformPriceAuction",
        N,
        auctionDuration,
        minimumBid,
        minimumIncrease,
        { from: minter }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyer,
          value: minimumBid
        }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyer,
          value: minimumBid + minimumIncrease * 10
        }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyerTwo,
          value: minimumBid + minimumIncrease * 20
        }
      );
      await contract.submitBid(
        1,
        0,
        {
          from: buyerTwo,
          value: minimumBid
        }
      );
      await contract.submitBid(
        2,
        0,
        {
          from: buyerTwo,
          value: minimumBid
        }
      );
      await contract.submitBid(
        1,
        0,
        {
          from: buyer,
          value: minimumBid + minimumIncrease
        }
      );

      for (var i = 0; i < 400; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should allow update fulfill price by buyer", async function() {
      const transaction = await contract.updateFulfillPrice(
        2,
        { from: buyer }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const fulfillPrice = await contract.fulfillPrice.call();
      assert.equal(fulfillPrice, minimumBid);
    });

    it ("should allow update fulfill price but do nothing for bad price", async function() {
      const transaction = await contract.updateFulfillPrice(
        1,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const fulfillPrice = await contract.fulfillPrice.call();
      assert.equal(fulfillPrice, minimumBid);
    });

    it ("should allow privileged address to fulfill bid", async function() {
      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 3 + minimumIncrease * 21);

      const beforeOwnerBalance = web3.eth.getBalance(minter).toNumber();
      const beforeBuyerBalance = web3.eth.getBalance(buyer).toNumber();
      const beforeBuyerTwoBalance = web3.eth.getBalance(buyerTwo).toNumber();

      const transaction = await contract.fulfillBid(
        0,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const afterOwnerBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(afterOwnerBalance > beforeOwnerBalance, true);

      const afterBuyerBalance = web3.eth.getBalance(buyer).toNumber();
      assert.equal(beforeBuyerBalance, afterBuyerBalance);

      const afterBuyerTwoBalance = web3.eth.getBalance(buyerTwo).toNumber();
      assert.equal(afterBuyerTwoBalance, beforeBuyerTwoBalance + minimumIncrease * 20);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 2 + minimumIncrease);
    });

    it ("should allow privileged address to fulfill another bid", async function() {
      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 2 + minimumIncrease);

      const beforeOwnerBalance = web3.eth.getBalance(minter).toNumber();

      const transaction = await contract.fulfillBid(
        1,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const afterOwnerBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(afterOwnerBalance > beforeOwnerBalance, true);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid);
    });

    it ("should allow privileged address to fulfill last bid", async function() {
      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid);

      const beforeOwnerBalance = web3.eth.getBalance(minter).toNumber();

      const transaction = await contract.fulfillBid(
        2,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const afterOwnerBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(afterOwnerBalance > beforeOwnerBalance, true);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, 0);
    });

    it ("should allow initialize new auction after fulfills", async function() {
      const auctionDuration = 6000;

      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, 0);

      const transaction = await contract.initializeAuction(
        "UniformPriceAuction2",
        N,
        auctionDuration * 2,
        minimumBid,
        minimumIncrease,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const name = await contract.name.call();
      assert.equal(name, "UniformPriceAuction2", "name is not correct");

      const duration = await contract.duration.call();
      assert.equal(duration, auctionDuration * 2);

      const blockStart = await contract.blockStart.call();
      assert.equal(blockStart > 0, true, "block start should be greater than 0");
    });
  });

  describe ("refund", function() {
    const auctionDuration = 310;

    // BEFORE NOT BEFORE EACH
    before(async function() {
      contract = await UniformPriceAuction.new();
      miner = await BlockMiner.new();
      await contract.initializeAuction(
        "UniformPriceAuction",
        N,
        auctionDuration,
        minimumBid,
        minimumIncrease,
        { from: minter }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyer,
          value: minimumBid
        }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyer,
          value: minimumBid + minimumIncrease * 10
        }
      );
      await contract.submitBid(
        0,
        0,
        {
          from: buyerTwo,
          value: minimumBid + minimumIncrease * 20
        }
      );
      await contract.submitBid(
        1,
        0,
        {
          from: buyerTwo,
          value: minimumBid
        }
      );
      await contract.submitBid(
        2,
        0,
        {
          from: buyerTwo,
          value: minimumBid
        }
      );
      await contract.submitBid(
        1,
        0,
        {
          from: buyer,
          value: minimumBid + minimumIncrease
        }
      );

      for (var i = 0; i < 400; i += 1) {
        transaction = await miner.mine();
      }
    });

    it ("should allow privileged address to refund bid", async function() {
      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 3 + minimumIncrease * 21);

      const beforeOwnerBalance = web3.eth.getBalance(minter).toNumber();
      const beforeBuyerBalance = web3.eth.getBalance(buyer).toNumber();
      const beforeBuyerTwoBalance = web3.eth.getBalance(buyerTwo).toNumber();

      const transaction = await contract.refundBid(
        0,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const afterOwnerBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(beforeOwnerBalance > afterOwnerBalance, true);

      const afterBuyerBalance = web3.eth.getBalance(buyer).toNumber();
      assert.equal(beforeBuyerBalance, afterBuyerBalance);

      const afterBuyerTwoBalance = web3.eth.getBalance(buyerTwo).toNumber();
      assert.equal(afterBuyerTwoBalance, beforeBuyerTwoBalance + minimumBid + minimumIncrease * 20);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 2 + minimumIncrease);
    });

    it ("should allow privileged address to refund another bid", async function() {
      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid * 2 + minimumIncrease);

      const beforeOwnerBalance = web3.eth.getBalance(minter).toNumber();

      const transaction = await contract.refundBid(
        1,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const afterOwnerBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(beforeOwnerBalance > afterOwnerBalance, true);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid);
    });

    it ("should allow privileged address to refund last bid", async function() {
      let contractBalance;
      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid);

      const beforeOwnerBalance = web3.eth.getBalance(minter).toNumber();

      const transaction = await contract.refundBid(
        2,
        { from: minter }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

      const afterOwnerBalance = web3.eth.getBalance(minter).toNumber();
      assert.equal(beforeOwnerBalance > afterOwnerBalance, true);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, 0);
    });
  });
});
