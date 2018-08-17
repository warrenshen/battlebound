const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge revive", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("revive", function() {
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
          "cardCount": 30,
          "mode": 0,
          "mulliganCards": [],
          "id": "ID_OPPONENT",
          "expiredStreak": 0
        },
        "ID_PLAYER": {
          "hasTurn": 1,
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
              "id": "ID_PLAYER-10",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
            },
            {
              "id": "ID_PLAYER-15",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 40,
              "name": "Grave-digging",
              "description": "Bring back your last creature",
              "abilities": [],
              "baseId": "C28",
              "attackStart": null,
              "costStart": 40,
              "healthStart": null,
              "healthMax": null
            },
          ],
          "deckSize": 0,
          "cardCount": 34,
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
      "deadCards": [
        {
          "id": "ID_OPPONENT-0",
          "level": 1,
          "category": 0,
          "attack": 10,
          "health": -10,
          "cost": 20,
          "name": "Blessed Newborn",
          "description": "Battlecry: Draw a card",
          "abilities": [
            4
          ],
          "baseId": "C3",
          "playerId": "ID_OPPONENT",
          "attackStart": 10,
          "costStart": 20,
          "healthStart": 10,
          "healthMax": 10,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 0,
          "deathRank": 0
        },
        {
          "id": "ID_OPPONENT-15",
          "level": 1,
          "category": 0,
          "attack": 40,
          "health": 0,
          "cost": 20,
          "name": "Ritual Hatchling",
          "description": "Warcry: sacrifice 20 dmg from your life total",
          "abilities": [
            24
          ],
          "baseId": "C36",
          "playerId": "ID_OPPONENT",
          "attackStart": 40,
          "costStart": 20,
          "healthStart": 30,
          "healthMax": 30,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 4,
          "deathRank": 1
        },
        {
          "id": "ID_PLAYER-17",
          "level": 1,
          "category": 0,
          "attack": 20,
          "health": 0,
          "cost": 30,
          "name": "Hoofed Lush",
          "description": "Taunt; Warcry: Draw a card",
          "abilities": [
            1,
            4
          ],
          "baseId": "C38",
          "playerId": "ID_PLAYER",
          "attackStart": 20,
          "costStart": 30,
          "healthStart": 30,
          "healthMax": 30,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 5,
          "deathRank": 2
        },
        {
          "id": "ID_OPPONENT-22",
          "level": 1,
          "category": 0,
          "attack": 40,
          "health": -20,
          "cost": 30,
          "name": "Redhaired Paladin",
          "description": "Deal 10 dmg to adjacent creatures of targeted creature on attack",
          "abilities": [
            22
          ],
          "baseId": "C34",
          "playerId": "ID_OPPONENT",
          "attackStart": 40,
          "costStart": 30,
          "healthStart": 30,
          "healthMax": 30,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 3,
          "deathRank": 3
        },
        {
          "id": "ID_PLAYER-9",
          "level": 1,
          "category": 0,
          "attack": 60,
          "health": 0,
          "cost": 40,
          "name": "Cereboarus",
          "description": "Lifesteal; Deathwish: Draw a card",
          "abilities": [
            1,
            5,
            6
          ],
          "abilitiesStart": [
            5,
            6
          ],
          "baseId": "C46",
          "playerId": "ID_PLAYER",
          "attackStart": 50,
          "costStart": 50,
          "healthStart": 40,
          "healthMax": 40,
          "buffsField": [1001],
          "buffsField": [2000],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 8,
          "deathRank": 4
        },
        {
          "id": "ID_PLAYER-25",
          "level": 1,
          "category": 0,
          "attack": 20,
          "health": 0,
          "cost": 40,
          "name": "Swift Shellback",
          "description": "Charge",
          "abilities": [
            0
          ],
          "baseId": "C30",
          "playerId": "ID_PLAYER",
          "attackStart": 20,
          "costStart": 40,
          "healthStart": 50,
          "healthMax": 50,
          "buffsField": [],
          "canAttack": 1,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 2,
          "deathRank": 5
        },
      ],
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

    it("should support revive highest cost dead creature", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-10",
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
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-10");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-34");
            assert.equal(lastMoves[1].attributes.card.name, "Cereboarus");
            assert.equal(lastMoves[1].attributes.card.cost, 50);
            assert.equal(lastMoves[1].attributes.card.attack, 50);
            assert.equal(lastMoves[1].attributes.card.health, 40);
            assert.equal(lastMoves[1].attributes.card.canAttack, 0);
            assert.equal(lastMoves[1].attributes.card.isFrozen, 0);
            assert.equal(lastMoves[1].attributes.card.isSilenced, 0);
            assert.equal(lastMoves[1].attributes.card.abilities.indexOf(5) >= 0, true);
            assert.equal(lastMoves[1].attributes.card.abilities.indexOf(6) >= 0, true);
            assert.equal(lastMoves[1].attributes.card.abilities.indexOf(1) >= 0, false);
            assert.equal(lastMoves[1].attributes.card.buffsField.length, 0);
            assert.equal(lastMoves[1].attributes.card.buffsHand.length, 0);
            assert.equal(lastMoves[1].attributes.fieldIndex >= 0, true);
            assert.equal(lastMoves[1].attributes.fieldIndex <= 5, true);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 10);
            assert.equal(playerState.manaMax, 90);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_PLAYER-10");
            assert.equal(playerField.filter(function(fieldCard) { return fieldCard.id != "EMPTY" }).length, 2);

            resolve();
          }
        );
      });
    });

    it("should support grave-digging", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-15",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-15");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-34");
            assert.equal(lastMoves[1].attributes.card.name, "Swift Shellback");
            assert.equal(lastMoves[1].attributes.fieldIndex >= 0, true);
            assert.equal(lastMoves[1].attributes.fieldIndex <= 5, true);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 50);
            assert.equal(playerState.manaMax, 90);

            const playerField = playerState.field;
            assert.equal(playerField.filter(function(fieldCard) { return fieldCard.id != "EMPTY" }).length, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("revive - switch players", function() {
    const challengeStateData = {
      "current": {
        "ID_OPPONENT": {
          "hasTurn": 1,
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
              "id": "ID_OPPONENT-10",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
            },
            {
              "id": "ID_OPPONENT-15",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 40,
              "name": "Grave-digging",
              "description": "Bring back your last creature",
              "abilities": [],
              "baseId": "C28",
              "attackStart": null,
              "costStart": 40,
              "healthStart": null,
              "healthMax": null
            },
          ],
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
              "id": "C45-ID_PLAYER-10",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 60,
              "cost": 80,
              "name": "Thunderous Desperado",
              "description": "Warcry: Revive your highest cost fallen creature",
              "abilities": [
                29
              ],
              "baseId": "C45",
              "attackStart": 40,
              "costStart": 80,
              "healthStart": 60,
              "healthMax": 60,
            },
            {
              "id": "C28-ID_PLAYER-15",
              "playerId": "ID_PLAYER",
              "level": 1,
              "category": 1,
              "attack": null,
              "health": null,
              "cost": 40,
              "name": "Grave-digging",
              "description": "Bring back your last creature",
              "abilities": [],
              "baseId": "C28",
              "attackStart": null,
              "costStart": 40,
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
      "deadCards": [
        {
          "id": "ID_OPPONENT-0",
          "level": 1,
          "category": 0,
          "attack": 10,
          "health": -10,
          "cost": 20,
          "name": "Blessed Newborn",
          "description": "Battlecry: Draw a card",
          "abilities": [
            4
          ],
          "baseId": "C3",
          "playerId": "ID_OPPONENT",
          "attackStart": 10,
          "costStart": 20,
          "healthStart": 10,
          "healthMax": 10,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 0,
          "deathRank": 0
        },
        {
          "id": "ID_OPPONENT-15",
          "level": 1,
          "category": 0,
          "attack": 40,
          "health": 0,
          "cost": 20,
          "name": "Ritual Hatchling",
          "description": "Warcry: sacrifice 20 dmg from your life total",
          "abilities": [
            24
          ],
          "baseId": "C36",
          "playerId": "ID_OPPONENT",
          "attackStart": 40,
          "costStart": 20,
          "healthStart": 30,
          "healthMax": 30,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 4,
          "deathRank": 1
        },
        {
          "id": "C38-ID_PLAYER-17",
          "level": 1,
          "category": 0,
          "attack": 20,
          "health": 0,
          "cost": 30,
          "name": "Hoofed Lush",
          "description": "Taunt; Warcry: Draw a card",
          "abilities": [
            1,
            4
          ],
          "baseId": "C38",
          "playerId": "ID_PLAYER",
          "attackStart": 20,
          "costStart": 30,
          "healthStart": 30,
          "healthMax": 30,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 5,
          "deathRank": 2
        },
        {
          "id": "ID_OPPONENT-22",
          "level": 1,
          "category": 0,
          "attack": 40,
          "health": -20,
          "cost": 30,
          "name": "Redhaired Paladin",
          "description": "Deal 10 dmg to adjacent creatures of targeted creature on attack",
          "abilities": [
            22
          ],
          "baseId": "C34",
          "playerId": "ID_OPPONENT",
          "attackStart": 40,
          "costStart": 30,
          "healthStart": 30,
          "healthMax": 30,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 3,
          "deathRank": 3
        },
        {
          "id": "C46-ID_PLAYER-9",
          "level": 1,
          "category": 0,
          "attack": 50,
          "health": 0,
          "cost": 50,
          "name": "Cereboarus",
          "description": "Lifesteal; Deathwish: Draw a card",
          "abilities": [
            5,
            6
          ],
          "baseId": "C46",
          "playerId": "ID_PLAYER",
          "attackStart": 50,
          "costStart": 50,
          "healthStart": 40,
          "healthMax": 40,
          "buffsField": [],
          "canAttack": 0,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 8,
          "deathRank": 4
        },
        {
          "id": "C30-ID_PLAYER-25",
          "level": 1,
          "category": 0,
          "attack": 20,
          "health": 0,
          "cost": 40,
          "name": "Swift Shellback",
          "description": "Charge",
          "abilities": [
            0
          ],
          "baseId": "C30",
          "playerId": "ID_PLAYER",
          "attackStart": 20,
          "costStart": 40,
          "healthStart": 50,
          "healthMax": 50,
          "buffsField": [],
          "canAttack": 1,
          "isFrozen": 0,
          "isSilenced": 0,
          "spawnRank": 2,
          "deathRank": 5
        },
      ],
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

    it("should support revive highest cost dead creature", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlayCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardId: "ID_OPPONENT-10",
            attributesJson: {
              fieldIndex: 0,
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_MINION");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.card.id, "ID_OPPONENT-10");
            assert.equal(lastMoves[0].attributes.fieldIndex, 0);

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Redhaired Paladin");
            assert.equal(lastMoves[1].attributes.fieldIndex >= 0, true);
            assert.equal(lastMoves[1].attributes.fieldIndex <= 5, true);

            const playerState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(playerState.manaCurrent, 10);
            assert.equal(playerState.manaMax, 90);

            const playerField = playerState.field;
            assert.equal(playerField[0].id, "ID_OPPONENT-10");
            assert.equal(playerField.filter(function(fieldCard) { return fieldCard.id != "EMPTY" }).length, 2);

            resolve();
          }
        );
      });
    });

    it("should support grave-digging", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengePlaySpellUntargeted",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            cardId: "ID_OPPONENT-15",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.card.id, "ID_OPPONENT-15");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Redhaired Paladin");
            assert.equal(lastMoves[1].attributes.fieldIndex >= 0, true);
            assert.equal(lastMoves[1].attributes.fieldIndex <= 5, true);

            const playerState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(playerState.manaCurrent, 50);
            assert.equal(playerState.manaMax, 90);

            const playerField = playerState.field;
            assert.equal(playerField.filter(function(fieldCard) { return fieldCard.id != "EMPTY" }).length, 1);

            resolve();
          }
        );
      });
    });
  });

  describe("convert", function() {
    it("should support the seven", function() {
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
                "id": "ID_OPPONENT-24",
                "playerId": "ID_OPPONENT",
                "level": 1,
                "category": 0,
                "attack": 10,
                "health": 10,
                "cost": 10,
                "name": "Firebug Catelyn",
                "description": "Deathwish: damage opponent face by 10",
                "abilities": [
                  10
                ],
                "baseId": "C40",
                "attackStart": 20,
                "costStart": 30,
                "healthStart": 30,
                "healthMax": 30,
                "buffsField": [],
                "canAttack": 0,
                "isFrozen": 1,
                "isSilenced": 1,
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
                "id": "ID_PLAYER-15",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 1,
                "attack": null,
                "health": null,
                "cost": 50,
                "name": "The Seven",
                "description": "Convert an enemy creature to your side",
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
            cardId: "ID_PLAYER-15",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.spawnCount, 2);

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-15");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_CONVERT_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.targetId, "ID_OPPONENT-24");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-32");
            assert.equal(lastMoves[2].attributes.card.name, "Firebug Catelyn");
            assert.equal(lastMoves[2].attributes.fieldIndex, 4);

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 90);

            const playerField = playerState.field;
            assert.equal(playerField[4].id, "ID_PLAYER-32");
            assert.equal(playerField[4].cost, 10);
            assert.equal(playerField[4].attack, 10);
            assert.equal(playerField[4].health, 10);
            assert.equal(playerField[4].isFrozen, 1);
            assert.equal(playerField[4].isSilenced, 1);

            // Converted creatures deathwish should not run.
            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 100);
            assert.equal(opponentState.healthMax, 100);

            // Converted creatures do not die.
            const deadCards = challengeStateData.deadCards;
            assert.equal(deadCards.length, 0);

            resolve();
          }
        );
      });
    });

    it("should support the seven - no opponent creatures", function() {
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
                "id": "ID_PLAYER-15",
                "playerId": "ID_PLAYER",
                "level": 1,
                "category": 1,
                "attack": null,
                "health": null,
                "cost": 50,
                "name": "The Seven",
                "description": "Convert an enemy creature to your side",
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
            cardId: "ID_PLAYER-15",
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;
            assert.equal(challengeStateData.spawnCount, 2);

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_UNTARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.card.id, "ID_PLAYER-15");

            const playerState = challengeStateData.current["ID_PLAYER"];
            assert.equal(playerState.manaCurrent, 40);
            assert.equal(playerState.manaMax, 90);

            resolve();
          }
        );
      });
    });
  });
});
