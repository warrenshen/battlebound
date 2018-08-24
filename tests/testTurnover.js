const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge turnover", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("max mana increase", function() {
    it("should not increase max mana on 0th turn", function() {
      const challengeStateData = {
        "current": {
          "ID_PLAYER": {
            "hasTurn": 1,
            "turnCount": 0,
            "manaCurrent": 10,
            "manaMax": 10,
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
            "hand": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
            "deckSize": 0,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
            "deck": [],
          },
          "ID_OPPONENT": {
            "hasTurn": 0,
            "turnCount": 0,
            "manaCurrent": 10,
            "manaMax": 10,
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
            "deckSize": 0,
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

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.turnCount, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.turnCount, 0);
            assert.equal(opponentState.manaCurrent, 10);
            assert.equal(opponentState.manaMax, 10);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 0);

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

    it("should increase max mana on 1st turn", function() {
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
            "hand": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
            "deckSize": 0,
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
            "manaCurrent": 10,
            "manaMax": 10,
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
            "deckSize": 0,
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

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.turnCount, 2);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.turnCount, 1);
            assert.equal(opponentState.manaCurrent, 20);
            assert.equal(opponentState.manaMax, 20);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 0);

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
  });

  describe("draw cards", function() {
    it("should support draw card hand full if next player's hand is full and change player turns", function() {
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
            "hand": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
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
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
            ],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [
              {
                "id": "C6-ID_OPPONENT-6",
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
              }
            ],
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
            assert.equal(opponentHand.length, 10);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_HAND_FULL");
            assert.equal(lastMoves[1].attributes.card.id, "C6-ID_OPPONENT-6");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("draw cards", function() {
    it("should support draw card deck empty if next player's deck is empty", function() {
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
            "hand": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
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
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
            ],
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

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 10);

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

    it("should support draw card hand full if next player's hand is full and change player turns", function() {
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
            "hand": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
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
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
            ],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [
              {
                "id": "C6-ID_OPPONENT-6",
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
              }
            ],
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
            assert.equal(opponentHand.length, 10);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_HAND_FULL");
            assert.equal(lastMoves[1].attributes.card.id, "C6-ID_OPPONENT-6");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("creature states", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 0,
          "turnCount": 1,
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
              "id": "C5-ID_OPPONENT-5",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 20,
              "cost": 20,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C5",
              "attackStart": 30,
              "costStart": 20,
              "healthStart": 20,
              "healthMax": 20,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
            },
            {
              "id": "C4-ID_OPPONENT-4",
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 1,
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
          "deckSize": 0,
          "deck": [],
          "cardCount": 17,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
          "turnCount": 1,
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
              "id": "C6-ID_PLAYER-6",
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 1,
              "spawnRank": 5
            },
            {
              "id": "C7-ID_PLAYER-7",
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
              "baseId": "C7",
              "attackStart": 30,
              "costStart": 50,
              "healthStart": 60,
              "healthMax": 60,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 2,
              "spawnRank": 6
            },
            {
              "id": "C8-ID_PLAYER-8",
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
              "baseId": "C8",
              "attackStart": 30,
              "costStart": 50,
              "healthStart": 60,
              "healthMax": 60,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 7
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
          "deckSize": 0,
          "deck": [],
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
      "deadCards": [],
      "moveTakenThisTurn": 0,
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 7,
      "deathCount": 0,
      "nonce": 14
    };

    it("should decrement is frozen", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeEndTurn",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;

            assert.equal(playerField[2].id, "C6-ID_PLAYER-6");
            assert.equal(playerField[2].isFrozen, 0);

            assert.equal(playerField[3].id, "C7-ID_PLAYER-7");
            assert.equal(playerField[3].isFrozen, 1);

            assert.equal(playerField[4].id, "C8-ID_PLAYER-8");
            assert.equal(playerField[4].isFrozen, 0);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            // Opponent creatures should not change in terms of frozen.
            assert.equal(opponentField[2].id, "C5-ID_OPPONENT-5");
            assert.equal(opponentField[2].isFrozen, 0);

            assert.equal(opponentField[3].id, "C4-ID_OPPONENT-4");
            assert.equal(opponentField[3].isFrozen, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("triggered effects", function() {
    it("should support draw card on both end turns", function() {
      const challengeStateData = {
        "current": {
          "ID_PLAYER": {
            "hasTurn": 0,
            "turnCount": 1,
            "manaCurrent": 20,
            "manaMax": 30,
            "health": 100,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "C39-5b0b017502bd4e052f08a28d-23",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 30,
                "cost": 50,
                "name": "Dionysian Tosspot",
                "description": "Draw a card at end of either player's turn",
                "abilities": [
                  25
                ],
                "baseId": "C39",
                "attackStart": 40,
                "costStart": 50,
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
              },
            ],
            "hand": [],
            "deckSize": 3,
            "deck": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
          },
          "ID_OPPONENT": {
            "hasTurn": 1,
            "turnCount": 1,
            "manaCurrent": 30,
            "manaMax": 40,
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
            "hand": [
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
            ],
            "deckSize": 4,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
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
            challengePlayerId: "ID_OPPONENT"
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });

    it("should support draw card on both end turns", function() {
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
                "id": "C39-5b0b017502bd4e052f08a28d-23",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 30,
                "cost": 50,
                "name": "Dionysian Tosspot",
                "description": "Draw a card at end of either player's turn",
                "abilities": [
                  25
                ],
                "baseId": "C39",
                "attackStart": 40,
                "costStart": 50,
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
              },
            ],
            "hand": [],
            "deckSize": 5,
            "deck": [
              {
                "id": "C1-ID_OPPONENT-1",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30
              },
              {
                "id": "C4-ID_OPPONENT-4",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10
              },
              {
                "id": "C6-ID_PLAYER-6",
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
              },
            ],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
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
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
              {
                "id": "HIDDEN"
              },
            ],
            "deckSize": 4,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
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

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });

    it("should support unstable power death", function() {
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
            "deckSize": 0,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
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
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 50,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "Draw a card at end of either player's turn",
                "abilities": [
                  7
                ],
                "baseId": "C39",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30,
                "buffsField": [
                  1001
                ],
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
              },
            ],
            "hand": [],
            "deckSize": 0,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
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

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("triggered effects", function() {
    it("should support buff random friendly", function() {
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
                "id": "ID_PLAYER-23",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 30,
                "cost": 20,
                "name": "Marshwater Squealer",
                "description": "Draw a card at end of either player's turn",
                "abilities": [
                  7
                ],
                "baseId": "C38",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30,
                "buffsField": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 0
              },
              {
                "id": "ID_PLAYER-22",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 30,
                "cost": 10,
                "name": "Firesmith Apprentice",
                "description": "Turnover: Give random friendly creature +10/10",
                "abilities": [
                  43
                ],
                "baseId": "C39",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 30,
                "healthMax": 30,
                "buffsField": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 1
              },
              {
                "id": "ID_PLAYER-24",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 30,
                "cost": 20,
                "name": "PAL_V1",
                "description": "Turnover: Give random friendly creature +0/20",
                "abilities": [
                  40
                ],
                "baseId": "C40",
                "attackStart": 10,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30,
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
            ],
            "hand": [],
            "deckSize": 0,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
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
            "deckSize": 0,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
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

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-22");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.targetId, "ID_PLAYER-23");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-24");
            assert.equal(lastMoves[2].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.targetId, "ID_PLAYER-23");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY");
            assert.equal(lastMoves[3].playerId, "ID_OPPONENT");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;

            assert.equal(playerField[0].id, "ID_PLAYER-23");
            assert.equal(playerField[0].attack, 30);
            assert.equal(playerField[0].attackStart, 20);
            assert.equal(playerField[0].health, 60);
            assert.equal(playerField[0].healthMax, 60);
            assert.equal(playerField[0].healthStart, 30);
            assert.equal(playerField[0].buffsField.indexOf(1003) >= 0, true);
            assert.equal(playerField[0].buffsField.indexOf(1004) >= 0, true);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });

    it("should support buff random friendly - no friendly creatures", function() {
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
                "id": "ID_PLAYER-24",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 30,
                "cost": 20,
                "name": "PAL_V1",
                "description": "Turnover: Give random friendly creature +0/20",
                "abilities": [
                  40
                ],
                "baseId": "C40",
                "attackStart": 10,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30,
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
            ],
            "hand": [],
            "deckSize": 0,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
            "expiredStreak": 0,
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
            "deckSize": 0,
            "deck": [],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
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

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;

            assert.equal(playerField[2].id, "ID_PLAYER-24");
            assert.equal(playerField[2].attack, 10);
            assert.equal(playerField[2].attackStart, 10);
            assert.equal(playerField[2].health, 30);
            assert.equal(playerField[2].healthMax, 30);
            assert.equal(playerField[2].healthStart, 30);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 0);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 1);

            resolve();
          }
        );
      });
    });
  });
});
