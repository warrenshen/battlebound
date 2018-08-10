using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestEndTurnBuff
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
                ""id"": ""ID_PLAYER-22"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 10,
                ""health"": 30,
                ""cost"": 10,
                ""name"": ""Firesmith Apprentice"",
                ""description"": """",
                ""abilities"": [
                    43
                ],
                ""attackStart"": 10,
                ""costStart"": 30,
                ""healthStart"": 10,
                ""healthMax"": 10,
                ""buffsField"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""spawnRank"": 1
            },
            {
                ""id"": ""ID_PLAYER-24"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 10,
                ""health"": 30,
                ""cost"": 20,
                ""name"": ""PAL_V1"",
                ""description"": """",
                ""abilities"": [
                    40
                ],
                ""attackStart"": 10,
                ""costStart"": 20,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffsField"": [],
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
    public IEnumerator BuffRandomFriendlyEndTurnTest()
    {
        BoardCreature firesmithCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-22");
        BoardCreature palCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-24");

        Assert.AreEqual(10, firesmithCreature.GetAttack());
        Assert.AreEqual(30, firesmithCreature.GetHealthMax());

        Assert.AreEqual(10, palCreature.GetAttack());
        Assert.AreEqual(30, palCreature.GetHealthMax());

        EffectManager.Instance.OnEndTurn("ID_PLAYER", null);

        yield return new WaitForSeconds(3);

        Assert.AreEqual(10, firesmithCreature.GetAttack());
        Assert.AreEqual(50, firesmithCreature.GetHealthMax());

        Assert.AreEqual(20, palCreature.GetAttack());
        Assert.AreEqual(40, palCreature.GetHealthMax());
    }
}
