const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge spells", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("spells", function() {
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
              "id": "ID_OPPONENT-5",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 50,
              "health": 10,
              "cost": 70,
              "name": "Bombshell Bombadier",
              "description": "Charge; Deathrattle: Randomly fire off 3 bombs to enemy units, dealing 10 damage each",
              "abilities": [],
              "baseId": "C10",
              "attackStart": 50,
              "costStart": 70,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 2
            },
            {
              "id": "ID_OPPONENT-2",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 3
            },
            {
              "id": "ID_OPPONENT-3",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 0,
              "isFrozen": 0,
              "spawnRank": 4
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 5
            },
            {
              "id": "ID_PLAYER-5",
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
              "baseId": "C3",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 6
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
              "id": "ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Brr Brr Blizzard",
              "description": "Freeze all opponent creatures",
              "abilities": null,
              "baseId": "C23",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-1",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 50,
              "name": "Riot Up",
              "description": "Give all your creatures shields",
              "abilities": null,
              "baseId": "C20",
              "attackStart": null,
              "costStart": 50,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-0",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 20,
              "name": "Touch of Zeus",
              "description": "Deal 20 damage to a creature",
              "abilities": null,
              "baseId": "C19",
              "attackStart": null,
              "costStart": 20,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-2",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 30,
              "name": "Deep Freeze",
              "description": "Deal 10 damage to creature and freeze it",
              "abilities": null,
              "baseId": "C21",
              "attackStart": null,
              "costStart": 30,
              "healthStart": null,
              "healthMax": null
            },
            {
              "id": "ID_PLAYER-3",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 40,
              "name": "Widespread Frostbite",
              "description": "Freeze creature and one opposite it for two turns, and two adjacent creatures for one turn",
              "abilities": null,
              "baseId": "C22",
              "attackStart": null,
              "costStart": 40,
              "healthStart": null,
              "healthMax": null
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

    it("should support brr brr blizzard", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-4",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-4");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.hand.length, 4);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].isFrozen, 1);
            assert.equal(opponentField[1].isFrozen, 1);
            assert.equal(opponentField[2].isFrozen, 1);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support riot up", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-1",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-1");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.hand.length, 4);

            const playerField = playerState.field;
            assert.equal(playerField[1].abilities.indexOf(2) >= 0, true);
            assert.equal(playerField[2].abilities.indexOf(2) >= 0, true);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support touch of zeus", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-0",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-2");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.hand.length, 4);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[1].id, "EMPTY");

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support deep freeze", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-2",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-2");
            assert.equal(lastMoves[0].attributes.card.name, "Deep Freeze");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-2");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.hand.length, 4);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "ID_OPPONENT-2");
            assert.equal(opponentField[1].health, 20);
            assert.equal(opponentField[1].isFrozen, 1);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });

    it("should support widespread frostbite", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-3",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-3");
            assert.equal(lastMoves[0].attributes.card.name, "Widespread Frostbite");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-2");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.hand.length, 4);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "ID_OPPONENT-5");
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[0].isFrozen, 1);

            assert.equal(opponentField[1].id, "ID_OPPONENT-2");
            assert.equal(opponentField[1].health, 30);
            assert.equal(opponentField[1].isFrozen, 2);

            assert.equal(opponentField[2].id, "ID_OPPONENT-3");
            assert.equal(opponentField[2].health, 30);
            assert.equal(opponentField[2].isFrozen, 1);

            assert.equal(challengeStateData.current["ID_PLAYER"].hasTurn, 1);
            assert.equal(challengeStateData.current["ID_OPPONENT"].hasTurn, 0);
            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
              "id": "ID_OPPONENT-5",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 20,
              "name": "Firebug Catelyn",
              "description": "Deathrattle: Damage opponent face by 10 dmg",
              "abilities": [
                10,
              ],
              "baseId": "C10",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
            },
            {
              "id": "ID_OPPONENT-2",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 1,
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
            },
            {
              "id": "ID_PLAYER-5",
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
              "baseId": "C3",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 0
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
              "id": "ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 50,
              "name": "Death Note",
              "description": "Kill an opponent creature",
              "baseId": "C23",
              "costStart": 50,
            },
            {
              "id": "ID_PLAYER-5",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 30,
              "name": "Mudslinging",
              "description": "Give all friendly creatures taunt",
              "baseId": "C24",
              "costStart": 30,
            },
            {
              "id": "ID_PLAYER-6",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
            {
              "id": "ID_PLAYER-7",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 20,
              "name": "Condemn",
              "description": "Silence a creatures",
              "baseId": "C26",
              "costStart": 20,
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
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

    it("should support death note", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-4",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-4");
            assert.equal(lastMoves[0].attributes.card.name, "Death Note");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-5");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 90);
            assert.equal(playerState.manaCurrent, 20);

            resolve();
          }
        );
      });
    });

    it("should support mudslinging", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-5",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-5");
            assert.equal(lastMoves[0].attributes.card.name, "Mudslinging")

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "ID_PLAYER-2");
            assert.equal(playerField[1].abilities.indexOf(1) >= 0, true);

            assert.equal(playerField[2].id, "ID_PLAYER-5");
            assert.equal(playerField[2].abilities.indexOf(1) >= 0, true);

            resolve();
          }
        );
      });
    });

    it("should support silence of the lambs", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-6",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-6");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 30);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "ID_PLAYER-2");
            assert.equal(playerField[1].isSilenced, 1);

            assert.equal(playerField[2].id, "ID_PLAYER-5");
            assert.equal(playerField[2].isSilenced, 1);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-5");
            assert.equal(opponentField[0].isSilenced, 1);

            assert.equal(opponentField[1].id, "ID_OPPONENT-2");
            assert.equal(opponentField[1].isSilenced, 1);

            resolve();
          }
        );
      });
    });

    it("should support condemn", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-7",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-5",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-7");
            assert.equal(lastMoves[0].attributes.card.name, "Condemn");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-5");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-5");
            assert.equal(opponentField[0].isSilenced, 1);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 50);

            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
              "health": 50,
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
            },
            {
              "id": "ID_PLAYER-3",
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
              "buffsField": [],
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
          "hand": [
            {
              "id": "ID_PLAYER-5",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 10,
              "name": "Bestowed Vigor",
              "description": "Give a friendly creature +20/+10",
              "baseId": "C24",
              "costStart": 10,
            },
            {
              "id": "ID_PLAYER-6",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 10,
              "name": "Unstable Power",
              "description": "Give a friendly creature +30/+0, it dies next turn",
              "baseId": "C24",
              "costStart": 10,
            },
            {
              "id": "ID_PLAYER-7",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
          ],
          "deckSize": 1,
          "cardCount": 8,
          "mode": 0,
          "mulliganCards": [],
          "id": "5b0b017502bd4e052f08a28d",
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

    it("should support bestowed vigor", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-5",
            attributesJson: {
              fieldId: "ID_PLAYER",
              targetId: "ID_PLAYER-2",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-5");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.targetId, "ID_PLAYER-2");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "ID_PLAYER-2");
            assert.equal(playerField[1].attack, 50);
            assert.equal(playerField[1].attackStart, 30);
            assert.equal(playerField[1].health, 60);
            assert.equal(playerField[1].healthMax, 60);
            assert.equal(playerField[1].healthStart, 50);

            gamesparks.sendWithData(
              "LogEventRequest",
              {
                eventKey: "TestChallengePlaySpellUntargeted",
                challengeStateString: JSON.stringify(challengeStateData),
                challengePlayerId: "ID_PLAYER",
                cardId: "ID_PLAYER-7",
              },
              function(response) {
                const challengeStateData = response.scriptData.challengeStateData;

                const lastMoves = challengeStateData.lastMoves;
                assert.equal(lastMoves.length, 1);
                assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
                assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-7");
                assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-7");

                const playerState = challengeStateData.current["ID_PLAYER"];
                const playerField = playerState.field;

                assert.equal(playerField[1].id, "ID_PLAYER-2");
                assert.equal(playerField[1].isSilenced, 1);
                assert.equal(playerField[1].attack, 30);
                assert.equal(playerField[1].attackStart, 30);
                assert.equal(playerField[1].health, 50);
                assert.equal(playerField[1].healthMax, 50);
                assert.equal(playerField[1].healthStart, 50);
                assert.equal(playerField[1].buffsField.indexOf(1002) >= 0, true);

                resolve();
              }
            );
          }
        );
      });
    });

    it("should support unstable power", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellTargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-6",
            attributesJson: {
              fieldId: "ID_PLAYER",
              targetId: "ID_PLAYER-3",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-6");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-6");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.targetId, "ID_PLAYER-3");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[2].id, "ID_PLAYER-3");
            assert.equal(playerField[2].attack, 60);
            assert.equal(playerField[2].attackStart, 30);
            assert.equal(playerField[2].health, 30);
            assert.equal(playerField[2].healthMax, 50);
            assert.equal(playerField[2].healthStart, 50);

            gamesparks.sendWithData(
              "LogEventRequest",
              {
                eventKey: "TestChallengePlaySpellUntargeted",
                challengeStateString: JSON.stringify(challengeStateData),
                challengePlayerId: "ID_PLAYER",
                cardId: "ID_PLAYER-7",
              },
              function(response) {
                const challengeStateData = response.scriptData.challengeStateData;

                const lastMoves = challengeStateData.lastMoves;
                assert.equal(lastMoves.length, 1);
                assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
                assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-7");
                assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-7");

                const playerState = challengeStateData.current["ID_PLAYER"];
                const playerField = playerState.field;

                assert.equal(playerField[2].id, "ID_PLAYER-3");
                assert.equal(playerField[2].isSilenced, 1);
                assert.equal(playerField[2].attack, 30);
                assert.equal(playerField[2].attackStart, 30);
                assert.equal(playerField[2].health, 30);
                assert.equal(playerField[2].healthMax, 50);
                assert.equal(playerField[2].healthStart, 50);
                assert.equal(playerField[2].buffsField.indexOf(1001) >= 0, true);

                resolve();
              }
            );
          }
        );
      });
    });
  });

  describe("spells", function() {
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
              "id": "C10-ID_OPPONENT-5",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
              "cost": 20,
              "name": "Firebug Catelyn",
              "description": "Deathrattle: Damage opponent face by 10 dmg",
              "abilities": [
                10,
              ],
              "baseId": "C10",
              "attackStart": 20,
              "costStart": 20,
              "healthStart": 30,
              "healthMax": 30,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 2
            },
            {
              "id": "C2-ID_OPPONENT-2",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 30,
              "health": 30,
              "cost": 40,
              "name": "Waterborne Razorback",
              "description": "Charge; At the end of each turn, recover 20 health",
              "abilities": [],
              "baseId": "C2",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
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
              "id": "C2-ID_PLAYER-2",
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
            },
            {
              "id": "C3-ID_PLAYER-5",
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
              "baseId": "C3",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 0
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
              "id": "C23-ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 40,
              "name": "Greedy Fingers",
              "description": "Draw three cards",
              "baseId": "C23",
              "costStart": 40,
            },
          ],
          "deckSize": 1,
          "deck": [
            {
              "id": "C24-ID_PLAYER-5",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 30,
              "name": "Mudslinging",
              "description": "Give all friendly creatures taunt",
              "baseId": "C24",
              "costStart": 30,
            },
            {
              "id": "C25-ID_PLAYER-6",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
            {
              "id": "C26-ID_PLAYER-7",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 40,
              "name": "Silence of the Lambs",
              "description": "Silence all creatures",
              "baseId": "C25",
              "costStart": 40,
            },
          ],
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

    it("should support greedy fingers", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C23-ID_PLAYER-4",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 4);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].attributes.cardId, "C23-ID_PLAYER-4");
            assert.equal(lastMoves[0].attributes.card.name, "Greedy Fingers");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_DRAW_CARD");
            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_DRAW_CARD");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerHand = playerState.hand;
            assert.equal(playerHand.length, 3);

            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
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
          "health": 100,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "EMPTY"
            },
            {
              "id": "C2-ID_PLAYER-2",
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
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 1
            },
            {
              "id": "C3-ID_PLAYER-5",
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
              "baseId": "C3",
              "attackStart": 30,
              "costStart": 40,
              "healthStart": 50,
              "healthMax": 50,
              "buffsField": [],
              "canAttack": 1,
              "isFrozen": 0,
              "spawnRank": 0
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
              "id": "C23-ID_PLAYER-4",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "cost": 20,
              "name": "Bombs Away",
              "description": "Deal 10 dmg to three random opponent targetables",
              "baseId": "C23",
              "costStart": 40,
            },
          ],
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

    it("should support bombs away", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C23-ID_PLAYER-4",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "C23-ID_PLAYER-4");
            assert.equal(lastMoves[2].attributes.card.id, "C23-ID_PLAYER-4");
            assert.equal(lastMoves[3].attributes.card.id, "C23-ID_PLAYER-4");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.healthMax, 100);
            assert.equal(opponentState.health, 70);

            resolve();
          }
        );
      });
    });
  });

  describe("spells", function() {
    it("should support battle royale", function() {
      const challengeStateData = {
        "current": {
          "ID_OPPONENT": {
            "hasTurn": 0,
            "manaCurrent": 90,
            "manaMax": 90,
            "health": 100,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "EMPTY"
              },
              {
                "id": "ID_OPPONENT-0",
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
                "id": "EMPTY"
              },
              {
                "id": "ID_OPPONENT-1",
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
                "spawnRank": 2
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
            "cardCount": 33,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0
          },
          "ID_PLAYER": {
            "hasTurn": 0,
            "manaCurrent": 90,
            "manaMax": 90,
            "health": 60,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "EMPTY"
              },
              {
                "id": "ID_PLAYER-18",
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
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 3
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "ID_PLAYER-19",
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
              },
            ],
            "hand": [
              {
                "id": "ID_PLAYER-0",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 1,
                "attack": null,
                "health": null,
                "cost": 50,
                "name": "Battle Royale",
                "description": "Destroy all creatures except one random one",
                "abilities": [],
                "baseId": "C28",
                "attackStart": null,
                "costStart": 50,
                "healthStart": null,
                "healthMax": null
              },
            ],
            "deckSize": 0,
            "cardCount": 32,
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
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-0",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-0");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-0");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.targetId, "ID_PLAYER-18");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 90);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "ID_PLAYER-18");
            assert.equal(playerField[3].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "EMPTY");
            assert.equal(opponentField[3].id, "EMPTY");

            const deadCards = challengeStateData.deadCards;
            assert.equal(deadCards.length, 3);

            assert.equal(deadCards[0].id, "ID_OPPONENT-0");
            assert.equal(deadCards[0].spawnRank, 1);
            assert.equal(deadCards[1].id, "ID_OPPONENT-1");
            assert.equal(deadCards[1].spawnRank, 2);
            assert.equal(deadCards[2].id, "ID_PLAYER-19");
            assert.equal(deadCards[2].spawnRank, 4);

            resolve();
          }
        );
      });
    });

    it("should support battle royale with correct deathwish order", function() {
      const challengeStateData = {
        "current": {
          "ID_OPPONENT": {
            "hasTurn": 0,
            "manaCurrent": 90,
            "manaMax": 90,
            "health": 100,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "EMPTY"
              },
              {
                "id": "ID_OPPONENT-0",
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
                "buffsField": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 6
              },
              {
                "id": "ID_OPPONENT-1",
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
                "spawnRank": 2
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
            "cardCount": 33,
            "mode": 0,
            "mulliganCards": [],
            "id": "ID_OPPONENT",
            "expiredStreak": 0
          },
          "ID_PLAYER": {
            "hasTurn": 0,
            "manaCurrent": 90,
            "manaMax": 90,
            "health": 60,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "EMPTY"
              },
              {
                "id": "ID_PLAYER-18",
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
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 3
              },
              {
                "id": "ID_PLAYER-6",
                "playerId": "ID_PLAYER",
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
                "buffsField": [],
                "canAttack": 1,
                "isFrozen": 0,
                "spawnRank": 7
              },
              {
                "id": "ID_PLAYER-19",
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
              },
            ],
            "hand": [
              {
                "id": "ID_PLAYER-0",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 1,
                "attack": null,
                "health": null,
                "cost": 50,
                "name": "Battle Royale",
                "description": "Destroy all creatures except one random one",
                "abilities": [],
                "baseId": "C28",
                "attackStart": null,
                "costStart": 50,
                "healthStart": null,
                "healthMax": null
              },
            ],
            "deckSize": 0,
            "cardCount": 32,
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
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-0",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 8);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-0");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-0");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.targetId, "ID_PLAYER-18");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-5");
            assert.equal(lastMoves[2].attributes.fieldId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.targetId, "ID_PLAYER-18");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[3].attributes.card.id, "ID_OPPONENT-5");
            assert.equal(lastMoves[3].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[4].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[4].attributes.card.id, "ID_OPPONENT-5");
            assert.equal(lastMoves[4].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[5].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[5].playerId, "ID_PLAYER");
            assert.equal(lastMoves[5].attributes.card.id, "ID_PLAYER-6");
            assert.equal(lastMoves[5].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[6].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[6].playerId, "ID_PLAYER");
            assert.equal(lastMoves[6].attributes.card.id, "ID_PLAYER-6");
            assert.equal(lastMoves[6].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[7].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[7].playerId, "ID_PLAYER");
            assert.equal(lastMoves[7].attributes.card.id, "ID_PLAYER-6");
            assert.equal(lastMoves[7].attributes.targetId, "TARGET_ID_FACE");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 90);
            assert.equal(playerState.health, 40);

            const playerField = playerState.field;
            assert.equal(playerField[1].id, "EMPTY");
            assert.equal(playerField[2].id, "EMPTY");
            assert.equal(playerField[3].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 70);

            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "EMPTY");
            assert.equal(opponentField[2].id, "EMPTY");
            assert.equal(opponentField[3].id, "EMPTY");

            // Note that the presence of deathwishes result in dead order not matching spawn rank completely.
            const deadCards = challengeStateData.deadCards;
            assert.equal(deadCards.length, 6);

            assert.equal(deadCards[0].id, "ID_OPPONENT-0");
            assert.equal(deadCards[0].spawnRank, 1);

            assert.equal(deadCards[1].id, "ID_OPPONENT-1");
            assert.equal(deadCards[1].spawnRank, 2);

            assert.equal(deadCards[2].id, "ID_PLAYER-19");
            assert.equal(deadCards[2].spawnRank, 4);

            assert.equal(deadCards[3].id, "ID_PLAYER-18");
            assert.equal(deadCards[3].spawnRank, 3);

            assert.equal(deadCards[4].id, "ID_OPPONENT-5");
            assert.equal(deadCards[4].spawnRank, 6);

            assert.equal(deadCards[5].id, "ID_PLAYER-6");
            assert.equal(deadCards[5].spawnRank, 7);

            resolve();
          }
        );
      });
    });
  });
});
