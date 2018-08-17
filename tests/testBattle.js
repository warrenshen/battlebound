const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge general", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("lethal and lifesteal", function() {
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
              "id": "C12-ID_OPPONENT-8",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 30,
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
              "id": "C49-ID_OPPONENT-13",
              "playerId": "ID_OPPONENT",
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
          "health": 60,
          "healthMax": 100,
          "armor": 0,
          "field": [
            {
              "id": "C46-ID_PLAYER-16",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 50,
              "health": 40,
              "cost": 50,
              "name": "Cereboarus",
              "description": "Lifesteal; Deathwish: Draw a card",
              "abilities": [
                5,
                6
              ],
              "baseId": "C46",
              "attackStart": 50,
              "costStart": 50,
              "healthStart": 40,
              "healthMax": 40,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 5,
            },
            {
              "id": "C49-ID_PLAYER-13",
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

    it("should support lethal on attack", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C49-ID_PLAYER-13",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C12-ID_OPPONENT-8",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C49-ID_PLAYER-13");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C12-ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C49-ID_PLAYER-13");
            assert.equal(playerField[1].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            const deadCards = challengeStateData.deadCards;
            assert.equal(deadCards.length, 1);

            assert.equal(deadCards[0].id, "C12-ID_OPPONENT-8");
            assert.equal(deadCards[0].spawnRank, 1);

            resolve();
          }
        );
      });
    });

    it("should support lethal on defend", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C46-ID_PLAYER-16",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C49-ID_OPPONENT-13",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C46-ID_PLAYER-16");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C49-ID_OPPONENT-13");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_DRAW_CARD_DECK_EMPTY");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "EMPTY");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support lifesteal", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C46-ID_PLAYER-16",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C12-ID_OPPONENT-8",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C46-ID_PLAYER-16");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C12-ID_OPPONENT-8");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.health, 90);
            assert.equal(playerState.healthMax, 100);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C46-ID_PLAYER-16");
            assert.equal(playerField[0].health, 20);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("attack adjacent", function() {
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
              "id": "C40-ID_OPPONENT-22",
              "playerId": "ID_OPPONENT",
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
              "spawnRank": 0
            },
            {
              "id": "C40-ID_OPPONENT-23",
              "playerId": "ID_OPPONENT",
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
              "spawnRank": 1
            },
            {
              "id": "C40-ID_OPPONENT-24",
              "playerId": "ID_OPPONENT",
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
              "id": "C34-ID_PLAYER-28",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 30,
              "cost": 30,
              "name": "Redhaired Paladin",
              "description": "Deal 10 dmg to adjacent creatures of targeted creature on attack",
              "abilities": [
                22
              ],
              "baseId": "C34",
              "attackStart": 40,
              "costStart": 30,
              "healthStart": 30,
              "healthMax": 30,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 3
            },
            {
              "id": "C35-ID_PLAYER-27",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 50,
              "health": 70,
              "cost": 70,
              "name": "Firesworn Godblade",
              "description": "Deal dmg to adjacent creatures of targeted creature on attack",
              "abilities": [
                23
              ],
              "baseId": "C35",
              "attackStart": 50,
              "costStart": 70,
              "healthStart": 70,
              "healthMax": 70,
              "buffs": [],
              "canAttack": 1,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 4,
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

    it("should support attack damage adjacent by 10 damage", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C34-ID_PLAYER-28",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-23",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C34-ID_PLAYER-28");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-23");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C34-ID_PLAYER-28");
            assert.equal(playerField[0].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C40-ID_OPPONENT-22");
            assert.equal(opponentField[0].health, 20);

            assert.equal(opponentField[1].id, "EMPTY");

            assert.equal(opponentField[2].id, "C40-ID_OPPONENT-24");
            assert.equal(opponentField[2].health, 20);

            resolve();
          }
        );
      });
    });

    it("should support attack damage adjacent by 10 damage", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C34-ID_PLAYER-28",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C34-ID_PLAYER-28");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-24");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "C34-ID_PLAYER-28");
            assert.equal(playerField[0].health, 10);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C40-ID_OPPONENT-22");
            assert.equal(opponentField[0].health, 30);

            assert.equal(opponentField[1].id, "C40-ID_OPPONENT-23");
            assert.equal(opponentField[1].health, 20);

            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support attack damage adjacent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C35-ID_PLAYER-27",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-23",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C35-ID_PLAYER-27");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-23");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C35-ID_PLAYER-27");
            assert.equal(playerField[1].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "EMPTY");
            assert.equal(opponentField[1].id, "EMPTY");
            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support attack damage adjacent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "C35-ID_PLAYER-27",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C40-ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "C35-ID_PLAYER-27");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "C40-ID_OPPONENT-24");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[1].id, "C35-ID_PLAYER-27");
            assert.equal(playerField[1].health, 50);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[0].id, "C40-ID_OPPONENT-22");
            assert.equal(opponentField[0].health, 30);

            assert.equal(opponentField[1].id, "EMPTY");
            assert.equal(opponentField[2].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("attack icy", function() {
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
              "id": "ID_OPPONENT-9",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 60,
              "name": "Temple Guardian",
              "description": "Charge; Shield",
              "abilities": [
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
              "isSilenced": 0,
              "spawnRank": 0
            },
            {
              "id": "ID_OPPONENT-25",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 80,
              "cost": 70,
              "name": "Sabre, Crystalline Dragon",
              "description": "Freeze enemy in combat",
              "abilities": [
                42
              ],
              "baseId": "C36",
              "attackStart": 40,
              "costStart": 70,
              "healthStart": 80,
              "healthMax": 80,
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
              "id": "ID_PLAYER-28",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 80,
              "cost": 70,
              "name": "Sabre, Crystalline Dragon",
              "description": "Freeze enemy in combat",
              "abilities": [
                42
              ],
              "baseId": "C34",
              "attackStart": 40,
              "costStart": 70,
              "healthStart": 80,
              "healthMax": 80,
              "buffs": [],
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
      "expiredStreakByPlayerId": {
        "ID_PLAYER": 0,
        "ID_OPPONENT": 0,
      },
      "spawnCount": 2,
      "deathCount": 0,
      "nonce": 14
    };

    it("should support freeze enemy on attack", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-28",
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
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-28");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-9");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-28");
            assert.equal(playerField[0].health, 40);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-9");
            assert.equal(opponentField[0].health, 60);
            assert.equal(opponentField[0].isFrozen, 1);

            resolve();
          }
        );
      });
    });

    it("should support freeze enemy on defend", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-28",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-25",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-28");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-25");

            const playerState = challengeStateData.current["ID_PLAYER"];
            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-28");
            assert.equal(playerField[0].health, 40);
            assert.equal(playerField[0].isFrozen, 2);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[1].id, "ID_OPPONENT-25");
            assert.equal(opponentField[1].health, 40);
            assert.equal(opponentField[1].isFrozen, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("silenced", function() {
    it("should let damage through shield", function() {
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
                "id": "C9-ID_OPPONENT-9",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 40,
                "health": 60,
                "cost": 60,
                "name": "Temple Guardian",
                "description": "Charge; Shield",
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
                "isSilenced": 1,
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
                "id": "EMPTY"
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "EMPTY"
              },
              {
                "id": "C7-ID_PLAYER-7",
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
                "id": "C4-ID_PLAYER-4",
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
            cardId: "C4-ID_PLAYER-4",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "C9-ID_OPPONENT-9",
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
            assert.equal(opponentField[2].id, "C9-ID_OPPONENT-9");
            assert.equal(opponentField[2].abilities.indexOf(2) >= 0, true);
            assert.equal(opponentField[2].abilities.indexOf(1) >= 0, true);
            assert.equal(opponentField[2].isSilenced, 1);

            resolve();
          }
        );
      });
    });
  });
});
