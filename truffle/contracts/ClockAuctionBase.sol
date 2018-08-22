pragma solidity ^0.4.23;

contract ClockAuctionBase {
  function createAuction(
    uint256 _tokenId,
    uint256 _startingPrice,
    uint256 _endingPrice,
    uint256 _duration,
    address _seller
  ) external;

  function isSaleAuction() public returns (bool);
}
