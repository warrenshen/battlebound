pragma solidity ^0.4.23;

import "./Ownable.sol";
import "./ClockAuctionBase.sol";

/**
 * @title ERC721 token receiver interface
 * @dev Interface for any contract that wants to support safeTransfers
 * from ERC721 asset contracts.
 */
contract ERC721Receiver {
  /**
   * @dev Magic value to be returned upon successful reception of an NFT
   *  Equals to `bytes4(keccak256("onERC721Received(address,address,uint256,bytes)"))`,
   *  which can be also obtained as `ERC721Receiver(0).onERC721Received.selector`
   */
  bytes4 internal constant ERC721_RECEIVED = 0x150b7a02;

  /**
   * @notice Handle the receipt of an NFT
   * @dev The ERC721 smart contract calls this function on the recipient
   * after a `safetransfer`. This function MAY throw to revert and reject the
   * transfer. Return of other than the magic value MUST result in the
   * transaction being reverted.
   * Note: the contract address is always the message sender.
   * @param _operator The address which called `safeTransferFrom` function
   * @param _from The address which previously owned the token
   * @param _tokenId The NFT identifier which is being transferred
   * @param _data Additional data with no specified format
   * @return `bytes4(keccak256("onERC721Received(address,address,uint256,bytes)"))`
   */
  function onERC721Received(
    address _operator,
    address _from,
    uint256 _tokenId,
    bytes _data
  )
    public
    returns(bytes4);
}

/**
 * @title SafeMath
 * @dev Math operations with safety checks that revert on error
 */
library SafeMath {
  /**
  * @dev Adds two numbers, reverts on overflow.
  */
  function add(uint256 _a, uint256 _b) internal pure returns (uint256) {
    uint256 c = _a + _b;
    require(c >= _a);

    return c;
  }

  /**
  * @dev Subtracts two numbers, reverts on overflow (i.e. if subtrahend is greater than minuend).
  */
  function sub(uint256 _a, uint256 _b) internal pure returns (uint256) {
    require(_b <= _a);
    uint256 c = _a - _b;

    return c;
  }
}

/**
 * Utility library of inline functions on addresses
 */
library AddressUtils {
  /**
   * Returns whether the target address is a contract
   * @dev This function will return false if invoked during the constructor of a contract,
   * as the code is not actually created until after the constructor finishes.
   * @param _account address of the account to check
   * @return whether the target address is a contract
   */
  function isContract(address _account) internal view returns (bool) {
    uint256 size;
    // XXX Currently there is no better way to check if there is a contract in an address
    // than to check the size of the code at that address.
    // See https://ethereum.stackexchange.com/a/14016/36603
    // for more details about how this works.
    // TODO Check this again before the Serenity release, because all addresses will be
    // contracts then.
    // solium-disable-next-line security/no-inline-assembly
    assembly { size := extcodesize(_account) }
    return size > 0;
  }
}

contract CardBase is Ownable {
  bytes4 constant InterfaceSignature_ERC165 = 0x01ffc9a7;
  bytes4 constant InterfaceSignature_ERC721 = 0x80ac58cd;
  bytes4 internal constant InterfaceId_ERC721Exists = 0x4f558e79;

  /// @notice Introspection interface as per ERC-165 (https://github.com/ethereum/EIPs/issues/165).
  ///  Returns true for any standardized interfaces implemented by this contract. We implement
  ///  ERC-165 (obviously!) and ERC-721.
  function supportsInterface(bytes4 _interfaceID) external view returns (bool)
  {
    return (
      (_interfaceID == InterfaceSignature_ERC165) ||
      (_interfaceID == InterfaceSignature_ERC721) ||
      (_interfaceID == InterfaceId_ERC721Exists)
    );
  }
}

