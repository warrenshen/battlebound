using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestIcy
{
    private const string PLAYER_STATE = @"{
        ""id"": ""ID_PLAYER"",
        ""displayName"": ""Player"",
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
                ""id"": ""ID_PLAYER-28"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""color"": 1,
                ""attack"": 40,
                ""health"": 80,
                ""cost"": 70,
                ""name"": ""Sabre, Crystalline Dragon"",
                ""description"": """",
                ""abilities"": [
                    42
                ],
                ""attackStart"": 40,
                ""costStart"": 70,
                ""healthStart"": 80,
                ""healthMax"": 80,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""spawnRank"": 3
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
                ""id"": ""ID_ENEMY-9"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 40,
                ""health"": 60,
                ""cost"": 60,
                ""name"": ""Temple Guardian"",
                ""description"": """",
                ""abilities"": [
                    2
                ],
                ""attackStart"": 40,
                ""costStart"": 60,
                ""healthStart"": 60,
                ""healthMax"": 60,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""spawnRank"": 0
            },
            {
                ""id"": ""ID_ENEMY-25"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 40,
                ""health"": 80,
                ""cost"": 70,
                ""name"": ""Sabre, Crystalline Dragon"",
                ""description"": """",
                ""abilities"": [
                    42
                ],
                ""attackStart"": 40,
                ""costStart"": 70,
                ""healthStart"": 80,
                ""healthMax"": 80,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""spawnRank"": 1
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

    ResourceSingleton resourceSingleton;
    BattleSingleton battleSingleton;
    EffectManager effectManager;

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
    }

    [SetUp]
    public void SetUp()
    {
        GameObject effectManagerGameObject = new GameObject();
        this.effectManager = effectManagerGameObject.AddComponent<EffectManager>();

        BattleState.InstantiateWithState(
            playerState,
            enemyState
        );
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(this.effectManager.gameObject);
    }

    [UnityTest]
    public IEnumerator FreezeEnemyOnAttackTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-28");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");
        Assert.AreEqual(60, enemyCreature.Health);
        Assert.AreEqual(1, enemyCreature.IsFrozen);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-28");
        Assert.AreEqual(40, playerCreature.Health);
        Assert.AreEqual(0, playerCreature.IsFrozen);
    }

    [UnityTest]
    public IEnumerator FreezeEnemyOnDefendTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-28");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-25");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-25");
        Assert.AreEqual(40, enemyCreature.Health);
        Assert.AreEqual(1, enemyCreature.IsFrozen);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-28");
        Assert.AreEqual(40, playerCreature.Health);
        Assert.AreEqual(2, playerCreature.IsFrozen);
    }
}
