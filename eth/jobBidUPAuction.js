var Web3 = require("web3-js").Web3;
var EthTx = require("ethereumjs-tx");

var contractAddress = "0x634F3Cf671cC40E1d3B0d2d748d564Ed5Ddf0b7F";
var contractAbi = [{"constant":true,"inputs":[],"name":"getName","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBlockStart","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_index","type":"uint256"}],"name":"refundBid","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_name","type":"string"},{"name":"_N","type":"uint256"},{"name":"_duration","type":"uint256"},{"name":"_minimumBid","type":"uint256"},{"name":"_minimumIncrease","type":"uint256"}],"name":"initializeAuction","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"indicesOfThreeLowestBids","outputs":[{"name":"","type":"uint256"},{"name":"","type":"uint256"},{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_index","type":"uint256"}],"name":"bidByIndex","outputs":[{"name":"","type":"uint256"},{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"renounceOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_index","type":"uint256"}],"name":"submitBid","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[],"name":"owner","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"highestBidAmount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_index","type":"uint256"}],"name":"updateFulfillPrice","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getDuration","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_index","type":"uint256"}],"name":"fulfillBid","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getFulfillPrice","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"newOwner","type":"address"}],"name":"transferOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"}],"name":"OwnershipRenounced","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"previousOwner","type":"address"},{"indexed":true,"name":"newOwner","type":"address"}],"name":"OwnershipTransferred","type":"event"}];
var ownerAddress = "0x2afaf114b6833b951a00610cededd3d7c7a48ae8";
var ownerPrivateKey = Buffer.from(
  "763f4cac19b185ef4d08bea734541655bc38c043f179a177ae83bda945b86a16",
  "hex"
);

var buyerAddress = "0xe598350d4E3c223fCa03bbA55fb9C276576fA4ae";
var buyerPrivateKey = Buffer.from(
  "10eb93083b4f2bd8ce2fbbee18676c6db1ebf778ddaf8cf7255237afe8d61c60",
  "hex"
);

var web3 = new Web3(new Web3.providers.HttpProvider(
  "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8"
));

web3.eth.getTransactionCount(ownerAddress, function (err, nonce) {
  var data = new web3.eth.Contract(contractAbi, contractAddress)
    .methods.submitBid("0").encodeABI();

  var txParams = {
    nonce: web3.utils.toHex(nonce),
    gasPrice: web3.utils.toHex(web3.utils.toWei('2', 'gwei')),
    gasLimit: 1000000,
    to: contractAddress,
    value: web3.utils.toHex(web3.utils.toWei("250", "finney")),
    data: data,
    chainId: 4,
  };

  var tx = new EthTx(txParams);
  tx.sign(ownerPrivateKey);

  var rawTx = '0x' + tx.serialize().toString('hex');
  console.log(rawTx);
  web3.eth.sendSignedTransaction(rawTx, function (err, transactionHash) {
    console.log(transactionHash);
    console.log(err);
  });
});

// web3.eth.getTransactionCount(buyerAddress, function (err, nonce) {
//   var data = new web3.eth.Contract(contractAbi, contractAddress)
//     .methods.submitBid("4").encodeABI();

//   var txParams = {
//     nonce: web3.utils.toHex(nonce),
//     gasPrice: web3.utils.toHex(web3.utils.toWei('2', 'gwei')),
//     gasLimit: 100000,
//     to: contractAddress,
//     value: web3.utils.toHex(web3.utils.toWei("1234", "finney")),
//     data: data,
//     chainId: 4,
//   };

//   var tx = new EthTx(txParams);
//   tx.sign(buyerPrivateKey);

//   var rawTx = '0x' + tx.serialize().toString('hex');
//   console.log(rawTx);
//   web3.eth.sendSignedTransaction(rawTx, function (err, transactionHash) {
//     console.log(transactionHash);
//     console.log(err);
//   });
// });

