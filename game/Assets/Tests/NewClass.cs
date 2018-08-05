using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

public class TestDeathRattle
{
    private const string PLAYER_STATE = @"{
        ""id"": ""ID_PLAYER"",
        ""displayName"": ""Player"",
        ""hasTurn"": 0,
        ""manaCurrent"": 20,
        ""manaMax"": 20,
        ""health"": 100,
        ""healthMax"": 100,
        ""armor"": 0,
        ""cardCount"": 30,
        ""deckSize"": 30,
        ""mode"": 0,
        ""hand"": [],
        ""field"": [
            {
                ""id"": ""ID_PLAYER-5"",
                ""level"": 1,
                ""category"": 0,
                ""color"": 1,
                ""attack"": 10,
                ""health"": 10,
                ""cost"": 10,
                ""name"": ""Firebug Catelyn"",
                ""description"": """",
                ""abilities"": [],
                ""attackStart"": 10,
                ""costStart"": 10,
                ""healthStart"": 10,
                ""healthMax"": 10,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""spawnRank"": 3
            },
            {
                ""id"": ""ID_PLAYER-12"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 40,
                ""health"": 20,
                ""cost"": 60,
                ""name"": ""Adderspine Weevil"",
                ""description"": ""Taunt; Deathwish: Deal 20 dmg to all opponent creatures"",
                ""abilities"": [
                  1,
                  30
                ],
                ""attackStart"": 40,
                ""costStart"": 60,
                ""healthStart"": 60,
                ""healthMax"": 60,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 4
            },
            {
                ""id"": ""EMPTY""
            },
            {
                ""id"": ""EMPTY""
            },
            {
                ""id"": ""EMPTY""
            },
            {
                ""id"": ""EMPTY""
            }
        ],
        ""mulliganCards"": []
    }";

    private const string ENEMY_STATE = @"{
        ""id"": ""ID_ENEMY"",
        ""displayName"": ""Enemy"",
        ""hasTurn"": 1,
        ""manaCurrent"": 20,
        ""manaMax"": 20,
        ""health"": 100,
        ""healthMax"": 100,
        ""armor"": 0,
        ""cardCount"": 30,
        ""deckSize"": 30,
        ""mode"": 0,
        ""hand"": [],
        ""field"": [
            {
                ""id"": ""ID_ENEMY-3"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 20,
                ""cost"": 20,
                ""name"": ""Young Kyo"",
                ""description"": """",
                ""abilities"": [
                    15
                ],
                ""attackStart"": 20,
                ""costStart"": 20,
                ""healthStart"": 20,
                ""healthMax"": 20,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""spawnRank"": 0
            },
            {
                ""id"": ""ID_ENEMY-4"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 20,
                ""cost"": 20,
                ""name"": ""Young Kyo"",
                ""description"": """",
                ""abilities"": [
                    15
                ],
                ""attackStart"": 20,
                ""costStart"": 20,
                ""healthStart"": 20,
                ""healthMax"": 20,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""spawnRank"": 1
            },
            {
                ""id"": ""ID_ENEMY-5"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 20,
                ""cost"": 20,
                ""name"": ""Young Kyo"",
                ""description"": """",
                ""abilities"": [
                    15
                ],
                ""attackStart"": 20,
                ""costStart"": 20,
                ""healthStart"": 20,
                ""healthMax"": 20,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""spawnRank"": 2
            },
            {
                ""id"": ""EMPTY""
            },
            {
                ""id"": ""EMPTY""
            },
            {
                ""id"": ""EMPTY""
            }
        ],
        ""mulliganCards"": []
    }";

    ResourceSingleton resourceSingleton;
    BattleSingleton battleSingleton;
    EffectManager effectManager;

    Board board;
    Player player;
    Player enemy;

    PlayerState playerState = JsonUtility.FromJson<PlayerState>(PLAYER_STATE);
    PlayerState enemyState = JsonUtility.FromJson<PlayerState>(ENEMY_STATE);

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        GameObject resourceSingletonGameObject = new GameObject();
        this.resourceSingleton = resourceSingletonGameObject.AddComponent<ResourceSingleton>();

        GameObject battleSingletonGameObject = new GameObject();
        this.battleSingleton = battleSingletonGameObject.AddComponent<BattleSingleton>();
        this.battleSingleton.SetEnvironmentTest();

        GameObject effectManagerGameObject = new GameObject();
        this.effectManager = effectManagerGameObject.AddComponent<EffectManager>();
    }

    [SetUp]
    public void SetUp()
    {
        BattleState.InstantiateWithState(
            playerState,
            enemyState,
            0
        );
    }

    [TearDown]
    public void TearDown()
    {

    }

    //[Test]
    //public void MonoBehaviorsExistTest()
    //{
    //    Assert.AreEqual(0, this.battleManager.GetServerMoves().Count);
    //}

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    //[UnityTest]
    //public IEnumerator NewTestScriptWithEnumeratorPasses()
    //{
    //    // Use the Assert class to test conditions.
    //    // yield to skip a frame
    //    yield return null;

    //    BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
    //    Assert.AreEqual("ID_PLAYER-4", playerCreature.GetCardId());

    //    BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
    //    Assert.AreEqual("ID_ENEMY-3", enemyCreature.GetCardId());
    //}
}
