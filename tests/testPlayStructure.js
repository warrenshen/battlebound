const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge play structure", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("play structure", function() {
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
          "health": 60,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "ID_PLAYER-16",
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
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 5,
            },
            {
              "id": "ID_PLAYER-13",
              "playerId": "ID_PLAYER",
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
              "buffs": [],
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
              "id": "ID_PLAYER-0",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 2,
              "health": 30,
              "cost": 20,
              "name": "Taunt Structure",
              "baseId": "C36",
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30
            },
          ],
          "deckSize": 0,
          "deck": [],
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

    it("should support play structure", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayStructure",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-0",
            attributesJson: {
              fieldIndex: 7,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_STRUCTURE");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.fieldIndex, 7);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerFieldBack = playerState.fieldBack;
            assert.equal(playerFieldBack[1].id, "ID_PLAYER-0");
            assert.equal(playerFieldBack[1].health, 30);

            resolve();
          }
        );
      });
    });

    it("should support play structure grant taunt", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayStructure",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-0",
            attributesJson: {
              fieldIndex: 6,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_STRUCTURE");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.fieldIndex, 6);

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerFieldBack = playerState.fieldBack;
            assert.equal(playerFieldBack[0].id, "ID_PLAYER-0");
            assert.equal(playerFieldBack[0].health, 30);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-16");
            assert.equal(playerField[0].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[1].id, "ID_PLAYER-13");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            resolve();
          }
        );
      });
    });
  });
});
