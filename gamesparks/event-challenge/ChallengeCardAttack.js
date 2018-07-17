// ====================================================================================================
//
// Challenge event for when player uses a card on its field to attack a card on opponent's field
// or the opponent's "face" directly. Without any special effects, in the card vs card situation
// the attacking card takes X damage and the defending card takes Y damage - where X is the
// defending card's attack and Y is the attacking card's attack. Any card whose health drop to
// less than equal to zero is taken off the field. Without any special effects, in the card vs
// face situation the opponent's health is decreased by the attacking card's attack and the
// attacking card is unaffected.
//
// Attributes:
// - targetId: card ID of card to attack or TARGET_ID_FACE
// - fieldId: player's ID => player's field (friendly fire), opponent's ID => opponent's field
//
// ====================================================================================================
require("ScriptDataModule");
require("ChallengeEventPrefix");
require("ChallengeCardAttackCardModule");

const cardId = Spark.getData().cardId;
const attributesJson = Spark.getData().attributesJson;
const attributesString = Spark.getData().attributesString;

var attributes;

if (attributesJson.targetId) {
    attributes = attributesJson;
} else {
    attributes = JSON.parse(attributesString);
    if (!attributes.targetId) {
        setScriptError("Invalid attributesJson or attributesString parameter");
    }
}

handleChallengeCardAttackCard(challengeStateData, playerId, cardId, attributes);

require("PersistChallengeStateModule");

setScriptSuccess();
