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
    describe ("first bid", function() {
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

      it ("should not allow bid less than minimum bid", async function() {
        let transaction = null;

        try {
          transaction = await contract.submitBid(
            0,
            {
              from: buyer,
              value: 1e16
            }
          );
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "response should not exist");
      });

      it ("should not allow bid on invalid index", async function() {
        let transaction = null;

        try {
          transaction = await contract.submitBid(
            N + 1,
            {
              from: buyer,
              value: minimumBid
            }
          );
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "response should not exist");
      });
    });

    describe ("second bid", function() {
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
          {
            from: buyer,
            value: minimumBid,
          }
        );
      });

      it ("should not allow bid less than previous bid plus minimum increase", async function() {
        const bidAmount = minimumBid;
        const bidIndex = 0;

        let transaction = null;

        try {
          transaction = await contract.submitBid(
            0,
            {
              from: buyerTwo,
              value: minimumBid + minimumIncrease - 1e15,
            }
          );
        } catch (error) {
          err = error
        }
        assert.equal(transaction, null, "response should not exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, minimumBid);
      });
    });
  });

  describe ("shoulds", function() {
    describe ("first bid", function() {
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

      it ("should allow bid of minimum amount", async function() {
        const bidAmount = minimumBid;
        const bidIndex = 0;

        const transaction = await contract.submitBid(
          bidIndex,
          {
            from: buyer,
            value: bidAmount,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, bidAmount);

        const fulfillPrice = await contract.getFulfillPrice.call();
        assert.equal(fulfillPrice, bidAmount);

        const contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, bidAmount);
      });

      it ("should allow bid greater than minimum amount", async function() {
        const bidAmount = minimumBid + 1e16;
        const bidIndex = 0;

        const transaction = await contract.submitBid(
          bidIndex,
          {
            from: buyer,
            value: minimumBid + 1e16,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, bidAmount);

        const contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, bidAmount);
      });
    });

    describe ("second bid", function() {
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
          {
            from: buyer,
            value: minimumBid
          }
        );
      });

      it ("should allow bid greater than previous own bid", async function() {
        const bidAmount = minimumBid + minimumIncrease;
        const bidIndex = 0;

        let contractBalance;

        contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, minimumBid);

        const transaction = await contract.submitBid(
          bidIndex,
          {
            from: buyer,
            value: bidAmount,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, bidAmount);

        contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, bidAmount);
      });

      it ("should allow bid greater than previous bid by other", async function() {
        const bidAmount = minimumBid + minimumIncrease;
        const bidIndex = 0;

        const transaction = await contract.submitBid(
          0,
          {
            from: buyerTwo,
            value: minimumBid + minimumIncrease,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyerTwo);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, bidAmount);

        const contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, bidAmount);
      });
    });

    describe ("multiple bids on different indices", function() {
      // BEFORE not BEFORE EACH
      before(async function() {
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
          {
            from: buyer,
            value: minimumBid
          }
        );
        await contract.submitBid(
          1,
          {
            from: buyer,
            value: minimumBid
          }
        );
        await contract.submitBid(
          2,
          {
            from: buyer,
            value: minimumBid
          }
        );
      });

      it ("should allow bid greater than previous bid", async function() {
        const bidAmount = minimumBid + minimumIncrease * 3;
        const bidIndex = 0;

        let contractBalance;
        contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, minimumBid * 3);

        const transaction = await contract.submitBid(
          bidIndex,
          {
            from: buyer,
            value: bidAmount,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, bidAmount);

        contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, minimumBid * 3 + minimumIncrease * 3);
      });

      it ("should allow bid greater than previous bid by other", async function() {
        const bidAmount = minimumBid + minimumIncrease * 3;
        const bidIndex = 1;

        const transaction = await contract.submitBid(
          bidIndex,
          {
            from: buyer,
            value: bidAmount,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, bidAmount);

        const contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, minimumBid * 3 + minimumIncrease * 6);
      });

      it ("should allow bid greater than previous bid but less than global highest bid", async function() {
        const bidAmount = minimumBid + minimumIncrease * 2;
        const bidIndex = 2;

        const transaction = await contract.submitBid(
          2,
          {
            from: buyer,
            value: bidAmount,
          }
        );
        assert.equal(transaction.receipt.status, '0x01', "transaction should exist");

        const [amount, bidder] = await contract.bidByIndex.call(bidIndex);
        assert.equal(amount, bidAmount);
        assert.equal(bidder, buyer);

        const highestBidAmount = await contract.highestBidAmount.call();
        assert.equal(highestBidAmount, minimumBid + minimumIncrease * 3);

        const contractBalance = web3.eth.getBalance(contract.address).toNumber();
        assert.equal(contractBalance, minimumBid * 3 + minimumIncrease * 8);
      });
    });
  });
});
