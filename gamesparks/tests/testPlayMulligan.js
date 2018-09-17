const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge piercing", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("neither player waiting", function() {
    const challengeStateData = {
      "id": "ID_CHALLENGE",
      "mode": 1,
      "current": {
        "ID_PLAYER": {
          "hasTurn": 1,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
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
          "cardCount": 30,
          "mode": 1,
          "mulliganCards": [
            {
              "id": "ID_PLAYER-1",
              "level": 0,
              "category": 1,
              "color": 3,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": [],
              "baseId": "C15",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "buffsHand": []
            },
            {
              "id": "ID_PLAYER-0",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 10,
              "cost": 20,
              "name": "Blessed Newborn",
              "description": "Battlecry: Draw a card",
              "abilities": [
                4
              ],
              "baseId": "C3",
              "playerId": "ID_PLAYER",
              "attackStart": 10,
              "costStart": 20,
              "healthStart": 10,
              "healthMax": 10,
              "abilitiesStart": [
                4
              ],
              "buffsHand": []
            },
            {
              "id": "ID_PLAYER-19",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Shields Up!",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "buffsHand": []
            },
            {
              "id": "ID_PLAYER-2",
              "level": 0,
              "category": 0,
              "color": 3,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Wave Charmer",
              "description": "Charge",
              "abilities": [
                4
              ],
              "baseId": "C5",
              "playerId": "ID_PLAYER",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                4
              ],
              "buffsHand": []
            }
          ],
          "id": "ID_PLAYER",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_PLAYER-15",
              "playerId": "ID_PLAYER",
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
            {
              "id": "ID_PLAYER-16",
              "playerId": "ID_PLAYER",
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
            {
              "id": "ID_PLAYER-17",
              "playerId": "ID_PLAYER",
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
            {
              "id": "ID_PLAYER-18",
              "playerId": "ID_PLAYER",
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
        },
        "ID_OPPONENT": {
          "hasTurn": 0,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
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
          "mode": 1,
          "mulliganCards": [
            {
              "id": "ID_OPPONENT-6",
              "level": 0,
              "category": 0,
              "color": 2,
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
              "playerId": "ID_OPPONENT",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                1,
                21
              ],
              "buffsHand": []
            },
            {
              "id": "ID_OPPONENT-10",
              "level": 0,
              "category": 0,
              "color": 5,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "playerId": "ID_OPPONENT",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
              "abilitiesStart": [
                29
              ],
              "buffsHand": []
            },
            {
              "id": "ID_OPPONENT-7",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Shields Up!",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_OPPONENT",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "buffsHand": []
            }
          ],
          "id": "ID_OPPONENT",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_OPPONENT-15",
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
            },
            {
              "id": "ID_OPPONENT-16",
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
            },
            {
              "id": "ID_OPPONENT-17",
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
            },
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
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 0,
      "deathCount": 0,
      "nonce": 4,
    };

    it("should support discard all cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: [],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 2);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-15");
            assert.equal(playerHand[1].id, "ID_PLAYER-16");
            assert.equal(playerHand[2].id, "ID_PLAYER-17");
            assert.equal(playerHand[3].id, "ID_PLAYER-18");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 5);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 4);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-15");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-16");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].attributes.card.id, "ID_PLAYER-17");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[4].playerId, "ID_PLAYER");
            assert.equal(lastMoves[4].attributes.card.id, "ID_PLAYER-18");

            resolve();
          }
        );
      });
    });

    it("should support discard all cards - 3 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardIds: [],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 2);
            assert.equal(opponentState.mulliganCards.length, 0);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 3);
            assert.equal(opponentHand[0].id, "ID_OPPONENT-15");
            assert.equal(opponentHand[1].id, "ID_OPPONENT-16");
            assert.equal(opponentHand[2].id, "ID_OPPONENT-17");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 3);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-15");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-16");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[3].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[3].attributes.card.id, "ID_OPPONENT-17");

            resolve();
          }
        );
      });
    });

    it("should support discard some cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-19"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 2);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-15");
            assert.equal(playerHand[2].id, "ID_PLAYER-19");
            assert.equal(playerHand[3].id, "ID_PLAYER-16");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-15");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-16");

            resolve();
          }
        );
      });
    });

    it("should support discard some cards - 3 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardIds: ["ID_OPPONENT-7"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 2);
            assert.equal(opponentState.mulliganCards.length, 0);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 3);
            assert.equal(opponentHand[0].id, "ID_OPPONENT-15");
            assert.equal(opponentHand[1].id, "ID_OPPONENT-16");
            assert.equal(opponentHand[2].id, "ID_OPPONENT-7");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-15");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-16");

            resolve();
          }
        );
      });
    });

    it("should support discard no cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-0", "ID_PLAYER-19", "ID_PLAYER-2"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 2);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-0");
            assert.equal(playerHand[2].id, "ID_PLAYER-19");
            assert.equal(playerHand[3].id, "ID_PLAYER-2");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 0);

            resolve();
          }
        );
      });
    });

    it("should support discard no cards - 3 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardIds: ["ID_OPPONENT-6", "ID_OPPONENT-7", "ID_OPPONENT-10"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 2);
            assert.equal(opponentState.mulliganCards.length, 0);

            const opponentHand = opponentState.hand;
            assert.equal(opponentHand.length, 3);
            assert.equal(opponentHand[0].id, "ID_OPPONENT-6");
            assert.equal(opponentHand[1].id, "ID_OPPONENT-10");
            assert.equal(opponentHand[2].id, "ID_OPPONENT-7");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 0);

            resolve();
          }
        );
      });
    });
  });

  describe("one player waiting", function() {
    const challengeStateData = {
      "id": "ID_CHALLENGE",
      "mode": 1,
      "current": {
        "ID_PLAYER": {
          "hasTurn": 0,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
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
          "cardCount": 30,
          "mode": 1,
          "mulliganCards": [
            {
              "id": "ID_PLAYER-1",
              "level": 0,
              "category": 1,
              "color": 3,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": [],
              "baseId": "C15",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "buffsHand": []
            },
            {
              "id": "ID_PLAYER-0",
              "level": 1,
              "category": 0,
              "color": 2,
              "attack": 10,
              "health": 10,
              "cost": 20,
              "name": "Blessed Newborn",
              "description": "Battlecry: Draw a card",
              "abilities": [
                4
              ],
              "baseId": "C3",
              "playerId": "ID_PLAYER",
              "attackStart": 10,
              "costStart": 20,
              "healthStart": 10,
              "healthMax": 10,
              "abilitiesStart": [
                4
              ],
              "buffsHand": []
            },
            {
              "id": "ID_PLAYER-19",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Shields Up!",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_PLAYER",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "buffsHand": []
            },
            {
              "id": "ID_PLAYER-2",
              "level": 0,
              "category": 0,
              "color": 3,
              "attack": 20,
              "health": 30,
              "cost": 30,
              "name": "Wave Charmer",
              "description": "Charge",
              "abilities": [
                4
              ],
              "baseId": "C5",
              "playerId": "ID_PLAYER",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                4
              ],
              "buffsHand": []
            }
          ],
          "id": "ID_PLAYER",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_PLAYER-15",
              "playerId": "ID_PLAYER",
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
            {
              "id": "ID_PLAYER-16",
              "playerId": "ID_PLAYER",
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
            {
              "id": "ID_PLAYER-17",
              "playerId": "ID_PLAYER",
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
            {
              "id": "ID_PLAYER-18",
              "playerId": "ID_PLAYER",
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
        },
        "ID_OPPONENT": {
          "hasTurn": 1,
          "manaCurrent": 20,
          "manaMax": 20,
          "health": 300,
          "healthMax": 300,
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
              "id": "ID_OPPONENT-6",
              "level": 0,
              "category": 0,
              "color": 2,
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
              "playerId": "ID_OPPONENT",
              "attackStart": 10,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "abilitiesStart": [
                1,
                21
              ],
              "buffsHand": []
            },
            {
              "id": "ID_OPPONENT-10",
              "level": 0,
              "category": 0,
              "color": 5,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "playerId": "ID_OPPONENT",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
              "abilitiesStart": [
                29
              ],
              "buffsHand": []
            },
            {
              "id": "ID_OPPONENT-7",
              "level": 0,
              "category": 1,
              "color": 5,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Shields Up!",
              "description": "Give all your creatures shields",
              "abilities": [],
              "baseId": "C20",
              "playerId": "ID_OPPONENT",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null,
              "abilitiesStart": [],
              "buffsHand": []
            },
          ],
          "deckSize": 4,
          "cardCount": 8,
          "mode": 2,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0,
          "deck": [
            {
              "id": "ID_OPPONENT-15",
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
            },
            {
              "id": "ID_OPPONENT-16",
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
            },
            {
              "id": "ID_OPPONENT-17",
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
            },
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
      "turnCountByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 0,
      "deathCount": 0,
      "nonce": 4,
    };

    it("should support discard all cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: [],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 0);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-15");
            assert.equal(playerHand[1].id, "ID_PLAYER-16");
            assert.equal(playerHand[2].id, "ID_PLAYER-17");
            assert.equal(playerHand[3].id, "ID_PLAYER-18");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 7);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 4);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-15");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-16");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].attributes.card.id, "ID_PLAYER-17");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[4].playerId, "ID_PLAYER");
            assert.equal(lastMoves[4].attributes.card.id, "ID_PLAYER-18");

            assert.equal(lastMoves[5].category, "MOVE_CATEGORY_FINISH_MULLIGAN");

            assert.equal(lastMoves[6].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[6].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 0);
            assert.equal(opponentState.mulliganCards.length, 0);

            assert.equal(opponentState.hand.length, 4);

            resolve();
          }
        );
      });
    });

    it("should support discard some cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-19", "ID_PLAYER-1"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 0);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-15");
            assert.equal(playerHand[2].id, "ID_PLAYER-19");
            assert.equal(playerHand[3].id, "ID_PLAYER-16");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 5);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 2);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-15");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD_MULLIGAN");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-16");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_FINISH_MULLIGAN");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[4].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 0);
            assert.equal(opponentState.hand.length, 4);
            assert.equal(opponentState.mulliganCards.length, 0);

            resolve();
          }
        );
      });
    });

    it("should support discard no cards - 4 mulligan", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayMulligan",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardIds: ["ID_PLAYER-1", "ID_PLAYER-0", "ID_PLAYER-19", "ID_PLAYER-2"],
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.mode, 0);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.mode, 0);
            assert.equal(playerState.mulliganCards.length, 0);

            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 4);
            assert.equal(playerHand[0].id, "ID_PLAYER-1");
            assert.equal(playerHand[1].id, "ID_PLAYER-0");
            assert.equal(playerHand[2].id, "ID_PLAYER-19");
            assert.equal(playerHand[3].id, "ID_PLAYER-2");

            const lastMoves = response.scriptData.challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MULLIGAN");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.deckCardIndices.length, 0);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_FINISH_MULLIGAN");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.mode, 0);
            assert.equal(opponentState.hand.length, 4);
            assert.equal(opponentState.mulliganCards.length, 0);

            resolve();
          }
        );
      });
    });
  });
});