contract CardMint is CardBase {

  using SafeMath for uint256;
  using AddressUtils for address;

  /* EVENTS */
  event TemplateMinted(uint256 _templateId);
  event CardMinted(address _owner, uint256 _cardId);
  event Transfer(
    address indexed _from,
    address indexed _to,
    uint256 indexed _tokenId
  );
  event Approval(
    address indexed _owner,
    address indexed _approved,
    uint256 indexed _tokenId
  );
  event ApprovalForAll(
    address indexed _owner,
    address indexed _operator,
    bool _approved
  );

  /* DATA TYPES */
  struct Template {
    uint128 generation;
    uint64 category;
    uint64 power;
    string name;
  }

  // Each Card is a template ID (index of a template in `templates`) and a variation.
  struct Card {
    uint256 templateId;
    uint256 variation;
  }

  /* STORAGE */
  // Minter address can mint cards but not templates.
  address public minter;

  Template[] internal templates;
  Card[] internal cards;

  // Template ID => max number of cards that can be minted with this template ID.
  mapping (uint256 => uint256) internal templateIdToMintLimit;
  // Template ID => number of cards that have been minted with this template ID.
  mapping (uint256 => uint256) internal templateIdToMintCount;
  // Card ID => owner of card.
  mapping (uint256 => address) internal cardIdToOwner;
  // Owner => number of cards owner owns.
  mapping (address => uint256) internal ownerToCardCount;
  // Card ID => address approved to transfer on behalf of owner.
  mapping (uint256 => address) internal cardIdToApproved;
  // Operator => from address to operated or not.
  mapping (address => mapping (address => bool)) internal operatorToApprovals;

  ClockAuctionBase public saleAuction;

  /* MODIFIERS */
  modifier onlyMinter() {
    require(minter != address(0));
    require(msg.sender == minter);
    _;
  }

  /* FUNCTIONS */
  /** PRIVATE FUNCTIONS **/
  function _mintCard(
    uint256 _templateId,
    uint256 _variation,
    address _owner
  ) internal returns (uint256) {
    require(templateIdToMintCount[_templateId] < templateIdToMintLimit[_templateId]);
    templateIdToMintCount[_templateId] = templateIdToMintCount[_templateId].add(1);

    Card memory newCard = Card({
      templateId: _templateId,
      variation: _variation
    });

    uint256 newCardId = cards.push(newCard).sub(1);

    ownerToCardCount[_owner] = ownerToCardCount[_owner].add(1);
    cardIdToOwner[newCardId] = _owner;

    emit Transfer(0, _owner, newCardId);
    emit CardMinted(_owner, newCardId);
    return newCardId;
  }

  /** PUBLIC FUNCTIONS **/
  function setMinter(address _minter) external onlyOwner {
    minter = _minter;
  }

  function mintTemplate(
    uint256 _mintLimit,
    uint128 _generation,
    uint64 _category,
    uint64 _power,
    string _name
  ) external onlyOwner returns (uint256) {
    require(_mintLimit > 0);

    Template memory newTemplate = Template({
      generation: _generation,
      category: _category,
      power: _power,
      name: _name
    });
    uint256 newTemplateId = templates.push(newTemplate).sub(1);
    templateIdToMintLimit[newTemplateId] = _mintLimit;

    emit TemplateMinted(newTemplateId);
    return newTemplateId;
  }

  function mintCard(
    uint256 _templateId,
    uint256 _variation,
    address _owner
  ) external onlyMinter returns (uint256) {
    require(_owner != address(0));
    return _mintCard(_templateId, _variation, _owner);
  }

  function mintCards(
    uint256[] _templateIds,
    uint256[] _variations,
    address _owner
  ) external onlyMinter {
    require(_owner != address(0));
    require(_templateIds.length == _variations.length);

    uint256 templateId;
    uint256 variation;

    for (uint256 i = 0; i < _templateIds.length; ++i) {
      templateId = _templateIds[i];
      variation = _variations[i];
      _mintCard(templateId, variation, _owner);
    }
  }
}

