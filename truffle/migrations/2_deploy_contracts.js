var CardTreasury = artifacts.require("./CardTreasury.sol");

module.exports = function(deployer) {
  deployer.deploy(CardTreasury, { gas: 4712388 });
};
