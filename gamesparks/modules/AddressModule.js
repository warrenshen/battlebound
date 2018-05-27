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

function cleanAddress(address) {
    address = cleanHex(address);
    
    if (address.length > 40) {
        return "BAD_ADDRESS";
    } else {
        return address;
    }
}

// Converts hex input to int - assume input is composed of 32-byte words.
function convertHexToInt(hexInput) {
    hexInput = cleanHex(hexInput);
    
    if (hexInput.length === 32) {
        return parseInt(hexInput, 16);
    } else {
        var result = [];
        // The = 5 and the += 2 are hacks that only work for uint256[] responses.
        for (var i = 5; i < hexInput.length / 32; i += 2) {
            result.push(parseInt(hexInput.substring(i * 32, (i + 1) * 32), 16));
        }
        return result;
    }
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