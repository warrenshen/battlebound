const assert = require('assert');
const testUtils = require('./testUtils');

describe("challenge grant experience", function() {
  const gamesparks = testUtils.gamesparks;

  before(function() {
    return testUtils.initGS();
  });

  describe("add to experience cards by player ID - creatures", function() {
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
              "id": "ID_OPPONENT-12",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 20,
              "cost": 60,
              "name": "Adderspine Weevil",
              "description": "Taunt; Deathwish: Deal 20 dmg to all opponent creatures",
              "abilities": [],
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
              "id": "ID_PLAYER-9",
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
              "id": "ID_PLAYER-22",
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
              "id": "ID_PLAYER-23",
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
        "ID_PLAYER": ["B4"],
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

    it("should support card attack card", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-12",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-12");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 2);
            assert.equal(expCards[0], "B4");
            assert.equal(expCards[1], "C53");

            resolve();
          }
        );
      });
    });

    it("should support card attack face", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "TARGET_ID_FACE",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "TARGET_ID_FACE");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 2);
            assert.equal(expCards[0], "B4");
            assert.equal(expCards[1], "C53");

            resolve();
          }
        );
      });
    });
  });

  describe("add to experience cards by player ID - duplicate experience card", function() {
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
              "id": "ID_OPPONENT-12",
              "playerId": "ID_OPPONENT",
              "level": 1,
              "category": 0,
              "attack": 40,
              "health": 20,
              "cost": 60,
              "name": "Adderspine Weevil",
              "description": "Taunt; Deathwish: Deal 20 dmg to all opponent creatures",
              "abilities": [],
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
              "id": "ID_PLAYER-9",
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
              "id": "ID_PLAYER-22",
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
              "id": "ID_PLAYER-23",
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
        "ID_PLAYER": ["C53"],
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

    it("should support card attack card", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeCardAttackCard",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            cardId: "ID_PLAYER-9",
            attributesJson: {
              fieldId: "ID_OPPONENT",
              targetId: "ID_OPPONENT-12",
            },
          },
          function(response) {
            const challengeStateData = response.scriptData.challengeStateData;

            const lastMoves = challengeStateData.lastMoves;
            assert.equal(lastMoves.length, 1);
            assert.equal(lastMoves[0].category, "MOVE_CATEGORY_CARD_ATTACK");
            assert.equal(lastMoves[0].attributes.cardId, "ID_PLAYER-9");
            assert.equal(lastMoves[0].attributes.fieldId, "ID_OPPONENT");
            assert.equal(lastMoves[0].attributes.targetId, "ID_OPPONENT-12");

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 1);
            assert.equal(expCards[0], "C53");

            resolve();
          }
        );
      });
    });
  });

  describe("add to experience cards by player ID - spells", function() {
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
              "buffs": [],
              "canAttack": 1,
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
              "buffs": [],
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
              "buffs": [],
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
            }
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

    it("should support untargeted spell", function() {
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

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 1);
            assert.equal(expCards[0], "C20");

            resolve();
          }
        );
      });
    });

    it("should support targeted spell", function() {
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

            const expCards = challengeStateData.expCardIdsByPlayerId["ID_PLAYER"];
            assert.equal(expCards.length, 1);
            assert.equal(expCards[0], "C19");

            resolve();
          }
        );
      });
    });
  });

  describe("grant experience", function() {
    const challengeStateData = {
      "opponentIdByPlayerId": {
        "ID_PLAYER": "ID_OPPONENT",
        "ID_OPPONENT": "ID_PLAYER",
      },
      "moves": [],
      "expCardIdsByPlayerId": {
        "ID_PLAYER": ["C0", "C7", "B6", "B15"],
        "ID_OPPONENT": ["C2", "C4", "B6", "B8"],
      },
    };

    const LEVEL_TO_EXP_MAX = {
      1: 5,
      2: 10,
      3: 50,
      4: 250,
      5: 1000
    };

    it("should grant experience to correct cards of player", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeGrantExperience",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_PLAYER",
            decksDataJson: {
              cardByCardId: {
                C0: {
                  level: 1,
                  id: "C0",
                  templateId: "C0",
                },
                C7: {
                  level: 2,
                  id: "C7",
                  templateId: "C7",
                  exp: 9,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
                C8: {
                  level: 2,
                  id: "C8",
                  templateId: "C8",
                  exp: 9,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
              },
            },
            bCardsJson: [
              {
                level: 2,
                id: "B6",
                templateId: "B6",
                exp: 9,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
              {
                level: 2,
                id: "B15",
                templateId: "B15",
                exp: 3,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
              {
                level: 2,
                id: "B16",
                templateId: "B16",
                exp: 3,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
            ],
          },
          function(response) {
            const newDecksData = response.scriptData.newDecksData;
            const newBCards = response.scriptData.newBCards;
            const expCards = response.scriptData.expCards;

            assert.equal("C0", newDecksData.cardByCardId["C0"].id);
            assert.equal(1, newDecksData.cardByCardId["C0"].level);
            assert.equal(1, newDecksData.cardByCardId["C0"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[1], newDecksData.cardByCardId["C0"].expMax);

            assert.equal("C7", newDecksData.cardByCardId["C7"].id);
            assert.equal(3, newDecksData.cardByCardId["C7"].level);
            assert.equal(0, newDecksData.cardByCardId["C7"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[3], newDecksData.cardByCardId["C7"].expMax);

            // Should not have any change.
            assert.equal("C8", newDecksData.cardByCardId["C8"].id);
            assert.equal(2, newDecksData.cardByCardId["C8"].level);
            assert.equal(9, newDecksData.cardByCardId["C8"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newDecksData.cardByCardId["C8"].expMax);

            assert.equal(newBCards.length, 2);

            assert.equal("B6", newBCards[0].id);
            assert.equal(3, newBCards[0].level);
            assert.equal(0, newBCards[0].exp);
            assert.equal(LEVEL_TO_EXP_MAX[3], newBCards[0].expMax);

            assert.equal("B15", newBCards[1].id);
            assert.equal(2, newBCards[1].level);
            assert.equal(4, newBCards[1].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newBCards[1].expMax);

            assert.equal(4, expCards.length);

            assert.equal(expCards[0].id, "B6");
            assert.equal(expCards[0].expPrevious, 9);
            assert.equal(expCards[0].exp, 0);
            assert.equal(expCards[0].expMax, LEVEL_TO_EXP_MAX[3]);
            assert.equal(expCards[0].levelPrevious, 2);
            assert.equal(expCards[0].level, 3);

            assert.equal(expCards[1].id, "B15");
            assert.equal(expCards[1].expPrevious, 3);
            assert.equal(expCards[1].exp, 4);
            assert.equal(expCards[1].expMax, LEVEL_TO_EXP_MAX[2]);
            assert.equal(expCards[1].levelPrevious, 2);
            assert.equal(expCards[1].level, 2);

            assert.equal(expCards[2].id, "C0");
            assert.equal(expCards[2].expPrevious, 0);
            assert.equal(expCards[2].exp, 1);
            assert.equal(expCards[2].expMax, LEVEL_TO_EXP_MAX[1]);
            assert.equal(expCards[2].levelPrevious, 1);
            assert.equal(expCards[2].level, 1);

            assert.equal(expCards[3].id, "C7");
            assert.equal(expCards[3].expPrevious, 9);
            assert.equal(expCards[3].exp, 0);
            assert.equal(expCards[3].expMax, LEVEL_TO_EXP_MAX[3]);
            assert.equal(expCards[3].levelPrevious, 2);
            assert.equal(expCards[3].level, 3);

            resolve();
          }
        );
      });
    });

    it("should grant experience to correct cards of opponent", function() {
      return new Promise((resolve) => {
        gamesparks.sendWithData(
          "LogEventRequest",
          {
            eventKey: "TestChallengeGrantExperience",
            challengeStateString: JSON.stringify(challengeStateData),
            challengePlayerId: "ID_OPPONENT",
            decksDataJson: {
              cardByCardId: {
                C2: {
                  level: 1,
                  id: "C2",
                  templateId: "C2",
                },
                C4: {
                  level: 2,
                  id: "C4",
                  templateId: "C4",
                  exp: 0,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
                C7: {
                  level: 2,
                  id: "C7",
                  templateId: "C7",
                  exp: 0,
                  expMax: LEVEL_TO_EXP_MAX[2],
                },
              },
            },
            bCardsJson: [
              {
                level: 2,
                id: "B6",
                templateId: "B6",
                exp: 9,
                expMax: LEVEL_TO_EXP_MAX[2],
              },
              {
                level: 1,
                id: "B8",
                templateId: "B8",
                exp: 3,
                expMax: LEVEL_TO_EXP_MAX[1],
              },
            ],
          },
          function(response) {
            const newDecksData = response.scriptData.newDecksData;
            const newBCards = response.scriptData.newBCards;
            const expCards = response.scriptData.expCards;

            assert.equal("C2", newDecksData.cardByCardId["C2"].id);
            assert.equal(1, newDecksData.cardByCardId["C2"].level);
            assert.equal(1, newDecksData.cardByCardId["C2"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[1], newDecksData.cardByCardId["C2"].expMax);

            assert.equal("C4", newDecksData.cardByCardId["C4"].id);
            assert.equal(2, newDecksData.cardByCardId["C4"].level);
            assert.equal(1, newDecksData.cardByCardId["C4"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newDecksData.cardByCardId["C4"].expMax);

            // Should not have any change.
            assert.equal("C7", newDecksData.cardByCardId["C7"].id);
            assert.equal(2, newDecksData.cardByCardId["C7"].level);
            assert.equal(0, newDecksData.cardByCardId["C7"].exp);
            assert.equal(LEVEL_TO_EXP_MAX[2], newDecksData.cardByCardId["C7"].expMax);

            assert.equal(newBCards.length, 2);

            assert.equal("B6", newBCards[0].id);
            assert.equal(3, newBCards[0].level);
            assert.equal(0, newBCards[0].exp);
            assert.equal(LEVEL_TO_EXP_MAX[3], newBCards[0].expMax);

            assert.equal("B8", newBCards[1].id);
            assert.equal(1, newBCards[1].level);
            assert.equal(4, newBCards[1].exp);
            assert.equal(LEVEL_TO_EXP_MAX[1], newBCards[1].expMax);

            assert.equal(4, expCards.length);

            assert.equal(expCards[0].id, "B6");
            assert.equal(expCards[0].expPrevious, 9);
            assert.equal(expCards[0].exp, 0);
            assert.equal(expCards[0].expMax, LEVEL_TO_EXP_MAX[3]);
            assert.equal(expCards[0].levelPrevious, 2);
            assert.equal(expCards[0].level, 3);

            assert.equal(expCards[1].id, "B8");
            assert.equal(expCards[1].expPrevious, 3);
            assert.equal(expCards[1].exp, 4);
            assert.equal(expCards[1].expMax, LEVEL_TO_EXP_MAX[1]);
            assert.equal(expCards[1].levelPrevious, 1);
            assert.equal(expCards[1].level, 1);

            assert.equal(expCards[2].id, "C2");
            assert.equal(expCards[2].expPrevious, 0);
            assert.equal(expCards[2].exp, 1);
            assert.equal(expCards[2].expMax, LEVEL_TO_EXP_MAX[1]);
            assert.equal(expCards[2].levelPrevious, 1);
            assert.equal(expCards[2].level, 1);

            assert.equal(expCards[3].id, "C4");
            assert.equal(expCards[3].expPrevious, 0);
            assert.equal(expCards[3].exp, 1);
            assert.equal(expCards[3].expMax, LEVEL_TO_EXP_MAX[2]);
            assert.equal(expCards[3].levelPrevious, 2);
            assert.equal(expCards[3].level, 2);

            resolve();
          }
        );
      });
    });
  });
});
