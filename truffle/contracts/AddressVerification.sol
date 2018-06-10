pragma solidity ^0.4.23;

contract AddressVerification {

  function recoverAddress(bytes32 h, uint8 v, bytes32 r, bytes32 s)
    external
    pure
    returns (address) {
    return ecrecover(keccak256(abi.encodePacked("\x19Ethereum Signed Message:\n32", h)), v, r, s);
  }
}
