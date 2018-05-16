pragma solidity ^0.4.23;

import "./Ownable.sol";

// contract ERCXXXX {
//   // Events
//   event Transfer(address from, address to, uint256 tokenId);
//   event Approval(address owner, address approved, uint256 tokenId);

//   // Required methods
//   function balanceOf(address _owner) public view returns (uint256 balance);
//   function ownerOf(uint256 _tokenId) external view returns (address owner);

//   function approve(address _to, uint256 _tokenId) external;
//   function transfer(address _to, uint256 _tokenId) external;
//   function transferFrom(address _from, address _to, uint256 _tokenId) external;

//   function templatesSupply() public view returns (uint256 count);
//   function instancesSupply() public view returns (uint256 count);
//   function instanceLimit(uint256 _templateId) public view returns (uint256 limit);

//   // Optional
//   function name() public view returns (string);
//   function symbol() public view returns (string);
//   function tokensOfOwner(address _owner) external view returns (uint256[] tokenIds);
//   // function tokenMetadata(uint256 _tokenId, string _preferredTransport) public view returns (string infoUrl);

//   // ERC-165 Compatibility (https://github.com/ethereum/EIPs/issues/165)
//   // function supportsInterface(bytes4 _interfaceID) external view returns (bool);
// }

contract CardBase is Ownable {
    // bytes4 constant InterfaceSignature_ERC165 =
    //     bytes4(keccak256('supportsInterface(bytes4)'));

    // bytes4 constant InterfaceSignature_ERC721 =
    //     bytes4(keccak256('name()')) ^
    //     bytes4(keccak256('symbol()')) ^
    //     bytes4(keccak256('totalSupply()')) ^
    //     bytes4(keccak256('balanceOf(address)')) ^
    //     bytes4(keccak256('ownerOf(uint256)')) ^
    //     bytes4(keccak256('approve(address,uint256)')) ^
    //     bytes4(keccak256('transfer(address,uint256)')) ^
    //     bytes4(keccak256('transferFrom(address,address,uint256)')) ^
    //     bytes4(keccak256('tokensOfOwner(address)')) ^
    //     bytes4(keccak256('tokenMetadata(uint256,string)'));

    // /// @notice Introspection interface as per ERC-165 (https://github.com/ethereum/EIPs/issues/165).
    // ///  Returns true for any standardized interfaces implemented by this contract. We implement
    // ///  ERC-165 (obviously!) and ERC-721.
    // function supportsInterface(bytes4 _interfaceID) external view returns (bool)
    // {
    //     // DEBUG ONLY
    //     //require((InterfaceSignature_ERC165 == 0x01ffc9a7) && (InterfaceSignature_ERC721 == 0x9a20483d));

    //     return ((_interfaceID == InterfaceSignature_ERC165) || (_interfaceID == InterfaceSignature_ERC721));
    // }
}

