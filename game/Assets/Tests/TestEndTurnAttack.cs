using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestEndTurnAttack
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
                ""name"": ""Emberkitty"",
                ""description"": """",
                ""abilities"": [
                    13
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
                ""id"": ""ID_PLAYER-3"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 10,
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
                ""isFrozen"": 2,
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
            },
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
    public void DecrementIsFrozenEndTurnTest()
    {
        EffectManager.Instance.OnEndTurn("ID_PLAYER", null);

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-3");

        Assert.AreEqual(1, playerCreature.IsFrozen);
    }

    [UnityTest]
    public IEnumerator AttackInFrontEndTurnTest()
    {
        EffectManager.Instance.OnEndTurn("ID_PLAYER", null);

        yield return new WaitForSeconds(3);

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-3");
        Assert.AreEqual(10, enemyCreature.Health);
    }
}
