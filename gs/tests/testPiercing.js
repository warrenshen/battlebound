const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge piercing", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("piercing", function() {
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
              "id": "ID_OPPONENT-9",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 30,
              "cost": 30,
              "name": "Lighthunter",
              "description": "Piercing",
              "abilities": [
                45
              ],
              "baseId": "C49",
              "attackStart": 40,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2,
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
          "health": 90,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "ID_PLAYER-13",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 30,
              "cost": 30,
              "name": "Lighthunter",
              "description": "Piercing",
              "abilities": [
                45
              ],
              "baseId": "C49",
              "attackStart": 40,
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
              "id": "ID_PLAYER-14",
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
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 7
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
          "hand": [],
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

    it("should support piercing on attack", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-13",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-8",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 90);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-13");
            assert.equal(playerField[0].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 70);

            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support piercing on defend", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-14",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-9",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-14");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-9");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 60);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 100);

            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "ID_OPPONENT-9");
            assert.equal(opponentField[1].health, 10);

            resolve();
          }
        );
      });
    });
  });
});
