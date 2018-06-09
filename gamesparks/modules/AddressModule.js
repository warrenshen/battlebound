// ====================================================================================================
//
// Cloud Code for AddressModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
function cleanHex(hexInput) {
    if (hexInput.indexOf("0x") === 0) {
        return hexInput.substring(2);
    } else {
        return hexInput;
    }
}

/**
 * Given an address input like "0x2afaf114b6833b951a00610cededd3d7c7a48ae8",
 * validates length and returns "2afaf114b6833b951a00610cededd3d7c7a48ae8".
 * This is meant to be used when generating payload for Infura JSON RPC calls.
 * 
 * @return string
 **/
function cleanAddress(address) {
    address = cleanHex(address);
    if (address.length > 40) {
        return "BAD_ADDRESS";
    } else {
        return address;
    }
}

// Converts hex input of fixed length to int(s) - assume input is composed of 32-byte words.
function convertHexToIntFixedLength(hexInput) {
    hexInput = cleanHex(hexInput);
    
    if (hexInput.length === 64) {
        return parseInt(hexInput, 16);
    } else {
        const result = [];
        for (var i = 0; i < hexInput.length / 64; i += 1) {
            result.push(parseInt(hexInput.substring(i * 64, (i + 1) * 64), 16));
        }
        return result;
    }
}

// Converts hex input of variable length to array of ints - assume input is composed of 32-byte words.
function convertHexToIntVariableLength(hexInput) {
    hexInput = cleanHex(hexInput);
    const result = [];
    // The = 2 is a hack that skip the first 2 words (word length and result count) for uint256[] responses.
    for (var i = 2; i < hexInput.length / 64; i += 1) {
        result.push(parseInt(hexInput.substring(i * 64, (i + 1) * 64), 16));
    }
    return result;
}

/**
 * Given an input like "0x0000000000000000000000002afaf114b6833b951a00610cededd3d7c7a48ae8",
 * returns "0x2afaf114b6833b951a00610cededd3d7c7a48ae8".
 * 
 * @return string
 **/
function parseAddress(rawAddress) {
    const prefixedAddress = cleanHex(rawAddress);
    return "0x" + prefixedAddress.substring(prefixedAddress.length - 40);
}

/**
 * Converts given param to a hex string
 * and pads it to be a 32-bit word.
 * 
 * @param param - int
 * @return string
 */
function padParameter(param) {
    const paramString = param.toString(16);
    var leftPadding = "";
    for (var i = 0; i < 64 - paramString.length; i += 1) {
        leftPadding += "0";
    }
    return leftPadding + paramString;
}

/**
 * Generates a new address challenge (64 characters 0-9)
 * and updates the current player's privateData with it.
 * Returns the new address challenge.
 **/
function resetPlayerAddressChallenge() {
    const player = Spark.getPlayer();
    
    var addressChallenge = "";
    for (var i = 0; i < 64; i += 1) {
        var randomInt = Math.floor(Math.random() * 10);
        addressChallenge += randomInt.toString();
    }
    
    player.setPrivateData("addressChallenge", addressChallenge);
    return addressChallenge;
}