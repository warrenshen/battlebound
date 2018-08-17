const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge damage taken", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("take damage", function() {
    it("should support damage player face on take damage", function() {
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
                "buffs": [],
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
                "id": "ID_PLAYER-25",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 80,
                "health": 70,
                "cost": 50,
                "name": "Hellbringer",
                "description": "Whenever this creature takes damage, damage your face 30 damage",
                "abilities": [
                  17
                ],
                "baseId": "C37",
                "attackStart": 80,
                "costStart": 50,
                "healthStart": 70,
                "healthMax": 70,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 2
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
            "hand": [],
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-25",
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
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-25");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 70);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-25");
            assert.equal(playerField[0].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it ("should not damage player face on take damage if divine shield", function() {
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
                "buffs": [],
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
                "id": "ID_PLAYER-25",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 80,
                "health": 70,
                "cost": 50,
                "name": "Hellbringer",
                "description": "Whenever this creature takes damage, damage your face 30 damage",
                "abilities": [
                  2,
                  17
                ],
                "baseId": "C37",
                "attackStart": 80,
                "costStart": 50,
                "healthStart": 70,
                "healthMax": 70,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 2
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
            "hand": [],
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-25",
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
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-25");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 100);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-25");
            assert.equal(playerField[0].health, 70);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support damage opponent face on take damage", function() {
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
                "buffs": [],
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
                "id": "ID_PLAYER-25",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 80,
                "health": 70,
                "cost": 50,
                "name": "Terratank",
                "description": "Whenever this creature takes damage, deal 20 damage to opponent",
                "abilities": [
                  41
                ],
                "baseId": "C37",
                "attackStart": 80,
                "costStart": 50,
                "healthStart": 70,
                "healthMax": 70,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 2
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
            "hand": [],
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-25",
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
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-25");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 100);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-25");
            assert.equal(playerField[0].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 80);

            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });
});
