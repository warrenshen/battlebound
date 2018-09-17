const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge deathwish", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("deathwish", function() {
    it("should run Bombshell Bombadier's deathwish", function() {
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
                "id": "ID_OPPONENT-5",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 50,
                "health": 10,
                "cost": 70,
                "name": "Bombshell Bombadier",
                "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
                "abilities": [
                  0,
                  12
                ],
                "baseId": "C10",
                "attackStart": 50,
                "costStart": 70,
                "healthStart": 30,
                "healthMax": 30,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
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
                "id": "ID_PLAYER-2",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 30,
                "health": 30,
                "cost": 40,
                "name": "Waterborne Razorback",
                "description": "Charge; At the end of each turn, recover 20 health",
                "abilities": [
                  0,
                  8
                ],
                "baseId": "C2",
                "attackStart": 30,
                "costStart": 40,
                "healthStart": 50,
                "healthMax": 50,
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-2",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-2");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-5");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-5");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-5");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[3].attributes.card.id, "ID_OPPONENT-5");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.healthMax, 100);
            assert.equal(playerState.health, 70);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should run both Bombshell Bombadier's deathwish", function() {
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
                "id": "ID_OPPONENT-5",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 50,
                "health": 10,
                "cost": 70,
                "name": "Bombshell Bombadier",
                "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
                "abilities": [
                  0,
                  12
                ],
                "baseId": "C10",
                "attackStart": 50,
                "costStart": 70,
                "healthStart": 30,
                "healthMax": 30,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
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
                "id": "EMPTY"
              },
              {
                "id": "ID_PLAYER-10",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 50,
                "health": 40,
                "cost": 70,
                "name": "Bombshell Bombadier",
                "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
                "abilities": [
                  12
                ],
                "baseId": "C10",
                "attackStart": 50,
                "costStart": 70,
                "healthStart": 40,
                "healthMax": 40,
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-10",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 7);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-10");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-10");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].attributes.card.id, "ID_PLAYER-10");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[5].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[6].category, "MOVE_CATEGORY_RANDOM_TARGET");

            assert.equal(lastMoves[4].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[5].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[6].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[4].attributes.card.id, "ID_OPPONENT-5");
            assert.equal(lastMoves[5].attributes.card.id, "ID_OPPONENT-5");
            assert.equal(lastMoves[6].attributes.card.id, "ID_OPPONENT-5");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "EMPTY");

            assert.equal(playerState.healthMax, 100);
            assert.equal(playerState.health, 60);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");

            assert.equal(opponentState.healthMax, 100);
            assert.equal(opponentState.health, 70);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should run Adderspine Weevil's deathwish", function() {
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
                "id": "C50-ID_OPPONENT-12",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 20,
                "cost": 60,
                "name": "Adderspine Weevil",
                "description": "Taunt; Deathwish: Deal 20 dmg to all opponent creatures",
                "abilities": [
                  1,
                  30
                ],
                "baseId": "C50",
                "attackStart": 40,
                "costStart": 60,
                "healthStart": 60,
                "healthMax": 60,
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 9
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
                "id": "C53-ID_PLAYER-9",
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
                "buffs": [],
                "canAttack": 1,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 2
              },
              {
                "id": "C40-ID_PLAYER-22",
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
                "healthMax": 30,
                "buffs": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 3
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "C40-ID_PLAYER-23",
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
                "healthMax": 30,
                "buffs": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 4
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

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C53-ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C50-ID_OPPONENT-12",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C53-ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C50-ID_OPPONENT-12");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "EMPTY");

            assert.equal(playerField[1].id, "C40-ID_PLAYER-22");
            assert.equal(playerField[1].health, 10);

            assert.equal(playerField[3].id, "C40-ID_PLAYER-23");
            assert.equal(playerField[3].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });
});
