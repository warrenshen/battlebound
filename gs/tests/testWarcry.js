const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge warcry", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("warcry: adjacent and sacrifice health", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_PLAYER-18",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_PLAYER-19",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_PLAYER-20",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [
            {
              "id": "ID_PLAYER-9",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 50,
              "cost": 40,
              "name": "Lil' Rusty",
              "description": "Warcy: Grant Taunt to adjacent creatures",
              "abilities": [
                20
              ],
              "baseId": "C53",
              "attackStart": 20,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
            },
            {
              "id": "ID_PLAYER-22",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Seahorse Squire",
              "description": "Warcry: Heal adjacent creatures by 20 hp",
              "abilities": [
                26
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30
            },
            {
              "id": "ID_PLAYER-23",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 20,
              "cost": 40,
              "name": "Forgemech",
              "description": "Warcry: Give +30/30 to adjacent creatures",
              "abilities": [
                44
              ],
              "baseId": "C41",
              "attackStart": 20,
              "costStart": 40,
              "healthStart": 20,
              "healthMax": 20
            },
            {
              "id": "ID_PLAYER-26",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 30,
              "cost": 20,
              "name": "Ritual Hatchling",
              "description": "Warcry: sacrifice 20 dmg from your life total",
              "abilities": [
                24
              ],
              "baseId": "C36",
              "attackStart": 40,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": [],
        "ID_OPPONENT": [],
      },
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support grant taunt to adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-9");

            assert.equal(playerField[1].id, "ID_PLAYER-18");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[3].id, "ID_PLAYER-19");
            assert.equal(playerField[3].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[5].id, "ID_PLAYER-20");
            assert.equal(playerField[5].abilities.indexOf(1) >= 0, false);

            resolve();
          }
        );
      });
    });

    it("should support grant taunt to adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-9");

            assert.equal(playerField[1].id, "ID_PLAYER-18");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[3].id, "ID_PLAYER-19");
            assert.equal(playerField[3].abilities.indexOf(1) >= 0, false);

            assert.equal(playerField[5].id, "ID_PLAYER-20");
            assert.equal(playerField[5].abilities.indexOf(1) >= 0, false);

            resolve();
          }
        );
      });
    });

    it("should support heal adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-22",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-22");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-22");

            assert.equal(playerField[1].id, "ID_PLAYER-18");
            assert.equal(playerField[1].health, 30);

            assert.equal(playerField[3].id, "ID_PLAYER-19");
            assert.equal(playerField[3].health, 30);

            assert.equal(playerField[5].id, "ID_PLAYER-20");
            assert.equal(playerField[5].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support heal adjacent creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-22",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-22");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-22");

            assert.equal(playerField[1].id, "ID_PLAYER-18");
            assert.equal(playerField[1].health, 30);

            assert.equal(playerField[3].id, "ID_PLAYER-19");
            assert.equal(playerField[3].health, 10);

            assert.equal(playerField[5].id, "ID_PLAYER-20");
            assert.equal(playerField[5].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support buff adjacent creatures +30/30", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-23",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-23");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-23");

            assert.equal(playerField[1].id, "ID_PLAYER-18");
            assert.equal(playerField[1].attack, 50);
            assert.equal(playerField[1].attackStart, 20);
            assert.equal(playerField[1].health, 40);
            assert.equal(playerField[1].healthMax, 60);
            assert.equal(playerField[1].healthStart, 30);
            assert.equal(playerField[1].buffsField.indexOf(1005) >= 0, true);

            assert.equal(playerField[3].id, "ID_PLAYER-19");
            assert.equal(playerField[3].health, 10);

            assert.equal(playerField[5].id, "ID_PLAYER-20");
            assert.equal(playerField[5].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support damage player face", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-26",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-26");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 50);
            assert.equal(playerState.manaMax, 70);
            assert.equal(playerState.health, 80);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-26");

            resolve();
          }
        );
      });
    });
  });

  describe("warcry: in front", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "C44-ID_OPPONENT-18",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [
            {
              "id": "C13-ID_PLAYER-13",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Flamebelcher",
              "description": "Battlecry: Deal 10 damage to any minion in front",
              "abilities": [
                15
              ],
              "baseId": "C13",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 30,
              "healthMax": 30,
            },
            {
              "id": "C51-ID_PLAYER-12",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 10,
              "health": 30,
              "cost": 30,
              "name": "Thief of Night",
              "description": "Warcy: Silence creature in front",
              "abilities": [
                18
              ],
              "baseId": "C51",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": [],
        "ID_OPPONENT": [],
      },
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support attack in front", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C13-ID_PLAYER-13",
            attributesJson: {
              fieldIndex: 5,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].attributes.card.id, "C13-ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldIndex, 5);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[5].id, "C13-ID_PLAYER-13");
            assert.equal(playerField[5].health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support attack in front", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C13-ID_PLAYER-13",
            attributesJson: {
              fieldIndex: 4,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].attributes.card.id, "C13-ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldIndex, 4);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[4].id, "C13-ID_PLAYER-13");
            assert.equal(playerField[4].health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "C44-ID_OPPONENT-18");
            assert.equal(opponentField[0].health, 10);

            resolve();
          }
        );
      });
    });

    it("should support silence in front", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C51-ID_PLAYER-12",
            attributesJson: {
              fieldIndex: 5,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].attributes.card.id, "C51-ID_PLAYER-12");
            assert.equal(lastMoves[0].attributes.fieldIndex, 5);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[5].id, "C51-ID_PLAYER-12");
            assert.equal(playerField[5].health, 30);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "C44-ID_OPPONENT-18");
            assert.equal(opponentField[0].isSilenced, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("warcry: all enemy", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "C12-ID_OPPONENT-8",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "C49-ID_OPPONENT-13",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 10,
              "health": 30,
              "cost": 30,
              "name": "Pricklepillar",
              "description": "Taunt; Lethal",
              "abilities": [
                1,
                21
              ],
              "baseId": "C49",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 6,
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 60,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "C46-ID_PLAYER-16",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 50,
              "health": 40,
              "cost": 50,
              "name": "Cereboarus",
              "description": "Lifesteal; Deathwish: Draw a card",
              "abilities": [
                5,
                6
              ],
              "baseId": "C46",
              "attackStart": 50,
              "costStart": 50,
              "healthStart": 40,
              "healthMax": 40,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 5,
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [
            {
              "id": "C52-ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 60,
              "health": 50,
              "cost": 70,
              "name": "POWER SIPH#NER",
              "description": "Warcy: Silence all opponent creatures",
              "abilities": [
                31
              ],
              "baseId": "C52",
              "attackStart": 60,
              "costStart": 70,
              "healthStart": 50,
              "healthMax": 50,
            },
          ],
          "deckSize": 0,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": [],
        "ID_OPPONENT": [],
      },
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support silence all enemy creatures", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C52-ID_PLAYER-4",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "C52-ID_PLAYER-4");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 0);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "C52-ID_PLAYER-4");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C12-ID_OPPONENT-8");
            assert.equal(opponentField[0].isSilenced, 1);

            assert.equal(opponentField[1].id, "C49-ID_OPPONENT-13");
            assert.equal(opponentField[1].isSilenced, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("warcry: attack random", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 0,
          "manaMax": 70,
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "ID_OPPONENT-8",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Lux",
              "description": "Warcry: Heal 40 hp to all creatures on board",
              "abilities": [
                28
              ],
              "baseId": "C44",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 2,
              "isSilenced": 0,
              "spawnRank": 1
            },
            {
              "id": "ID_OPPONENT-13",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 10,
              "health": 30,
              "cost": 30,
              "name": "Pricklepillar",
              "description": "Taunt; Lethal",
              "abilities": [
                1,
                21
              ],
              "baseId": "C49",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 1,
              "isSilenced": 0,
              "spawnRank": 6,
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            }
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [],
          "deckSize": 0,
          "cardCount": 6,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 70,
          "manaMax": 70,
          "health": 60,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "fieldBack": [
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "EMPTY"
            },
          ],
          "hand": [
            {
              "id": "ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 40,
              "cost": 20,
              "name": "Blue Gipsy V3",
              "description": "Warcy: Deal 20 damage to random frozen enemy creature",
              "abilities": [
                37
              ],
              "baseId": "C52",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 40,
              "healthMax": 40,
            },
            {
              "id": "ID_PLAYER-5",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 60,
              "health": 50,
              "cost": 60,
              "name": "Frostland THRASHER-8",
              "description": "Warcy: Destroy a random frozen enemy creature",
              "abilities": [
                38
              ],
              "baseId": "C53",
              "attackStart": 60,
              "costStart": 60,
              "healthStart": 50,
              "healthMax": 50,
            },
          ],
          "deckSize": 0,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_PLAYER",
          "expiredStreak": 0
        },
      },
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "lastMoves": [],
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": [],
        "ID_OPPONENT": [],
      },
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support attack random frozen", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-4",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-4");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-4");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.targetId, "ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 50);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-4");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "ID_OPPONENT-8");
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[0].isFrozen, 2);

            assert.equal(opponentField[1].id, "ID_OPPONENT-13");
            assert.equal(opponentField[1].health, 30);
            assert.equal(opponentField[1].isFrozen, 1);

            resolve();
          }
        );
      });
    });

    it("should support kill random frozen", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-5",
            attributesJson: {
              fieldIndex: 2,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.targetId, "ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 10);
            assert.equal(playerState.manaMax, 70);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-5");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "EMPTY");

            assert.equal(opponentField[1].id, "ID_OPPONENT-13");
            assert.equal(opponentField[1].health, 30);
            assert.equal(opponentField[1].isFrozen, 1);

            resolve();
          }
        );
      });
    });
  });
});
