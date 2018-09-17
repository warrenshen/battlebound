// ====================================================================================================
//
// Cloud Code for MintCardsModule, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
// const TEMPLATE_ID_TO_LIMIT = {
//     0: 88888,
//     1: 88888,
//     2: 88888,
//     3: 88888,
//     4: 88888,
//     5: 88888,
//     6: 88888,
//     7: 88888,
//     8: 88888,
//     9: 88888,
//     10: 88888,
//     11: 88888,
//     12: 88888,
//     13: 88888,
//     14: 88888,
//     15: 88888,
//     16: 88888,
//     17: 88888,
//     18: 88888,
//     19: 88888,
//     20: 88888,
//     21: 88888,
//     22: 88888,
//     23: 88888,
//     24: 88888,
//     25: 88888,
//     26: 88888,
//     27: 88888,
//     28: 88888,
//     29: 88888,
//     30: 88888,
//     31: 88888,
//     32: 88888,
//     33: 88888,
//     34: 88888,
//     35: 14632,
//     36: 14632,
//     37: 14632,
//     38: 14632,
//     39: 14632,
//     40: 14632,
//     41: 14632,
//     42: 14632,
//     43: 14632,
//     44: 14632,
//     45: 14632,
//     46: 39905,
//     47: 39905,
//     48: 39905,
//     49: 39905,
//     50: 39905,
//     51: 39905,
//     52: 39905,
//     53: 39905,
//     54: 39905,
//     55: 39905,
//     56: 39905,
//     57: 39905,
//     58: 39905,
//     59: 39905,
//     60: 39905,
//     61: 39905,
//     62: 39905,
//     63: 39905,
//     64: 39905,
//     65: 39905,
//     66: 39905,
//     67: 39905,
//     68: 3737,
//     69: 3737,
//     70: 30,
// };
const TEMPLATE_ID_TO_LIMIT = {
    0: 0,
    1: 0,
    2: 100,
    3: 1000,
    4: 1000,
    5: 10,
    6: 2,
    7: 2,
    8: 2,
    9: 2,
    10: 2,
};
const TEMPLATE_IDS_EPIC = [
    3,
];
const TEMPLATE_IDS_LEGENDARY = [
    4,
];
const TEMPLATE_IDS_COSMIC = [
    5,
];
const TEMPLATE_IDS_EPIC_OR_BETTER = TEMPLATE_IDS_EPIC.concat(TEMPLATE_IDS_LEGENDARY.concat(TEMPLATE_IDS_COSMIC));
const TEMPLATE_IDS_LEGENDARY_OR_BETTER = TEMPLATE_IDS_LEGENDARY.concat(TEMPLATE_IDS_COSMIC);
const TEMPLATE_IDS_GOLDEN = [
    6,
    7,
    8,
    9,
    10,
];

function _mintRandomCard(category, templateIdToCount) {
    var totalCardsRemaining = 0;
    var templateIds;
    if (category === 1) {
        templateIds = TEMPLATE_IDS_EPIC_OR_BETTER;
    } else if (category === 2) {
        templateIds = TEMPLATE_IDS_LEGENDARY_OR_BETTER;
    } else if (category === 3) {
        templateIds = TEMPLATE_IDS_GOLDEN;
    } else {
        templateIds = Object.keys(TEMPLATE_ID_TO_LIMIT);
    }
    
    templateIds.sort(function(a, b) {
        return a < b ? -1 : 1;
    });
    templateIds.forEach(function(templateId) {
        var templateCardsCount = templateIdToCount[templateId];
        if (templateCardsCount == null) {
            templateCardsCount = 0;
            templateIdToCount[templateId] = 0;
        }
        const templateCardsLeft = TEMPLATE_ID_TO_LIMIT[templateId] - templateCardsCount;
        totalCardsRemaining += templateCardsLeft;
    });
    
    const randomIndex = Math.floor(Math.random() * totalCardsRemaining);
    var mintTemplateId = null;
    var currentIndex = 0;
    for (var i = 0; i < templateIds.length; i += 1) {
        var templateId = templateIds[i];
        var templateCardsCount = templateIdToCount[templateId];
        var templateCardsLeft = TEMPLATE_ID_TO_LIMIT[templateId] - templateCardsCount;
        if (templateCardsLeft <= 0) {
            continue;
        } else if (randomIndex < currentIndex + templateCardsLeft) {
            mintTemplateId = templateId;
            templateIdToCount[templateId] = templateIdToCount[templateId] + 1;
            break;
        }
        currentIndex += templateCardsLeft;
    }
    
    if (mintTemplateId != null) {
        return parseInt(mintTemplateId);
    } else {
        return null;
    }
}

