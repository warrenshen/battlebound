exports.handler = async (event) => {
    var nonce = event.nonce;
    console.log('nonce is ', event.nonce);
    console.log('templateId is ', event.templateId);
    console.log('owner address is ', event.ownerAddress);
    if (!nonce) {
        return 'Nonce param is not set';
    }

    var Web3 = require("web3-js").Web3;
    var EthTx = require("ethereumjs-tx");

    var contractAddress = "0x5Cd6BaEF2B5bd80CD699A702649591DC1fE6bf33";
    var contractAbi = [{"constant":true,"inputs":[],"name":"name","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":false,"inputs":[{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"}],"name":"approve","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"instancesSupply","outputs":[{"name":"count","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_from","type":"address"},{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"}],"name":"transferFrom","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_templateId","type":"uint256"}],"name":"getTemplate","outputs":[{"name":"generation","type":"uint128"},{"name":"power","type":"uint128"},{"name":"name","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"templatesSupply","outputs":[{"name":"count","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_templateId","type":"uint256"}],"name":"instanceLimit","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_tokenId","type":"uint256"}],"name":"ownerOf","outputs":[{"name":"owner","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"}],"name":"balanceOf","outputs":[{"name":"count","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"renounceOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"}],"name":"tokensOfOwner","outputs":[{"name":"tokenIds","type":"uint256[]"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"owner","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_cardId","type":"uint256"}],"name":"getCard","outputs":[{"name":"generation","type":"uint128"},{"name":"power","type":"uint128"},{"name":"name","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"symbol","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":false,"inputs":[{"name":"_to","type":"address"},{"name":"_tokenId","type":"uint256"}],"name":"transfer","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_templateId","type":"uint256"},{"name":"_owner","type":"address"}],"name":"mintCard","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_mintLimit","type":"uint256"},{"name":"_generation","type":"uint128"},{"name":"_power","type":"uint128"},{"name":"_name","type":"string"}],"name":"createTemplate","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"newOwner","type":"address"}],"name":"transferOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"anonymous":false,"inputs":[{"indexed":false,"name":"templateId","type":"uint256"}],"name":"TemplateCreated","type":"event"},{"anonymous":false,"inputs":[{"indexed":false,"name":"owner","type":"address"},{"indexed":false,"name":"cardId","type":"uint256"},{"indexed":false,"name":"templateId","type":"uint256"}],"name":"InstanceMinted","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_from","type":"address"},{"indexed":true,"name":"_to","type":"address"},{"indexed":false,"name":"_tokenId","type":"uint256"}],"name":"Transfer","type":"event"},{"anonymous":false,"inputs":[{"indexed":false,"name":"_owner","type":"address"},{"indexed":false,"name":"_approved","type":"address"},{"indexed":false,"name":"_tokenId","type":"uint256"}],"name":"Approval","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"}],"name":"OwnershipRenounced","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"},{"indexed":true,"name":"newOwner","type":"address"}],"name":"OwnershipTransferred","type":"event"}];

    var ownerAddress = "0x2afaf114b6833b951a00610cededd3d7c7a48ae8";
    var ownerPrivateKey = Buffer.from(
      "763f4cac19b185ef4d08bea734541655bc38c043f179a177ae83bda945b86a16",
      "hex"
    );

    var web3 = new Web3(new Web3.providers.HttpProvider(
      "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8"
    ));

    var data = new web3.eth.Contract(contractAbi, contractAddress)
          .methods
          .mintCard(0, "0x2aFAf114B6833B951A00610cEdeDD3D7c7A48AE8")
          .encodeABI();

    var txParams = {
      nonce: web3.utils.toHex(nonce),
      gasPrice: web3.utils.toHex(web3.utils.toWei('5', 'gwei')),
      gasLimit: 1000000,
      to: contractAddress,
      value: 0,
      data: data,
      chainId: 4,
    };

    var tx = new EthTx(txParams);
    tx.sign(ownerPrivateKey);

    var rawTx = '0x' + tx.serialize().toString('hex');
    return 'This is cool, hello, this is Bob!';

};
