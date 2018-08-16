pragma solidity ^0.4.23;

// Contract to "waste" blocks for truffle tests.
contract BlockMiner {
  uint blocksMined;

  constructor() {
    blocksMined = 0;
  }

  function mine() external {
    blocksMined += 1;
  }
}