function mintRandomCards(mintCount, category, templateIdToCount) {
    const templateIds = [];
    for (var i = 0; i < mintCount; i += 1) {
        var newTemplateId;
        if (category === 3 || i % 5 === 0) {
            newTemplateId = _mintRandomCard(category, templateIdToCount);
        } else {
            newTemplateId = _mintRandomCard(0, templateIdToCount);
        }
        if (newTemplateId != null) {
            templateIds.push(newTemplateId);
        }
    }
    // const variations = templateIds.map(function(templateId) {
    //     if (category === 3) {
    //         return 1;
    //     } else {
    //         const randomIndex = Math.floor(Math.random() * 100);
    //         return randomIndex >= 99 ? 1 : 0;
    //     }
    // });
    // return [templateIds, variations];
    return templateIds;
}

function _computeGasLimit(mintCount) {
    if (mintCount <= 1) {
        return 100000; // 91431
    } else if (mintCount <= 25) {
        return 1800000; // 1739780
    } else if (mintCount <= 50) {
        return 3400000; // 3364777
    } else if (mintCount <= 100) {
        return 6800000; // 6704952
    } else {
        Spark.getLog().error({
            errorMessage: "Compute gas limit called on count > 100.",
            mintCount: mintCount,
        });
        return null;
    }
}

function fetchSignedTransaction(txElement) {
    const nonce = fetchNonceByAddress(MINTER_ADDRESS);
    if (txElement.type === "TRANSACTION_TYPE_LOOT_BOX") {
        const category = txElement.data.category;
        const quantity = txElement.data.quantity;
        
        const mintCount = quantity * 5;
        const templateIds = mintRandomCards(mintCount, category, templateIdToCount);
        // const templateIds = mintResponse[0];
        // const variations = mintResponse[1];
        
        if (templateIds.length <= 0 || templateIds.length != variations.length) {
            // Create signed transaction failed - log error.
            Spark.getLog().error({
                errorMessage: "Mint random cards fail.",
                templateIds: templateIds,
                variations: variations,
            });
            return null;
        }
        
        var json;
        const gasLimit = _computeGasLimit(templateIds.length);
        
        if (Spark.getConfig().getStage() === "live") {
            json = {
                nonce: nonce,
                recipientAddress: txElement.data.recipientAddress,
                gasPrice: 2,
                gasLimit: gasLimit,
                chainId: 4,
                contractAddress: "",
                privateKey: "",
                templateIds: templateIds,
                variations: variations,
            };
        } else {
            json = {
                nonce: nonce,
                recipientAddress: txElement.data.recipientAddress,
                gasPrice: 2,
                gasLimit: gasLimit,
                chainId: 4,
                contractAddress: "0x948d395aA9Bafb8C819F9A5EC59f36b8E92E375B",
                privateKey: MINTER_PRIVATE_KEY,
                templateIds: templateIds,
                variations: variations,
            };
        }
        
        const jsonString = JSON.stringify(json);
        const response = Spark.getHttp("https://lwiyu7lla4.execute-api.us-west-2.amazonaws.com/Beta/sign-mint-cards").postString(jsonString);
        const responseCode = response.getResponseCode();
        const responseJson = response.getResponseJson();
        
        if (responseJson == null) {
            // Create signed transaction failed - log error.
            Spark.getLog().error({
                errorMessage: "Sign transaction with Lambda failed: no response.",
                txElement: txElement,
                json: json,
            });
            return null;
        }
        
        const rawTx = responseJson.rawTransaction;
        const error = responseJson.error;
        if (rawTx) {
            return responseJson.rawTransaction;
        } else {
            // Create signed transaction failed - log error.
            Spark.getLog().error({
                errorMessage: "Sign transaction with Lambda failed: " + error,
                txElement: txElement,
                json: json,
            });
            return null;
        }
    } else {
        // Create signed transaction failed - log error.
        Spark.getLog().error({
            errorMessage: "Unsupported txElement type: " + txElement.type,
            txElement: txElement,
        });
        return null;
    }
}
