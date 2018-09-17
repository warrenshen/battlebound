// ====================================================================================================
//
// Cloud Code for CampaignBeginLevel, write your code here to customize the GameSparks platform.
//
// For details of the GameSparks Cloud Code API see https://docs.gamesparks.com/
//
// ====================================================================================================
require("ScriptDataModule");
require("DeckModule");
require("InitializeChallengeModule");
require("ChallengeStateModule");

// Player is always the challenger for single-player campaign.
const player = Spark.getPlayer();
const playerId = player.getPlayerId();

const challengerId = playerId;
const challengerName = player.getDisplayName();

const challengedId = "CAMPAIGN-BOT";
const challengedName = challengedId;

const challengeId = "CAMPAIGN";

// Get challenger player deck for battle.
const challengerDeck = getActiveDeckByPlayerId(challengerId);
// Get campaign bot deck for battle.
const challengedDeck = getChallengeDeckByPlayerIdAndCards(
    challengedId,
    [
        { id: "C0", templateId: "C0", level: 1 },
        { id: "C0", templateId: "C0", level: 1 },
        { id: "C0", templateId: "C1", level: 1 },
        { id: "C0", templateId: "C1", level: 1 },
        { id: "C0", templateId: "C11", level: 1 },
        { id: "C0", templateId: "C11", level: 1 },
        { id: "C0", templateId: "C12", level: 1 },
        { id: "C0", templateId: "C12", level: 1 },
        { id: "C0", templateId: "C14", level: 1 },
        { id: "C0", templateId: "C14", level: 1 },
        { id: "C0", templateId: "C17", level: 1 },
        { id: "C0", templateId: "C17", level: 1 },
        { id: "C0", templateId: "C2", level: 1 },
        { id: "C0", templateId: "C2", level: 1 },
        { id: "C0", templateId: "C29", level: 1 },
        { id: "C0", templateId: "C29", level: 1 },
        { id: "C0", templateId: "C3", level: 1 },
        { id: "C0", templateId: "C3", level: 1 },
        { id: "C0", templateId: "C30", level: 1 },
        { id: "C0", templateId: "C30", level: 1 },
        { id: "C0", templateId: "C31", level: 1 },
        { id: "C0", templateId: "C31", level: 1 },
        { id: "C0", templateId: "C32", level: 1 },
        { id: "C0", templateId: "C32", level: 1 },
        { id: "C0", templateId: "C34", level: 1 },
        { id: "C0", templateId: "C34", level: 1 },
        { id: "C0", templateId: "C36", level: 1 },
        { id: "C0", templateId: "C36", level: 1 },
        { id: "C0", templateId: "C38", level: 1 },
        { id: "C0", templateId: "C38", level: 1 },
    ]
);

// const firstTurnId = Math.floor(Math.random() * 2) < 1 ? challengerId : challengedId;
const firstTurnId = challengerId;

const challengeStateData = initializeChallengeStateData(
    challengeId,
    challengerId,
    challengerName,
    challengerDeck,
    challengedId,
    challengedName,
    challengedDeck,
    firstTurnId
);
challengeStateData.nonce = 0;

const API = Spark.getGameDataService();
var dexDataItem = API.getItem("Dex", playerId).document();
if (dexDataItem === null) {
    dexDataItem = API.createItem("Dex", playerId);
}

const dexData = dexDataItem.getData();

dexData.campaign = challengeStateData;

const error = dexDataItem.persistor().persist().error();
if (error) {
    setScriptError(error);
}

setChallengeStateForPlayer(challengerId, challengeStateData);
setScriptSuccess();
