using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestBasicAttack
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
                ""id"": ""ID_PLAYER-4"",
                ""playerId"": ""ID_PLAYER"",
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
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""spawnRank"": 0
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

    [Test]
    public void BattleStateTest()
    {
        Assert.AreEqual("ID_PLAYER", BattleState.Instance().You.Id);
        Assert.AreEqual("ID_ENEMY", BattleState.Instance().Opponent.Id);
        Assert.AreEqual(0, BattleState.Instance().SpawnCount);
    }

    [Test]
    public void CreaturesExistTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual("ID_PLAYER-4", playerCreature.GetCardId());

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual("ID_ENEMY-3", enemyCreature.GetCardId());
    }

    [UnityTest]
    public IEnumerator CreatureAttackCreatureTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual(null, playerCreature);

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);
    }
}
