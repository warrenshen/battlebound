// ====================================================================================================
//
// Cloud Code for LOG_EVENT_REQUEST_BALANCE, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
var address = Spark.getData().address;
var url = "https://rinkeby.infura.io/kBLFY7NMU7NrFMdpvDR8";
// url = "https://mainnet.infura.io/kBLFY7NMU7NrFMdpvDR8";

var json = {
  jsonrpc: "2.0",
  method: "eth_getBalance",
  id: 1,
  params: [address, "latest"]
};
var jsonString = JSON.stringify(json);
var response = Spark.getHttp(url).postString(jsonString);

var responseCode = response.getResponseCode();
var payloadJson = response.getResponseJson();

json = {
  jsonrpc: "2.0",
  method: "eth_call",
  id: 1,
  params: [
    {
      "to": "0x06012c8cf97BEaD5deAe237070F9587f8E7A266d",
      "data": "0x6352211e000000000000000000000000000000000000000000000000000000000000000a"
    },
    "latest"
  ]
};
jsonString = JSON.stringify(json);
// response = Spark.getHttp(url).postString(jsonString);
// var responseCodeTwo = response.getResponseCode();
// var payloadJsonTwo = response.getResponseJson();

json = {
  jsonrpc: "2.0",
  method: "eth_sendRawTransaction",
  id: 1,
  params: ["0xf8aa0c85012a05f200830f4240945cd6baef2b5bd80cd699a702649591dc1fe6bf3380b844e933cfb000000000000000000000000000000000000000000000000000000000000000000000000000000000000000002afaf114b6833b951a00610cededd3d7c7a48ae82ba0726b6589e0ea08e591dcdc246527b11d6a5b7a0c384ce485f5233a099549a937a06159b2d70fca58c99f6f2614d68693a311731ac37aa6876470e1d35e91e2e315"]
};
jsonString = JSON.stringify(json);
response = Spark.getHttp(url).postString(jsonString);
var responseCodeTwo = response.getResponseCode();
var payloadJsonTwo = response.getResponseJson();

if (responseCode === 200) {
  Spark.setScriptData("payload", response.getResponseString());
  // Result = balance of address in hex.
  var result = payloadJson.result;
  var balance = parseInt(result, 16);
  Spark.setScriptData("balance", balance.toString());
  
  result = payloadJsonTwo.result;
  Spark.setScriptData("owner", result.toString());
} else {
  Spark.setScriptData("error", payloadJson);
}

// curl -i -X POST \
// -H "Content-Type: application/json" \
// --data '{"jsonrpc":"2.0","method":"eth_getBalance","params":["0xF02c1c8e6114b1Dbe8937a39260b5b0a374432bB","latest"],"id":1}' \
// "https://mainnet.infura.io/kBLFY7NMU7NrFMdpvDR8"