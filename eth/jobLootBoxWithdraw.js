var Web3 = require("web3-js").Web3;
var EthTx = require("ethereumjs-tx");

var contractAddress = "0xbca850e75f5e5dc02Dcb345461bE392af906F7C3";
var contractAbi = [{"constant":true,"inputs":[],"name":"getBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"boughtCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_category","type":"uint256"}],"name":"priceByCategory","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_owner","type":"address"},{"name":"_referrer","type":"address"},{"name":"_category","type":"uint256"},{"name":"_quantity","type":"uint256"}],"name":"buyBoxes","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":false,"inputs":[],"name":"withdraw","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[],"name":"unpause","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"blocksPerDay","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"paused","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"renounceOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"blockStart","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"pause","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"owner","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"basePrice","type":"uint256"},{"name":"boxCount","type":"uint256"}],"name":"calculatePrice","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"newOwner","type":"address"}],"name":"transferOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"inputs":[],"payable":false,"stateMutability":"nonpayable","type":"constructor"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_owner","type":"address"},{"indexed":true,"name":"_referrer","type":"address"},{"indexed":true,"name":"_category","type":"uint256"},{"indexed":false,"name":"_quantity","type":"uint256"}],"name":"BoxesBought","type":"event"},{"anonymous":false,"inputs":[],"name":"Pause","type":"event"},{"anonymous":false,"inputs":[],"name":"Unpause","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"}],"name":"OwnershipRenounced","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"},{"indexed":true,"name":"newOwner","type":"address"}],"name":"OwnershipTransferred","type":"event"}];

var ownerAddress = "0x2afaf114b6833b951a00610cededd3d7c7a48ae8";
var ownerPrivateKey = Buffer.from(
  "763f4cac19b185ef4d08bea734541655bc38c043f179a177ae83bda945b86a16",
  "hex"
);

var web3 = new Web3(new Web3.providers.HttpProvider(
  "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8"
));

web3.eth.getTransactionCount(ownerAddress, function (err, nonce) {
  var data = new web3.eth.Contract(contractAbi, contractAddress)
    .methods.withdraw().encodeABI();

  var txParams = {
    nonce: web3.utils.toHex(nonce),
    gasPrice: web3.utils.toHex(web3.utils.toWei('2', 'gwei')),
    gasLimit: 100000,
    to: contractAddress,
    value: 0,
    data: data,
    chainId: 4,
  };

  var tx = new EthTx(txParams);
  tx.sign(ownerPrivateKey);

  var rawTx = '0x' + tx.serialize().toString('hex');
  console.log(rawTx);
  web3.eth.sendSignedTransaction(rawTx, function (err, transactionHash) {
    console.log(transactionHash);
  });
});


