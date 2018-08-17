const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge phantom skullcrusher", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("deathwish summons", function() {
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
              "id": "ID_OPPONENT-24",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Dusk Dweller",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                34
              ],
              "abilitiesStart": [
                34
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 10,
              "healthMax": 10,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 0
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_OPPONENT-25",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 10,
              "cost": 60,
              "name": "Talusreaver",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                35
              ],
              "abilitiesStart": [
                35
              ],
              "baseId": "C40",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 50,
              "healthMax": 50,
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
              "id": "ID_OPPONENT-26",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 60,
              "health": 10,
              "cost": 80,
              "name": "Phantom Skullcrusher",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                36
              ],
              "abilitiesStart": [
                36
              ],
              "baseId": "C40",
              "attackStart": 60,
              "costStart": 100,
              "healthStart": 80,
              "healthMax": 80,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
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

    it("should support dusk dweller", function() {
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
              targetId: "ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 2);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-24");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[1].attributes.fieldIndex, 0);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-33");
            assert.equal(opponentField[0].cost, 30);
            assert.equal(opponentField[0].attack, 20);
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[0].abilities.indexOf(34) >= 0, true);
            assert.equal(opponentField[0].color, 4);

            resolve();
          }
        );
      });
    });

    it("should support talusreaver", function() {
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
              targetId: "ID_OPPONENT-25",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-25");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[1].attributes.fieldIndex, 2);

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-34");
            assert.equal(lastMoves[2].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[2].attributes.fieldIndex, 3);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[2].id, "ID_OPPONENT-33");
            assert.equal(opponentField[2].name, "Dusk Dweller");
            assert.equal(opponentField[2].cost, 30);
            assert.equal(opponentField[2].attack, 20);
            assert.equal(opponentField[2].health, 10);
            assert.equal(opponentField[2].abilities.indexOf(34) >= 0, true);
            assert.equal(opponentField[2].color, 4);

            assert.equal(opponentField[3].id, "ID_OPPONENT-34");
            assert.equal(opponentField[3].name, "Dusk Dweller");
            assert.equal(opponentField[3].cost, 30);
            assert.equal(opponentField[3].attack, 20);
            assert.equal(opponentField[3].health, 10);
            assert.equal(opponentField[3].abilities.indexOf(34) >= 0, true);
            assert.equal(opponentField[3].color, 4);

            resolve();
          }
        );
      });
    });

    it("should support phantom skullcrusher", function() {
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
              targetId: "ID_OPPONENT-26",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-26");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Talusreaver");
            assert.equal(lastMoves[1].attributes.fieldIndex, 4);

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-34");
            assert.equal(lastMoves[2].attributes.card.name, "Talusreaver");
            assert.equal(lastMoves[2].attributes.fieldIndex, 5);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[4].id, "ID_OPPONENT-33");
            assert.equal(opponentField[4].name, "Talusreaver");
            assert.equal(opponentField[4].cost, 60);
            assert.equal(opponentField[4].attack, 40);
            assert.equal(opponentField[4].health, 50);
            assert.equal(opponentField[4].abilities.indexOf(35) >= 0, true);
            assert.equal(opponentField[4].color, 4);

            assert.equal(opponentField[5].id, "ID_OPPONENT-34");
            assert.equal(opponentField[5].name, "Talusreaver");
            assert.equal(opponentField[5].cost, 60);
            assert.equal(opponentField[5].attack, 40);
            assert.equal(opponentField[5].health, 50);
            assert.equal(opponentField[5].abilities.indexOf(35) >= 0, true);
            assert.equal(opponentField[5].color, 4);

            resolve();
          }
        );
      });
    });
  });

  describe("deathwish summons - field full", function() {
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
              "id": "ID_OPPONENT-24",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Dusk Dweller",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                34
              ],
              "abilitiesStart": [
                34
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 10,
              "healthMax": 10,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 2
            },
            {
              "id": "NOT_EMPTY"
            },
            {
              "id": "ID_OPPONENT-25",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 10,
              "cost": 60,
              "name": "Talusreaver",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                35
              ],
              "abilitiesStart": [
                35
              ],
              "baseId": "C40",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 3
            },
            {
              "id": "NOT_EMPTY"
            },
            {
              "id": "ID_OPPONENT-26",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 60,
              "health": 10,
              "cost": 80,
              "name": "Phantom Skullcrusher",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                36
              ],
              "abilitiesStart": [
                36
              ],
              "baseId": "C40",
              "attackStart": 60,
              "costStart": 100,
              "healthStart": 80,
              "healthMax": 80,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 0,
              "spawnRank": 4
            },
            {
              "id": "NOT_EMPTY"
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

    it("should support talusreaver", function() {
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
              targetId: "ID_OPPONENT-25",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-25");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[1].attributes.fieldIndex, 2);

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-34");
            assert.equal(lastMoves[2].attributes.card.name, "Dusk Dweller");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[2].id, "ID_OPPONENT-33");
            assert.equal(opponentField[2].name, "Dusk Dweller");
            assert.equal(opponentField[2].cost, 30);
            assert.equal(opponentField[2].attack, 20);
            assert.equal(opponentField[2].health, 10);
            assert.equal(opponentField[2].abilities.indexOf(34) >= 0, true);
            assert.equal(opponentField[2].color, 4);

            assert.equal(opponentField[3].id, "NOT_EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support phantom skullcrusher", function() {
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
              targetId: "ID_OPPONENT-26",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 3);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-26");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Talusreaver");
            assert.equal(lastMoves[1].attributes.fieldIndex, 4);

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_SUMMON_CREATURE_FIELD_FULL");
            assert.equal(lastMoves[2].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.card.id, "ID_OPPONENT-34");
            assert.equal(lastMoves[2].attributes.card.name, "Talusreaver");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;

            assert.equal(opponentField[4].id, "ID_OPPONENT-33");
            assert.equal(opponentField[4].name, "Talusreaver");
            assert.equal(opponentField[4].cost, 60);
            assert.equal(opponentField[4].attack, 40);
            assert.equal(opponentField[4].health, 50);
            assert.equal(opponentField[4].abilities.indexOf(35) >= 0, true);
            assert.equal(opponentField[4].color, 4);

            assert.equal(opponentField[5].id, "NOT_EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("deathwish summons - silenced", function() {
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
              "id": "ID_OPPONENT-24",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 20,
              "health": 10,
              "cost": 30,
              "name": "Dusk Dweller",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                34
              ],
              "abilitiesStart": [
                34
              ],
              "baseId": "C40",
              "attackStart": 20,
              "costStart": 30,
              "healthStart": 10,
              "healthMax": 10,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 1,
              "spawnRank": 2
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_OPPONENT-25",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 10,
              "cost": 60,
              "name": "Talusreaver",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                35
              ],
              "abilitiesStart": [
                35
              ],
              "baseId": "C40",
              "attackStart": 40,
              "costStart": 60,
              "healthStart": 50,
              "healthMax": 50,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 1,
              "spawnRank": 3
            },
            {
              "id": "EMPTY"
            },
            {
              "id": "ID_OPPONENT-26",
              "playerId": "ID_OPPONENT",
              "color": 4,
              "level": 1,
              "category": 0,
              "attack": 60,
              "health": 10,
              "cost": 80,
              "name": "Phantom Skullcrusher",
              "description": "Deathwish: Re-summon this creature",
              "abilities": [
                36
              ],
              "abilitiesStart": [
                36
              ],
              "baseId": "C40",
              "attackStart": 60,
              "costStart": 100,
              "healthStart": 80,
              "healthMax": 80,
              "buffs": [],
              "canAttack": 0,
              "isFrozen": 0,
              "isSilenced": 1,
              "spawnRank": 4
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

    it("should support dusk dweller - silenced", function() {
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
              targetId: "ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-24");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support talusreaver - silenced", function() {
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
              targetId: "ID_OPPONENT-25",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-25");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[2].id, "EMPTY");
            assert.equal(opponentField[3].id, "EMPTY");

            resolve();
          }
        );
      });
    });

    it("should support dusk dweller - silenced", function() {
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
              targetId: "ID_OPPONENT-26",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_PLAY_SPELL_TARGETED");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-0");
            assert.equal(lastMoves[0].attributes.card.name, "Touch of Zeus");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-26");

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            const opponentField = opponentState.field;
            assert.equal(opponentField[4].id, "EMPTY");
            assert.equal(opponentField[5].id, "EMPTY");

            resolve();
          }
        );
      });
    });
  });

  describe("deathwish summons - death by other deathwish", function() {
    it("should support bombshell bombadier deathwish kill dusk dweller - dusk dweller spawned first", function() {
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
                "id": "ID_OPPONENT-24",
                "playerId": "ID_OPPONENT",
                "color": 4,
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 10,
                "cost": 30,
                "name": "Dusk Dweller",
                "description": "Deathwish: Re-summon this creature",
                "abilities": [
                  34
                ],
                "abilitiesStart": [
                  34
                ],
                "baseId": "C40",
                "attackStart": 20,
                "costStart": 30,
                "healthStart": 10,
                "healthMax": 10,
                "buffs": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 2
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
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
            "hasTurn": 1,
            "manaCurrent": 90,
            "manaMax": 90,
            "health": 60,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "ID_PLAYER-5",
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
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-5",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 6);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-5");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-24");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[1].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[1].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[1].attributes.fieldIndex, 0);

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[2].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.targetId, "ID_OPPONENT-33");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[3].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[3].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[4].playerId, "ID_PLAYER");
            assert.equal(lastMoves[4].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[4].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[4].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[5].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[5].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[5].attributes.card.id, "ID_OPPONENT-34");
            assert.equal(lastMoves[5].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[5].attributes.fieldIndex, 0);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 80);
            assert.equal(opponentState.healthMax, 100);

            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-34");
            assert.equal(opponentField[0].name, "Dusk Dweller");
            assert.equal(opponentField[0].cost, 30);
            assert.equal(opponentField[0].attack, 20);
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[0].abilities.indexOf(34) >= 0, true);

            resolve();
          }
        );
      });
    });

    it("should support bombshell bombadier deathwish kill dusk dweller - bombshell bombadier spawned first", function() {
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
                "id": "ID_OPPONENT-24",
                "playerId": "ID_OPPONENT",
                "color": 4,
                "level": 1,
                "category": 0,
                "attack": 20,
                "health": 10,
                "cost": 30,
                "name": "Dusk Dweller",
                "description": "Deathwish: Re-summon this creature",
                "abilities": [
                  34
                ],
                "abilitiesStart": [
                  34
                ],
                "baseId": "C40",
                "attackStart": 20,
                "costStart": 30,
                "healthStart": 10,
                "healthMax": 10,
                "buffs": [],
                "canAttack": 0,
                "isFrozen": 0,
                "isSilenced": 0,
                "spawnRank": 7
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
              },
              {
                "id": "NOT_EMPTY"
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
            "hasTurn": 1,
            "manaCurrent": 90,
            "manaMax": 90,
            "health": 60,
            "healthMax": 100,
            "armor": 0,
            "field": [
              {
                "id": "ID_PLAYER-5",
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
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-5",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-24",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 5);

            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].playerId, "ID_PLAYER");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-5");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-24");

            assert.equal(lastMoves[1].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[1].playerId, "ID_PLAYER");
            assert.equal(lastMoves[1].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[1].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[1].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[2].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[2].playerId, "ID_PLAYER");
            assert.equal(lastMoves[2].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[2].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[2].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[3].category, "MOVE_CATEGORY_RANDOM_TARGET");
            assert.equal(lastMoves[3].playerId, "ID_PLAYER");
            assert.equal(lastMoves[3].attributes.card.id, "ID_PLAYER-5");
            assert.equal(lastMoves[3].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[3].attributes.targetId, "TARGET_ID_FACE");

            assert.equal(lastMoves[4].category, "MOVE_CATEGORY_SUMMON_CREATURE");
            assert.equal(lastMoves[4].playerId, "ID_OPPONENT");
            assert.equal(lastMoves[4].attributes.card.id, "ID_OPPONENT-33");
            assert.equal(lastMoves[4].attributes.card.name, "Dusk Dweller");
            assert.equal(lastMoves[4].attributes.fieldIndex, 0);

            const opponentState = challengeStateData.current["ID_OPPONENT"];
            assert.equal(opponentState.health, 70);
            assert.equal(opponentState.healthMax, 100);

            const opponentField = opponentState.field;
            assert.equal(opponentField[0].id, "ID_OPPONENT-33");
            assert.equal(opponentField[0].name, "Dusk Dweller");
            assert.equal(opponentField[0].cost, 30);
            assert.equal(opponentField[0].attack, 20);
            assert.equal(opponentField[0].health, 10);
            assert.equal(opponentField[0].abilities.indexOf(34) >= 0, true);

            resolve();
          }
        );
      });
    });
  });
});
