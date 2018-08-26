pragma solidity ^0.4.23;

/**
 * @title Ownable
 * @dev The Ownable contract has an owner address, and provides basic authorization control
 * functions, this simplifies the implementation of "user permissions".
 */
contract Ownable {
  address public owner;

  /**
   * @dev The Ownable constructor sets the original `owner` of the contract to the sender
   * account.
   */
  constructor() public {
    owner = msg.sender;
  }

  /**
   * @dev Throws if called by any account other than the owner.
   */
  modifier onlyOwner() {
    require(msg.sender == owner);
    _;
  }
}

/**
 * @title Pausable
 * @dev Base contract which allows children to implement an emergency stop mechanism.
 */
contract Pausable is Ownable {
  event Pause();
  event Unpause();

  bool public paused = false;

  /**
   * @dev Modifier to make a function callable only when the contract is not paused.
   */
  modifier whenNotPaused() {
    require(!paused);
    _;
  }

  /**
   * @dev Modifier to make a function callable only when the contract is paused.
   */
  modifier whenPaused() {
    require(paused);
    _;
  }

  /**
   * @dev called by the owner to pause, triggers stopped state
   */
  function pause() onlyOwner whenNotPaused public {
    paused = true;
    emit Pause();
  }

  /**
   * @dev called by the owner to unpause, returns to normal state
   */
  function unpause() onlyOwner whenPaused public {
    paused = false;
    emit Unpause();
  }
}

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
   * 0 = no rarity guarantees
   * 1 = guarantee one epic or better
   * 2 = guarantee one legendary or better
   * 3 = no rarity guarantees, all gilded
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
    return (basePrice - ((discountPoints() * basePrice) / 100)) * boxCount;
  }

  function discountPoints() public view returns (uint256) {
    // 7-day discount period
    // 1st day = 21% discount
    // 2nd day = 18% discount
    // ...
    uint256 blocksPassed = block.number - blockStart;
    uint256 daysPassed = blocksPassed / blocksPerDay;
    if (daysPassed < 7) {
      return 21 - (daysPassed * 3);
    } else {
      return 0;
    }
  }

  function priceByCategory(uint256 _category) public pure returns (uint256) {
    if (_category == 0) {
      return 30 finney;
    } else if (_category == 1) {
      return 60 finney;
    } else if (_category == 2) {
      return 240 finney;
    } else {
      return 1500 finney;
    }
  }

  function getBalance() external view returns (uint256) {
    return address(this).balance;
  }

  // ONLY OWNER
  function withdraw() external onlyOwner {
    owner.transfer(address(this).balance);
  }
}
