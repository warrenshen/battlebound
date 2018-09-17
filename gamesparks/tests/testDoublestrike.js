const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge doublestrike", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("doublestrike", function() {
    it("should recharge can attack to two", function() {
      const challengeStateData = {
        "current": {
          "ID_PLAYER": {
            "hasTurn": 1,
            "turnCount": 1,
            "manaCurrent": 20,
            "manaMax": 30,
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
            "hand": [],
            "deckSize": 5,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
            "deck": [],
          },
          "ID_OPPONENT": {
            "hasTurn": 0,
            "turnCount": 1,
            "manaCurrent": 30,
            "manaMax": 40,
            "health": 100,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "ID_OPPONENT-23",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 30,
                "cost": 50,
                "name": "Shurikana",
                "description": "Doublestrike",
                "abilities": [
                  47
                ],
                "baseId": "C39",
                "attackStart": 40,
                "costStart": 50,
                "healthStart": 40,
                "healthMax": 40,
                "buffsField": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 1
              },
              {
                "id": "ID_OPPONENT-6",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 10,
                "cost": 10,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C6",
                "attackStart": 20,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10,
                "buffsField": [],
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
            ],
            "hand": [],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
          },
        },
        "opponentIdByPlayerId": {
          "ID_PLAYER": "ID_OPPONENT",
          "ID_OPPONENT": "ID_PLAYER",
        },
        "lastMoves": [],
        "moves": [],
        "deadCards": [],
        "moveTakenThisTurn": 0,
        "expiredStreakByPlayerId": {
          "ID_PLAYER": 0,
          "ID_OPPONENT": 0,
        },
        "spawnCount": 0,
        "deathCount": 0,
        "nonce": 4,
      };

      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeEndTurn",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER"
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 0);

            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-23");
            assert.equal(opponentField[0].canAttack, 2);

            assert.equal(opponentField[1].id, "ID_OPPONENT-6");
            assert.equal(opponentField[1].canAttack, 1);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });

    it("should allow two attacks", function() {
      const challengeStateData = {
        "current": {
          "ID_PLAYER": {
            "hasTurn": 1,
            "turnCount": 1,
            "manaCurrent": 20,
            "manaMax": 30,
            "health": 100,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "ID_PLAYER-2",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 40,
                "cost": 50,
                "name": "Shurikana",
                "description": "Doublestrike",
                "abilities": [
                  47
                ],
                "baseId": "C39",
                "attackStart": 40,
                "costStart": 50,
                "healthStart": 40,
                "healthMax": 40,
                "buffsField": [],
                "canAttack": 2,
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
              },
            ],
            "hand": [],
            "deckSize": 5,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
            "deck": [],
          },
          "ID_OPPONENT": {
            "hasTurn": 0,
            "turnCount": 1,
            "manaCurrent": 30,
            "manaMax": 40,
            "health": 100,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "ID_OPPONENT-6",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 10,
                "cost": 10,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C6",
                "attackStart": 20,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10,
                "buffsField": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 2
              },
              {
                "id": "ID_OPPONENT-7",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 10,
                "cost": 10,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C6",
                "attackStart": 20,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10,
                "buffsField": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 3
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
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
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
        "spawnCount": 0,
        "deathCount": 0,
        "nonce": 4,
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
              targetId: "ID_OPPONENT-6",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-2");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-6");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-2");
            assert.equal(playerField[0].health, 20);
            assert.equal(playerField[0].canAttack, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            assert.equal(opponentField[1].id, "ID_OPPONENT-7");
            assert.equal(opponentField[1].health, 10);

            gamesparks.sendWithData(
              "LogEventRequest",
              {
                eventKey: "TestChallengeCardAttackCard",
                challengeStateString: JSON.stringify(challengeStateData),
                challengePlayerId: "ID_PLAYER",
                cardId: "ID_PLAYER-2",
                attributesJson: {
                  fieldId: "ID_OPPONENT",
                  targetId: "ID_OPPONENT-7",
                },
              },
              function(response) {
                const challengeStateData = response.scriptData.challengeStateData;

                const lastMoves = challengeStateData.lastMoves;
                assert.equal(lastMoves.length, 1);
                assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
                assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-2");
                assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
                assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-7");

                const playerState = challengeStateData.current["ID_PLAYER"];
                const playerField = playerState.field;
                assert.equal(playerField[0].id, "EMPTY");

                const opponentState = challengeStateData.current["ID_OPPONENT"];
                const opponentField = opponentState.field;
                assert.equal(opponentField[0].id, "EMPTY");
                assert.equal(opponentField[1].id, "EMPTY");

                resolve();
              }
            );
          }
        );
      });
    });
  });
});
