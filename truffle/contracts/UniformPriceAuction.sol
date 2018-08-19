pragma solidity ^0.4.23;

import "./Ownable.sol";

contract UniformPriceAuction is Ownable {

  // EVENTS
  event BidCreated(
    uint256 indexed amount,
    address indexed bidder,
    address indexed referrer
  );

  // Human-readable name of auction.
  string public name;
  // Number of items on auction.
  uint256 public N;
  // Length of auction in unit of blocks, can change on valid bids.
  uint256 public duration;
  // Minimum amount a bid must be to be valid.
  uint256 public minimumBid;
  // Minimum amount more a bid must be to beat another.
  uint256 public minimumIncrease;

  // Block number when auction starts.
  uint256 public blockStart;
  // Uniform price winners pay.
  uint256 public fulfillPrice;

  struct Bid {
    uint256 amount;
    address bidder;
  }
  mapping (uint256 => Bid) indexToBid;

  // ONLY OWNER
  function initializeAuction(
    string _name,
    uint256 _N,
    uint256 _duration,
    uint256 _minimumBid,
    uint256 _minimumIncrease
  ) external onlyOwner {
    require(_N > 0);
    require(_duration > 0);
    require(_minimumBid > 0);
    require(_minimumIncrease > 0);

    name = _name;
    N = _N;
    duration = _duration;
    minimumBid = _minimumBid;
    minimumIncrease = _minimumIncrease;

    blockStart = block.number;
    fulfillPrice = _minimumBid;
  }

  function fulfillBid(uint256 _index) external onlyOwner {
    require(_index < N);
    require(block.number > blockStart + duration);

    Bid storage bid = indexToBid[_index];
    uint256 amount = bid.amount;
    require(amount >= fulfillPrice);

    address winner = bid.bidder;
    bid.amount = 0;
    bid.bidder = address(0);

    owner.send(fulfillPrice);
    winner.send(amount - fulfillPrice);
  }

  function refundBid(uint256 _index) external onlyOwner {
    require(_index < N);
    require(block.number > blockStart + duration);

    Bid storage bid = indexToBid[_index];
    uint256 amount = bid.amount;

    address winner = bid.bidder;
    bid.amount = 0;
    bid.bidder = address(0);

    winner.send(amount);
  }

  // EXTERNAL
  function submitBid(uint256 _index, address _referrer) external payable {
    require(_index < N);
    require(block.number < blockStart + duration);
    require(msg.value >= minimumBid);
    require(msg.sender != _referrer);

    Bid storage bid = indexToBid[_index];

    uint256 previousBid = bid.amount;
    address previousBidder = bid.bidder;

    require(previousBid + minimumIncrease > previousBid);
    require(msg.value >= previousBid + minimumIncrease);

    bid.amount = msg.value;
    bid.bidder = msg.sender;

    // Naively set the fulfill price (uniform price winners pay)
    // to this bid's amount. This is ok because the fulfill price
    // can be updated by anyone after the end of the auction.
    fulfillPrice = msg.value;

    if (block.number + 300 > blockStart + duration) {
      duration += 300;
    }

    if (previousBid > 0 && previousBidder != address(0)) {
      previousBidder.send(previousBid);
    }

    emit BidCreated(msg.value, msg.sender, _referrer);
  }

  function updateFulfillPrice(uint256 _index) external {
    require(_index < N);
    require(block.number > blockStart + duration);

    Bid storage bid = indexToBid[_index];
    uint256 amount = bid.amount;
    require(amount > 0);

    if (amount < fulfillPrice) {
      fulfillPrice = amount;
    }
  }

  // EXTERNAL VIEW
  function bidByIndex(uint256 _index)
    external
    view
    returns (uint256, address)
  {
    require(_index < N);
    Bid storage bid = indexToBid[_index];
    return (bid.amount, bid.bidder);
  }

  function indicesOfThreeLowestBids()
    external
    view
    returns (uint256, uint256, uint256)
  {
    uint256 lowestBidAmountOne = 2**256 - 1;
    uint256 lowestBidAmountTwo = 2**256 - 1;
    uint256 lowestBidAmountThree = 2**256 - 1;

    uint256 lowestBidIndexOne = 0;
    uint256 lowestBidIndexTwo = 0;
    uint256 lowestBidIndexThree = 0;

    Bid storage bid;

    for (uint256 i = 0; i < N; i += 1) {
      bid = indexToBid[i];
      if (bid.amount < lowestBidAmountOne) {
        lowestBidAmountOne = bid.amount;
        lowestBidIndexOne = i;
      }
      if (bid.amount == 0) {
        break;
      }
    }

    for (uint256 j = 0; j < N; j += 1) {
      bid = indexToBid[j];
      if (j != lowestBidIndexOne) {
        if (bid.amount < lowestBidAmountTwo) {
          lowestBidAmountTwo = bid.amount;
          lowestBidIndexTwo = j;
        }
        if (bid.amount == 0) {
          break;
        }
      }
    }

    for (uint256 k = 0; k < N; k += 1) {
      bid = indexToBid[k];
      if (k != lowestBidIndexOne && k != lowestBidIndexTwo) {
        if (bid.amount < lowestBidAmountThree) {
          lowestBidAmountThree = bid.amount;
          lowestBidIndexThree = k;
        }
        if (bid.amount == 0) {
          break;
        }
      }
    }

    return (lowestBidIndexOne, lowestBidIndexTwo, lowestBidIndexThree);
  }

  function highestBid() external view returns (uint256, address) {
    uint256 highestBidAmount = 0;
    address highestBidBidder = address(0);

    Bid storage bid;

    for (uint256 i = 0; i < N; i += 1) {
      bid = indexToBid[i];
      if (bid.amount > highestBidAmount) {
        highestBidAmount = bid.amount;
        highestBidBidder = bid.bidder;
      }
    }

    return (highestBidAmount, highestBidBidder);
  }

  function indicesCountOfOwner(address _owner)
    external
    view
    returns (uint256)
  {
    uint256 count = 0;

    Bid storage bid;

    for (uint256 i = 0; i < N; i += 1) {
      bid = indexToBid[i];
      if (bid.bidder == _owner) {
        count += 1;
      }
    }

    return count;
  }

  function auctionEndBlock() external view returns (uint256) {
    return blockStart + duration;
  }
}
