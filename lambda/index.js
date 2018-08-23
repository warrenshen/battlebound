exports.handler = async (event, context, callback) => {
  const nonce = parseInt(event.nonce);
  const gasPriceInGWei = parseInt(event.gasPrice);
  const gasLimit = parseInt(event.gasLimit);
  const chainId = parseInt(event.chainId);
  const contractAddress = event.contractAddress;
  const privateKey = event.privateKey;

  let error = null;

  if (!nonce) {
    error = "ERROR: invalid nonce param: " + nonce;
  } else if (!gasPriceInGWei || !gasLimit) {
    error = "ERROR: gasPrice or gasLimit param";
  } else if (gasPriceInGWei > 10) {
    error = "ERROR: gasPrice param should not be greater than 10";
  } else if (!chainId ||chainId != 4 && chainId != 1) {
    error = "ERROR: invalid chainId param";
  } else if (!contractAddress || contractAddress.indexOf("0x") != 0 || contractAddress.length != 42) {
    error = "ERROR: invalid contractAddress param";
  } else if (!privateKey || privateKey.indexOf("0x") != 0 || privateKey.length != 66) {
    error = "ERROR: invalid privateKey param";
  }

  if (error) {
    var response = {
      "statusCode": 400,
      "body": JSON.stringify({ "error": error }),
      "isBase64Encoded": false
    };

    callback(null, response);
    return;
  }

  const templateIds = JSON.parse(event.templateIds);
  const variations = JSON.parse(event.variations);
  const recipientAddress = event.recipientAddress;

  if (!Array.isArray(templateIds) || !Array.isArray(variations)) {
    error = "ERROR: invalid templateIds or variations param";
  } else if (templateIds.length != variations.length) {
    error = "ERROR: params templateIds and variations must have same length";
  } else if (!recipientAddress || recipientAddress.indexOf("0x") != 0 || recipientAddress.length != 42) {
    error = "ERROR: invalid recipientAddress param";
  }

  if (error) {
    var response = {
      "statusCode": 400,
      "body": JSON.stringify({ "error": error }),
      "isBase64Encoded": false
    };

    callback(null, response);
    return;
  }

  const Web3 = require("web3-js").Web3;
  const web3 = new Web3();
  // const contractAddress = "0x948d395aA9Bafb8C819F9A5EC59f36b8E92E375B";
  const contractAbi = [{"constant":true,"inputs":[{"name":"_interfaceID","type":"bytes4"}],"name":"supportsInterface","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"name","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":true,"inputs":[],"name":"minter","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_tokenId","type":"uint256"}],"name":"getApproved","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"}],"name":"approve","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"totalSupply","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"templateSupply","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_from","type":"address"},{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"}],"name":"transferFrom","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_templateId","type":"uint256"}],"name":"getTemplate","outputs":[{"name":"generation","type":"uint128"},{"name":"category","type":"uint64"},{"name":"power","type":"uint64"},{"name":"name","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_templateIds","type":"uint256[]"},{"name":"_variations","type":"uint256[]"},{"name":"_owner","type":"address"}],"name":"mintCards","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_tokenId","type":"uint256"},{"name":"_startingPrice","type":"uint256"},{"name":"_endingPrice","type":"uint256"},{"name":"_duration","type":"uint256"}],"name":"createSaleAuction","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_from","type":"address"},{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"}],"name":"safeTransferFrom","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_tokenId","type":"uint256"}],"name":"exists","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_templateId","type":"uint256"}],"name":"instanceLimit","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_mintLimit","type":"uint256"},{"name":"_generation","type":"uint128"},{"name":"_category","type":"uint64"},{"name":"_power","type":"uint64"},{"name":"_name","type":"string"}],"name":"mintTemplate","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_tokenId","type":"uint256"}],"name":"ownerOf","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"}],"name":"balanceOf","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"renounceOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"}],"name":"tokensOfOwner","outputs":[{"name":"","type":"uint256[]"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"owner","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_cardId","type":"uint256"}],"name":"getCard","outputs":[{"name":"generation","type":"uint128"},{"name":"category","type":"uint64"},{"name":"power","type":"uint64"},{"name":"name","type":"string"},{"name":"variation","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"symbol","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":false,"inputs":[{"name":"_operator","type":"address"},{"name":"_approved","type":"bool"}],"name":"setApprovalForAll","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_address","type":"address"}],"name":"setSaleAuction","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_from","type":"address"},{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"},{"name":"_data","type":"bytes"}],"name":"safeTransferFrom","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_templateId","type":"uint256"},{"name":"_variation","type":"uint256"},{"name":"_owner","type":"address"}],"name":"mintCard","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_cardId","type":"uint256"}],"name":"templateIdOf","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"saleAuction","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"},{"name":"_operator","type":"address"}],"name":"isApprovedForAll","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"newOwner","type":"address"}],"name":"transferOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_minter","type":"address"}],"name":"setMinter","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"anonymous":false,"inputs":[{"indexed":false,"name":"_templateId","type":"uint256"}],"name":"TemplateMinted","type":"event"},{"anonymous":false,"inputs":[{"indexed":false,"name":"_owner","type":"address"},{"indexed":false,"name":"_cardId","type":"uint256"}],"name":"CardMinted","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_from","type":"address"},{"indexed":true,"name":"_to","type":"address"},{"indexed":true,"name":"_tokenId","type":"uint256"}],"name":"Transfer","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_owner","type":"address"},{"indexed":true,"name":"_approved","type":"address"},{"indexed":true,"name":"_tokenId","type":"uint256"}],"name":"Approval","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_owner","type":"address"},{"indexed":true,"name":"_operator","type":"address"},{"indexed":false,"name":"_approved","type":"bool"}],"name":"ApprovalForAll","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"}],"name":"OwnershipRenounced","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"},{"indexed":true,"name":"newOwner","type":"address"}],"name":"OwnershipTransferred","type":"event"}];

  const data = new web3.eth.Contract(contractAbi, contractAddress)
    .methods
    .mintCards(
      templateIds,
      variations,
      recipientAddress
    )
    .encodeABI();

  const txParams = {
    nonce: web3.utils.toHex(nonce),
    gasPrice: web3.utils.toHex(web3.utils.toWei(gasPriceInGWei.toString(), 'gwei')),
    gasLimit: web3.utils.toHex(gasLimit),
    to: contractAddress,
    value: web3.utils.toHex(0),
    data: data,
    chainId: chainId,
  };

  const account = web3.eth.accounts.privateKeyToAccount(privateKey);
  const signedTx = await account.signTransaction(txParams)

  const responseBody = {
    "rawTransaction": signedTx.rawTransaction,
  };

  var response = {
    "statusCode": 200,
    "body": JSON.stringify(responseBody),
    "isBase64Encoded": false
  };

  callback(null, response);
  return;
};