contract CardOwnership is CardMint {

  /* MODIFIERS */
  modifier onlyTokenOwner(uint256 _tokenId) {
    require(msg.sender == cardIdToOwner[_tokenId]);
    _;
  }

  /* FUNCTIONS */
  /** PRIVATE FUNCTIONS **/
  function _approve(address _owner, address _approved, uint256 _tokenId) internal {
    cardIdToApproved[_tokenId] = _approved;
    emit Approval(_owner, _approved, _tokenId);
  }

  /**
   * @dev Internal function to clear current approval of a given token ID
   * Reverts if the given address is not indeed the owner of the token
   * @param _owner owner of the token
   * @param _tokenId uint256 ID of the token to be transferred
   */
  function _clearApproval(address _owner, uint256 _tokenId) internal {
    require(ownerOf(_tokenId) == _owner);
    if (cardIdToApproved[_tokenId] != address(0)) {
      cardIdToApproved[_tokenId] = address(0);
    }
  }

  function _removeTokenFrom(address _from, uint256 _tokenId) internal {
    require(ownerOf(_tokenId) == _from);
    ownerToCardCount[_from] = ownerToCardCount[_from].sub(1);
    cardIdToOwner[_tokenId] = address(0);
  }

  function _addTokenTo(address _to, uint256 _tokenId) internal {
    require(cardIdToOwner[_tokenId] == address(0));
    cardIdToOwner[_tokenId] = _to;
    ownerToCardCount[_to] = ownerToCardCount[_to].add(1);
  }

  /** PUBLIC FUNCTIONS **/
  function approve(address _to, uint256 _tokenId) external {
    address owner = ownerOf(_tokenId);
    require(_to != owner);
    require(msg.sender == owner || isApprovedForAll(owner, msg.sender));

    cardIdToApproved[_tokenId] = _to;
    emit Approval(owner, _to, _tokenId);
  }

  function transferFrom(address _from, address _to, uint256 _tokenId) public {
    require(isApprovedOrOwner(msg.sender, _tokenId));
    require(_from != address(0));
    require(_to != address(0));
    require(_to != address(this));

    _clearApproval(_from, _tokenId);
    _removeTokenFrom(_from, _tokenId);
    _addTokenTo(_to, _tokenId);

    emit Transfer(_from, _to, _tokenId);
  }

  /**
   * @dev Safely transfers the ownership of a given token ID to another address
   * If the target address is a contract, it must implement `onERC721Received`,
   * which is called upon a safe transfer, and return the magic value
   * `bytes4(keccak256("onERC721Received(address,address,uint256,bytes)"))`; otherwise,
   * the transfer is reverted.
   *
   * Requires the msg sender to be the owner, approved, or operator
   * @param _from current owner of the token
   * @param _to address to receive the ownership of the given token ID
   * @param _tokenId uint256 ID of the token to be transferred
  */
  function safeTransferFrom(
    address _from,
    address _to,
    uint256 _tokenId
  )
    public
  {
    // solium-disable-next-line arg-overflow
    safeTransferFrom(_from, _to, _tokenId, "");
  }

  /**
   * @dev Safely transfers the ownership of a given token ID to another address
   * If the target address is a contract, it must implement `onERC721Received`,
   * which is called upon a safe transfer, and return the magic value
   * `bytes4(keccak256("onERC721Received(address,address,uint256,bytes)"))`; otherwise,
   * the transfer is reverted.
   * Requires the msg sender to be the owner, approved, or operator
   * @param _from current owner of the token
   * @param _to address to receive the ownership of the given token ID
   * @param _tokenId uint256 ID of the token to be transferred
   * @param _data bytes data to send along with a safe transfer check
   */
  function safeTransferFrom(
    address _from,
    address _to,
    uint256 _tokenId,
    bytes _data
  )
    public
  {
    transferFrom(_from, _to, _tokenId);
    // solium-disable-next-line arg-overflow
    require(checkAndCallSafeTransfer(_from, _to, _tokenId, _data));
  }

  function checkAndCallSafeTransfer(
    address _from,
    address _to,
    uint256 _tokenId,
    bytes _data
  )
    internal
    returns (bool)
  {
    if (!_to.isContract()) {
      return true;
    }
    bytes4 retval = ERC721Receiver(_to).onERC721Received(
      msg.sender, _from, _tokenId, _data);
    return (retval == 0x150b7a02);
  }

  /**
   * @dev Returns whether the given spender can transfer a given token ID
   * @param _spender address of the spender to query
   * @param _tokenId uint256 ID of the token to be transferred
   * @return bool whether the msg.sender is approved for the given token ID,
   *  is an operator of the owner, or is the owner of the token
   */
  function isApprovedOrOwner(
    address _spender,
    uint256 _tokenId
  )
    internal
    view
    returns (bool)
  {
    address owner = ownerOf(_tokenId);
    return (
      _spender == owner ||
      getApproved(_tokenId) == _spender ||
      isApprovedForAll(owner, _spender)
    );
  }

  /**
   * @dev Sets or unsets the approval of a given operator
   * An operator is allowed to transfer all tokens of the sender on their behalf
   * @param _to operator address to set the approval
   * @param _approved representing the status of the approval to be set
   */
  function setApprovalForAll(address _to, bool _approved) public {
    require(_to != msg.sender);
    operatorToApprovals[msg.sender][_to] = _approved;
    emit ApprovalForAll(msg.sender, _to, _approved);
  }

  /**
   * @dev Gets the approved address for a token ID, or zero if no address set
   * @param _tokenId uint256 ID of the token to query the approval of
   * @return address currently approved for the given token ID
   */
  function getApproved(uint256 _tokenId) public view returns (address) {
    return cardIdToApproved[_tokenId];
  }

  /**
   * @dev Tells whether an operator is approved by a given owner
   * @param _owner owner address which you want to query the approval of
   * @param _operator operator address which you want to query the approval of
   * @return bool whether the given operator is approved by the given owner
   */
  function isApprovedForAll(
    address _owner,
    address _operator
  )
    public
    view
    returns (bool)
  {
    return operatorToApprovals[_owner][_operator];
  }

  function setSaleAuction(address _address) external onlyOwner {
    ClockAuctionBase candidateContract = ClockAuctionBase(_address);
    require(candidateContract.isSaleAuction());
    saleAuction = candidateContract;
  }

  function createSaleAuction(
    uint256 _tokenId,
    uint256 _startingPrice,
    uint256 _endingPrice,
    uint256 _duration
  )
    external
    onlyTokenOwner(_tokenId)
  {
    require(saleAuction != address(0));
    _approve(msg.sender, saleAuction, _tokenId);
    saleAuction.createAuction(
        _tokenId,
        _startingPrice,
        _endingPrice,
        _duration,
        msg.sender
    );
  }

  function ownerOf(uint256 _tokenId) public view returns (address) {
    address owner = cardIdToOwner[_tokenId];
    require(owner != address(0));
    return owner;
  }

  function exists(uint256 _tokenId) public view returns (bool) {
    address owner = cardIdToOwner[_tokenId];
    return owner != address(0);
  }
}

