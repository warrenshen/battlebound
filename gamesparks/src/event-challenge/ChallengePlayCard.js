// ====================================================================================================
//
// Challenge event for when player uses a card in its hand. This can be to move
// a card from the player's hand onto the field or to use the spell power of a
// card from the player's hand (which discards the card afterwards).
//
// Card categories:
// - 0: minion (play on field)
// - 1: spell (use and discard)
// - 2: structure (play on field)
// - 3: weapon (use and discard)
//
// Security concerns:
// - Is the Card played valid (attributes are not changed)?
//   The Card is stored on server-side and cannot be tampered with.
// - Does the player own the Card played?
//   It could only be in the player's hand (stored server side) if so.
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("ChallengePlayCardModule");

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

var attributes;

if (attributesJson.fieldIndex != null) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (attributes.fieldIndex == null) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

handleChallengePlayCard(challengeStateData, playerId, cardId, attributes);
    
require("PersistChallengeStateModule");

setScriptSuccess();
