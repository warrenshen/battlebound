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
    
    if (hexInput.length === 64) {
        return parseInt(hexInput, 16);
    } else {
        const result = [];
        // The = 2 is a hack that skip the first 2 words (word length and result count) for uint256[] responses.
        for (var i = 2; i < hexInput.length / 64; i += 1) {
            result.push(parseInt(hexInput.substring(i * 64, (i + 1) * 64), 16));
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