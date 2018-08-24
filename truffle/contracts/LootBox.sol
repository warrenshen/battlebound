pragma solidity ^0.4.23;

import "./Pausable.sol";

contract LootBox is Pausable {

  event BoxesBought(
    address indexed _owner,
    address indexed _referrer,
    uint256 indexed _category,
    uint256 _quantity
  );

  // Block number at which this contract was created.
  uint256 public blockStart;
  // Total number of boxes bought so far.
  uint256 public boughtCount = 0;
  // Number of blocks per day.
  uint256 public blocksPerDay = 6000;

  constructor() public {
    blockStart = block.number;
  }

  /*
   * Categories:
   * 0 = no guarantees
   * 1 = guarantee epic
   * 2 = guarantee legendary
   */
  function buyBoxes(
    address _owner,
    address _referrer,
    uint256 _category,
    uint256 _quantity
  ) external payable whenNotPaused {
    require(_owner != address(0));
    require(_quantity > 0 && _quantity <= 20);
    require(msg.sender != _referrer);

    uint256 price = calculatePrice(priceByCategory(_category), _quantity);
    require(msg.value >= price);

    boughtCount += _quantity;
    emit BoxesBought(_owner, _referrer, _category, _quantity);
  }

  function calculatePrice(uint256 basePrice, uint256 boxCount) public view returns (uint) {
    uint256 blocksPassed = block.number - blockStart;
    uint256 daysPassed = blocksPassed / blocksPerDay;
    // 7-day discount period
    // 1st day = 21% discount
    // 2nd day = 18% discount
    // ...
    if (daysPassed < 7) {
      return (basePrice - (((21 - (daysPassed * 3)) * basePrice) / 100)) * boxCount;
    }
    return basePrice * boxCount;
  }

  function priceByCategory(uint256 _category) public pure returns (uint) {
    if (_category == 0) {
      return 50 finney;
    } else if (_category == 1) {
      return 100 finney;
    } else if (_category == 2) {
      return 300 finney;
    } else {
      return 5000 finney;
    }
  }

  function getBalance() external view returns (uint) {
    return address(this).balance;
  }

  // ONLY OWNER
  function withdraw() external onlyOwner {
    owner.transfer(address(this).balance);
  }
}
