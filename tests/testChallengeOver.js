const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge over", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("win", function() {
    it("should win on attack opponent face", function() {
      const challengeStateData = {
        "current": {
          "ID_OPPONENT": {
            "hasTurn": 0,
            "manaCurrent": 30,
            "manaMax": 50,
            "health": 20,
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
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CHALLENGE_OVER");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            resolve();
          }
        );
      });
    });

    it("should win by piercing", function() {
      const challengeStateData = {
        "current": {
          "ID_OPPONENT": {
            "hasTurn": 0,
            "manaCurrent": 0,
            "manaMax": 70,
            "health": 10,
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
                  1,
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
            "health": 60,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "EMPTY"
              },
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
              assert.equal(lastMoves.length, 2);

              assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
              assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-13");
              assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
              assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-8");

              assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CHALLENGE_OVER");
              assert.equal(lastMoves[1].playerId, "ID_PLAYER");

              const playerState = challengeStateData.current["ID_PLAYER"];
              const playerField = playerState.field;
              assert.equal(playerField[1].id, "ID_PLAYER-13");
              assert.equal(playerField[1].health, 10);

              const opponentState = challengeStateData.current["ID_OPPONENT"];
              assert.equal(opponentState.health, -10);

              const opponentField = opponentState.field;
              assert.equal(opponentField[0].id, "EMPTY");

              resolve();
            }
          );
        });
      });
    });
  });

  describe("lose", function() {
    it("should support damage player face", function() {
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
            "health": 20,
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
            "hand": [
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
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-26");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CHALLENGE_OVER");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 50);
            assert.equal(playerState.manaMax, 70);
            assert.equal(playerState.hand.length, 0);
            assert.equal(playerState.health, 0);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-26");

            resolve();
          }
        );
      });
    });

    it("should support damage player face on take damage", function() {
      const challengeStateData = {
        "current": {
          "ID_OPPONENT": {
            "hasTurn": 1,
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
                "canAttack": 1,
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
            "hasTurn": 0,
            "manaCurrent": 70,
            "manaMax": 70,
            "health": 20,
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
                "canAttack": 0,
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
            challengePlayerId: "ID_OPPONENT",
            cardId: "ID_OPPONENT-8",
            attributesJson: {
              fieldId: "ID_PLAYER",
              targetId: "ID_PLAYER-25",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_OPPONENT-8");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.targetId, "ID_PLAYER-25");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CHALLENGE_OVER");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, -10);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-25");
            assert.equal(playerField[0].health, 50);

            resolve();
          }
        );
      });
    });
  });

  describe("surrender", function() {
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
          "health": 20,
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

    it("should support surrender by player", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeSurrender",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_SURRENDER_BY_CHOICE");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CHALLENGE_OVER");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            resolve();
          }
        );
      });
    });

    it("should support surrender by opponent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeSurrender",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_SURRENDER_BY_CHOICE");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CHALLENGE_OVER");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            resolve();
          }
        );
      });
    });
  });
});