contract CardMint is CardBase {

  /* EVENTS */
  event TemplateCreated(uint256 templateId);
  event InstanceMinted(address owner, uint256 cardId, uint256 templateId);

  event Transfer(address indexed _from, address indexed _to, uint256 _tokenId);
  event Approval(address _owner, address _approved, uint256 _tokenId);

  /* DATA TYPES */
  struct Template {
    uint128 generation;
    uint128 power;
    string name;
  }

  /* STORAGE */
  Template[] internal templates;
  // Each uint256 in `cards` is a template ID.
  uint256[] internal cards;
  // Template ID is index of template in `templates`.
  // Template ID => max number of cards that can be minted with this template ID.
  mapping (uint256 => uint256) internal templateIdToMintLimit;
  // Template ID => number of cards that have been minted with this template ID.
  mapping (uint256 => uint256) internal templateIdToMintCount;
  // Card ID is index of card in `cards`.
  // mapping (uint256 => uint256) internal cardIdToTemplateId;
  mapping (uint256 => address) internal cardIdToOwner;
  // Owner => number of card owner owns.
  mapping (address => uint256) internal ownerCardCount;
  // Card ID => address approved to transfer on behalf of owner.
  mapping (uint256 => address) internal cardIdToApproved;

  /* FUNCTIONS */
  /** PRIVATE FUNCTIONS **/
  function _transfer(address _from, address _to, uint256 _tokenId) internal {
    // TODO: safe math?
    ownerCardCount[_to] += 1;
    cardIdToOwner[_tokenId] = _to;

    if (_from != address(0)) {
      ownerCardCount[_from] -= 1;
      delete cardIdToApproved[_tokenId];
    }

    emit Transfer(_from, _to, _tokenId);
  }

  /** PUBLIC FUNCTIONS **/
  function createTemplate(
    uint256 _mintLimit,
    uint128 _generation,
    uint128 _power,
    string _name
  ) external onlyOwner returns (uint256) {
    require(_mintLimit > 0);

    Template memory newTemplate = Template({
      generation: _generation,
      power: _power,
      name: _name
    });
    uint256 newTemplateId = templates.push(newTemplate) - 1;
    templateIdToMintLimit[newTemplateId] = _mintLimit;

    emit TemplateCreated(newTemplateId);
    return newTemplateId;
  }

  function mintCard(uint256 _templateId, address _owner) external onlyOwner returns (uint256) {
    require(templateIdToMintCount[_templateId] < templateIdToMintLimit[_templateId]);
    // need safe math
    templateIdToMintCount[_templateId] = templateIdToMintCount[_templateId] + 1;
    uint256 newCardId = cards.push(_templateId) - 1;
    _transfer(0, _owner, newCardId);

    emit InstanceMinted(_owner, newCardId, _templateId);
    return newCardId;
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
    // Emit approval event.
    emit Approval(_owner, _approved, _tokenId);
  }

  function _approvedFor(address _claimant, uint256 _tokenId) internal view returns (bool) {
    return cardIdToOwner[_tokenId] == _claimant;
  }

  /** PUBLIC FUNCTIONS **/
  function approve(address _to, uint256 _tokenId) external onlyTokenOwner(_tokenId) {
    // Register the approval (replacing any previous approval).
    _approve(msg.sender, _to, _tokenId);
  }

  function transfer(address _to, uint256 _tokenId) external onlyTokenOwner(_tokenId) {
    require(_to != address(0));
    require(_to != address(this));

    _transfer(msg.sender, _to, _tokenId);
  }

  function transferFrom(address _from, address _to, uint256 _tokenId) external {
    require(_to != address(0));
    require(_to != address(this));

    // Check for approval and valid ownership
    require(_approvedFor(msg.sender, _tokenId));
    require(_from == cardIdToOwner[_tokenId]);

    // Reassign ownership (also clears pending approvals and emits Transfer event).
    _transfer(_from, _to, _tokenId);
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
      uint128 power,
      string name
    )
  {
    require(_templateId < templates.length);

    Template storage template = templates[_templateId];

    generation = template.generation;
    power = template.power;
    name = template.name;
  }

  function getCard(uint256 _cardId)
    external
    view
    returns (
      uint128 generation,
      uint128 power,
      string name
    )
  {
    require(_cardId < cards.length);

    uint256 templateId = cards[_cardId];
    Template storage template = templates[templateId];

    generation = template.generation;
    power = template.power;
    name = template.name;
  }

  function ownerOf(uint256 _tokenId) external view returns (address owner) {
    owner = cardIdToOwner[_tokenId];
    require(owner != address(0));
  }

  function balanceOf(address _owner) public view returns (uint256 count) {
    return ownerCardCount[_owner];
  }

  function templatesSupply() public view returns (uint256 count) {
    return templates.length;
  }

  function instancesSupply() public view returns (uint256 count) {
    return cards.length;
  }

  function instanceLimit(uint256 _templateId) public view returns(uint256) {
    return templateIdToMintLimit[_templateId];
  }

  function name() public pure returns (string) {
    return "TCG";
  }

  function symbol() public pure returns (string) {
    return "TCG";
  }

  function tokensOfOwner(address _owner) external view returns (uint256[] tokenIds) {
    uint256 tokenCount = balanceOf(_owner);

    if (tokenCount == 0) {
      return new uint256[](0);
    } else {
      uint256[] memory result = new uint256[](tokenCount);
      uint256 resultIndex = 0;

      uint256 cardId;

      for (cardId = 0; cardId < cards.length; cardId++) {
        if (cardIdToOwner[cardId] == _owner) {
          result[resultIndex] = cardId;
          resultIndex++;
        }
      }

      return result;
    }
  }
}
