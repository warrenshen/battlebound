const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge shielded", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("pop or not", function() {
    it("should pop shield on take damage", function() {
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
                "id": "ID_OPPONENT-9",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 60,
                "cost": 60,
                "name": "Temple Guardian",
                "description": "Taunt; Shield",
                "abilities": [
                  1,
                  2
                ],
                "baseId": "C9",
                "attackStart": 40,
                "costStart": 60,
                "healthStart": 60,
                "healthMax": 60,
                "buffs": [],
                "canAttack": 0,
                "isFrozen": 0,
                "spawnRank": 7
              },
              {
                "id": "ID_OPPONENT-1",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 30,
                "health": 50,
                "cost": 40,
                "name": "Waterborne Razorback",
                "description": "Charge; At the end of each turn, recover 20 health",
                "abilities": [
                  0,
                  8
                ],
                "baseId": "C1",
                "attackStart": 30,
                "costStart": 40,
                "healthStart": 60,
                "healthMax": 60,
                "buffs": [],
                "canAttack": 0,
                "isFrozen": 0,
                "spawnRank": 5
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
                "id": "EMPTY"
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "ID_PLAYER-7",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 30,
                "health": 40,
                "cost": 30,
                "name": "Emberkitty",
                "description": "Deal 10 damage to any opponent in front on end turn",
                "abilities": [
                  13
                ],
                "baseId": "C7",
                "attackStart": 30,
                "costStart": 30,
                "healthStart": 40,
                "healthMax": 40,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 8
              },
              {
                "id": "ID_PLAYER-4",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 30,
                "health": 10,
                "cost": 20,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 30,
                "costStart": 20,
                "healthStart": 20,
                "healthMax": 20,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 2
              },
              {
                "id": "EMPTY"
              }
            ],
            "hand": [],
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-4",
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

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[4].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "ID_OPPONENT-9");
            assert.equal(opponentField[2].abilities.indexOf(2) >= 0, false);
            assert.equal(opponentField[2].abilities.indexOf(1) >= 0, true);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            // Follow up attack after shield is popped.
            gamesparks.sendWithData(
              "LogEventRequest",
              {
                eventKey: "TestChallengeCardAttackCard",
                challengeStateString: JSON.stringify(challengeStateData),
                challengePlayerId: "ID_PLAYER",
                cardId: "ID_PLAYER-7",
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

                const playerState = challengeStateData.current["ID_PLAYER"];
                const playerField = playerState.field;
                assert.equal(playerField[3].id, "EMPTY");

                const opponentState = challengeStateData.current["ID_OPPONENT"];
                const opponentField = opponentState.field;

                assert.equal(opponentField[2].id, "ID_OPPONENT-9");
                assert.equal(opponentField[2].health, 30);
                assert.equal(opponentField[2].abilities.indexOf(2) >= 0, false);
                assert.equal(opponentField[2].abilities.indexOf(1) >= 0, true);

                assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
                assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

                resolve();
              }
            );
          }
        );
      });
    });

    it("should not pop shield on attack opponent face", function() {
      const challengeStateData = {
        "current": {
          "ID_OPPONENT": {
            "hasTurn": 0,
            "manaCurrent": 30,
            "manaMax": 50,
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
                "id": "ID_OPPONENT-4",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 30,
                "health": 20,
                "cost": 20,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 30,
                "costStart": 20,
                "healthStart": 20,
                "healthMax": 20,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 1
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "EMPTY"
              }
            ],
            "hand": [
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              }
            ],
            "deckSize": 11,
            "cardCount": 17,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0
          },
          "ID_PLAYER": {
            "hasTurn": 1,
            "manaCurrent": 0,
            "manaMax": 50,
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
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 30,
                "health": 60,
                "cost": 50,
                "name": "Poseidon's Handmaiden",
                "description": "Charge; Deathrattle: Deal 20 damage to your opponent",
                "abilities": [
                  0,
                  2
                ],
                "baseId": "C6",
                "attackStart": 30,
                "costStart": 50,
                "healthStart": 60,
                "healthMax": 60,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 5
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
            "hand": [
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              }
            ],
            "deckSize": 12,
            "cardCount": 17,
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
        "spawnCount": 6,
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
            cardId: "ID_PLAYER-6",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "TARGET_ID_FACE",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-6");
            assert.equal(playerField[2].health, 60);
            assert.equal(playerField[2].abilities.indexOf(2) >= 0, true);
            assert.equal(playerField[2].abilities.indexOf(0) >= 0, true);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 30);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });
  });

  describe("block specials", function() {
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
              "id": "ID_OPPONENT-9",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 60,
              "name": "Temple Guardian",
              "description": "Taunt; Shield",
              "abilities": [
                1,
                2
              ],
              "baseId": "C9",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 60,
              "healthMax": 60,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
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
              "id": "ID_PLAYER-14",
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
              "baseId": "C50",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 8,
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

    it("should block lethal", function() {
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
              targetId: "ID_OPPONENT-9",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-9");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "ID_OPPONENT-9");
            assert.equal(opponentField[2].abilities.indexOf(2) >= 0, false);
            assert.equal(opponentField[2].abilities.indexOf(1) >= 0, true);

            // Follow up attack after shield is popped.
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
                const playerField = playerState.field;
                assert.equal(playerField[1].id, "EMPTY");

                const opponentState = challengeStateData.current["ID_OPPONENT"];
                const opponentField = opponentState.field;
                assert.equal(opponentField[2].id, "EMPTY");

                resolve();
              }
            );
          }
        );
      });
    });
  });
});
