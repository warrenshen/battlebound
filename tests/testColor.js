const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge color synergy", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("draw cards", function() {
    it("should support mana decrease on color synergy", function() {
      const challengeStateData = {
        "current": {
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
            "hand": [],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
          },
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
                "id": "ID_PLAYER-1",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
                "id": "ID_PLAYER-4",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
            ],
            "deckSize": 1,
            "deck": [
              {
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
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

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 3);

            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[0].cost, 10);
            assert.equal(playerHand[0].buffsHand[0], 2000);

            assert.equal(playerHand[1].id, "ID_PLAYER-4");
            assert.equal(playerHand[1].cost, 0);
            assert.equal(playerHand[1].buffsHand[0], 2000);

            assert.equal(playerHand[2].id, "ID_PLAYER-6");
            assert.equal(playerHand[2].cost, 0);
            assert.equal(playerHand[2].buffsHand[0], 2000);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-6");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });

    it("should support mana no change on no color synergy", function() {
      const challengeStateData = {
        "current": {
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
            "hand": [],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
          },
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
                "id": "ID_PLAYER-1",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
                "id": "ID_PLAYER-4",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
            ],
            "deckSize": 1,
            "deck": [
              {
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 1,
                "color": 0,
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
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
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

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 3);

            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[0].cost, 20);

            assert.equal(playerHand[1].id, "ID_PLAYER-4");
            assert.equal(playerHand[1].cost, 10);

            assert.equal(playerHand[2].id, "ID_PLAYER-6");
            assert.equal(playerHand[2].cost, 10);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-6");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });

    it("should support mana no change on neutral synergy", function() {
      const challengeStateData = {
        "current": {
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
            "hand": [],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
          },
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
                "id": "ID_PLAYER-1",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 0,
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
                "id": "ID_PLAYER-4",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 0,
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
            ],
            "deckSize": 1,
            "deck": [
              {
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 1,
                "color": 0,
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
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
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

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 3);

            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[0].cost, 20);

            assert.equal(playerHand[1].id, "ID_PLAYER-4");
            assert.equal(playerHand[1].cost, 10);

            assert.equal(playerHand[2].id, "ID_PLAYER-6");
            assert.equal(playerHand[2].cost, 10);

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-6");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });

    it("should not have negative cost on color synergy", function() {
      const challengeStateData = {
        "current": {
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
            "hand": [],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
          },
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
                "id": "ID_PLAYER-1",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
                "id": "ID_PLAYER-4",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
                "attack": 10,
                "health": 10,
                "cost": 0,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 0,
                "healthStart": 10,
                "healthMax": 10
              },
            ],
            "deckSize": 1,
            "deck": [
              {
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
                "attack": 20,
                "health": 10,
                "cost": 0,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C6",
                "attackStart": 20,
                "costStart": 0,
                "healthStart": 10,
                "healthMax": 10,
              }
            ],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
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
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-6");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 3);

            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[0].cost, 10);
            assert.equal(playerHand[0].buffsHand[0], 2000);

            assert.equal(playerHand[1].id, "ID_PLAYER-4");
            assert.equal(playerHand[1].cost, 0);
            assert.equal(playerHand[1].buffsHand[0], 2000);

            assert.equal(playerHand[2].id, "ID_PLAYER-6");
            assert.equal(playerHand[2].cost, 0);
            assert.equal(playerHand[2].buffsHand[0], 2000);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });

    it("should not change cost if existing color synergy", function() {
      const challengeStateData = {
        "current": {
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
            "hand": [],
            "deckSize": 4,
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0,
            "deck": [],
          },
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
                "id": "ID_PLAYER-1",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
                "attack": 20,
                "health": 30,
                "cost": 10,
                "name": "Marshwater Squealer",
                "description": "At the end of each turn, recover 10 health",
                "abilities": [
                  7
                ],
                "baseId": "C1",
                "attackStart": 20,
                "costStart": 20,
                "healthStart": 30,
                "healthMax": 30,
                "buffsHand": [2000],
              },
              {
                "id": "ID_PLAYER-4",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
                "attack": 10,
                "health": 10,
                "cost": 0,
                "name": "Firebug Catelyn",
                "description": "",
                "abilities": [],
                "baseId": "C4",
                "attackStart": 10,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10,
                "buffsHand": [2000],
              },
              {
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
                "attack": 20,
                "health": 10,
                "cost": 0,
                "name": "Young Kyo",
                "description": "",
                "abilities": [],
                "baseId": "C6",
                "attackStart": 20,
                "costStart": 10,
                "healthStart": 10,
                "healthMax": 10,
                "buffsHand": [2000],
              },
            ],
            "deckSize": 1,
            "deck": [
              {
                "id": "ID_PLAYER-7",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 0,
                "color": 1,
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
                "buffsHand": [],
              }
            ],
            "cardCount": 8,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_PLAYER",
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
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_END_TURN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-7");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);

            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[0].cost, 10);
            assert.equal(playerHand[0].buffsHand.length, 1);
            assert.equal(playerHand[0].buffsHand[0], 2000);

            assert.equal(playerHand[1].id, "ID_PLAYER-4");
            assert.equal(playerHand[1].cost, 0);
            assert.equal(playerHand[1].buffsHand.length, 1);
            assert.equal(playerHand[1].buffsHand[0], 2000);

            assert.equal(playerHand[2].id, "ID_PLAYER-6");
            assert.equal(playerHand[2].cost, 0);
            assert.equal(playerHand[2].buffsHand.length, 1);
            assert.equal(playerHand[2].buffsHand[0], 2000);

            assert.equal(playerHand[3].id, "ID_PLAYER-7");
            assert.equal(playerHand[3].cost, 0);
            assert.equal(playerHand[3].buffsHand.length, 1);
            assert.equal(playerHand[3].buffsHand[0], 2000);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);

            resolve();
          }
        );
      });
    });
  });

  describe("play cards", function() {
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
          "manaCurrent": 50,
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
              "id": "ID_PLAYER-1",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "color": 1,
              "attack": 20,
              "health": 30,
              "cost": 10,
              "name": "Marshwater Squealer",
              "description": "At the end of each turn, recover 10 health",
              "abilities": [
                7
              ],
              "baseId": "C1",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffsHand": [
                2000
              ]
            },
            {
              "id": "ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "color": 1,
              "attack": 10,
              "health": 10,
              "cost": 0,
              "name": "Firebug Catelyn",
              "description": "",
              "abilities": [],
              "baseId": "C4",
              "attackStart": 10,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
              "buffsHand": [
                2000
              ]
            },
            {
              "id": "ID_PLAYER-6",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "color": 1,
              "attack": 20,
              "health": 10,
              "cost": 0,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
              "buffsHand": [
                2000
              ]
            },
            {
              "id": "ID_PLAYER-7",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 20,
              "health": 30,
              "cost": 10,
              "name": "Marshwater Squealer",
              "description": "At the end of each turn, recover 10 health",
              "abilities": [
                7
              ],
              "baseId": "C1",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffsHand": [
                2000
              ]
            },
            {
              "id": "ID_PLAYER-8",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 10,
              "cost": 0,
              "name": "Firebug Catelyn",
              "description": "",
              "abilities": [],
              "baseId": "C4",
              "attackStart": 10,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
              "buffsHand": [
                2000
              ]
            },
            {
              "id": "ID_PLAYER-9",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 20,
              "health": 10,
              "cost": 0,
              "name": "Young Kyo",
              "description": "",
              "abilities": [],
              "baseId": "C6",
              "attackStart": 20,
              "costStart": 10,
              "healthStart": 10,
              "healthMax": 10,
              "buffsHand": [
                2000
              ]
            },
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

    it("should remove mana decrease on lose color synergy", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-1",
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
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-1");
            assert.equal(lastMoves[0].attributes.fieldIndex, 2);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 50);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 5);

            assert.equal(playerHand[0].id, "ID_PLAYER-4");
            assert.equal(playerHand[0].cost, 10);
            assert.equal(playerHand[0].buffsHand.length, 0);

            assert.equal(playerHand[1].id, "ID_PLAYER-6");
            assert.equal(playerHand[1].cost, 10);
            assert.equal(playerHand[1].buffsHand.length, 0);

            assert.equal(playerHand[2].id, "ID_PLAYER-7");
            assert.equal(playerHand[2].cost, 10);
            assert.equal(playerHand[2].buffsHand[0], 2000);

            assert.equal(playerHand[3].id, "ID_PLAYER-8");
            assert.equal(playerHand[3].cost, 0);
            assert.equal(playerHand[2].buffsHand[0], 2000);

            assert.equal(playerHand[4].id, "ID_PLAYER-9");
            assert.equal(playerHand[4].cost, 0);
            assert.equal(playerHand[2].buffsHand[0], 2000);

            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-1");

            resolve();
          }
        );
      });
    });
  });
});
