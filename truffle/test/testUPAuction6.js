const UniformPriceAuction = artifacts.require("./UniformPriceAuction.sol");

contract('UniformPriceAuction', function(accounts) {

  let contract;

  const minter = accounts[0];
  const buyer = accounts[1];
  const buyerTwo = accounts[2];

  const N = 3;
  const duration = 3600;
  const minimumBid = 1e17;
  const minimumIncrease = 1e16;

  describe ("should nots", function() {
    beforeEach(async function() {
      contract = await UniformPriceAuction.new();
      await contract.initializeAuction(
        "UniformPriceAuction",
        N,
        duration,
        minimumBid,
        minimumIncrease,
        { from: minter }
      );
    });

    it ("should not allow referrer to equal sender", async function() {
      const bidIndex = 0;

      let transaction = null;

      try {
        transaction = await contract.submitBid(
          bidIndex,
          buyerTwo,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease - 1e15,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });

    it ("should not allow referrer to equal sender", async function() {
      const bidIndex = 0;

      let transaction = null;

      try {
        transaction = await contract.submitBid(
          bidIndex,
          buyer,
          {
            from: buyer,
            value: minimumBid + minimumIncrease - 1e15,
          }
        );
      } catch (error) {
        err = error
      }
      assert.equal(transaction, null, "response should not exist");
    });
  });

  describe ("shoulds", function() {
    beforeEach(async function() {
      contract = await UniformPriceAuction.new();
      await contract.initializeAuction(
        "UniformPriceAuction",
        N,
        duration,
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
    });

    it ("should allow bid with valid referrer", async function() {
      const bidAmount = minimumBid + minimumIncrease;
      const bidIndex = 0;

      let contractBalance;

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid);

      const transaction = await contract.submitBid(
        bidIndex,
        buyerTwo,
        {
          from: buyer,
          value: bidAmount,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BidCreated", "expected a BidCreated event");
      assert.equal(transaction.logs[0].args.bidder, buyer);
      assert.equal(transaction.logs[0].args.referrer, buyerTwo);

      const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
      assert.equal(amount, bidAmount);
      assert.equal(bidder, buyer);

      const [highestBidAmount, highestBidBidder] = await contract.highestBid.call();
      assert.equal(highestBidAmount, bidAmount);
      assert.equal(highestBidBidder, buyer);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, bidAmount);

      const indicesCount = await contract.indicesCountOfOwner.call(buyer);
      assert.equal(indicesCount, 1);
    });

    it ("should allow another bid with valid referrer", async function() {
      const bidAmount = minimumBid + minimumIncrease * 2;
      const bidIndex = 0;

      let contractBalance;

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, minimumBid);

      const transaction = await contract.submitBid(
        bidIndex,
        buyer,
        {
          from: buyerTwo,
          value: bidAmount,
        }
      );
      assert.equal(transaction.receipt.status, '0x01', "transaction should exist");
      assert.equal(transaction.logs[0].event, "BidCreated", "expected a BidCreated event");
      assert.equal(transaction.logs[0].args.bidder, buyerTwo);
      assert.equal(transaction.logs[0].args.referrer, buyer);

      const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
      assert.equal(amount, bidAmount);
      assert.equal(bidder, buyerTwo);

      const [highestBidAmount, highestBidBidder] = await contract.highestBid.call();
      assert.equal(highestBidAmount, bidAmount);
      assert.equal(highestBidBidder, buyerTwo);

      contractBalance = web3.eth.getBalance(contract.address).toNumber();
      assert.equal(contractBalance, bidAmount);

      const indicesCount = await contract.indicesCountOfOwner.call(buyerTwo);
      assert.equal(indicesCount, 1);
    });
  });
});