contract CardTreasury is CardOwnership {

  /* FUNCTIONS */
  /** PUBLIC FUNCTIONS **/
  function getTemplate(uint256 _templateId)
    external
    view
    returns (
      uint128 generation,
      uint64 category,
      uint64 power,
      string name
    )
  {
    require(_templateId < templates.length);

    Template storage template = templates[_templateId];

    generation = template.generation;
    category = template.category;
    power = template.power;
    name = template.name;
  }

  function getCard(uint256 _cardId)
    external
    view
    returns (
      uint128 generation,
      uint64 category,
      uint64 power,
      string name,
      uint256 variation
    )
  {
    require(_cardId < cards.length);

    Card storage card = cards[_cardId];
    Template storage template = templates[card.templateId];

    generation = template.generation;
    category = template.category;
    power = template.power;
    name = template.name;
    variation = card.variation;
  }

  function templateIdOf(uint256 _tokenId) external view returns (uint256) {
    require(_tokenId < cards.length);
    return cards[_tokenId].templateId;
  }

  function balanceOf(address _owner) public view returns (uint256) {
    require(_owner != address(0));
    return ownerToCardCount[_owner];
  }

  function templateSupply() external view returns (uint256) {
    return templates.length;
  }

  function totalSupply() external view returns (uint256) {
    return cards.length;
  }

  function instanceLimit(uint256 _templateId) external view returns(uint256) {
    return templateIdToMintLimit[_templateId];
  }

  function name() external pure returns (string) {
    return "Battlebound";
  }

  function symbol() external pure returns (string) {
    return "BB";
  }

  function tokensOfOwner(address _owner) external view returns (uint256[]) {
    uint256 tokenCount = balanceOf(_owner);

    if (tokenCount == 0) {
      return new uint256[](0);
    } else {
      uint256[] memory result = new uint256[](tokenCount);
      uint256 resultIndex = 0;

      uint256 cardId;

      for (cardId = 0; cardId < cards.length; ++cardId) {
        if (cardIdToOwner[cardId] == _owner) {
          result[resultIndex] = cardId;
          ++resultIndex;
        }
      }

      return result;
    }
  }

  // function tokenURI(uint256 _tokenId) external pure returns (string) {
  //   return
  // }
}
