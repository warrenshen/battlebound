using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestDuskDwellerBombshellBombadier
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
                ""id"": ""ID_PLAYER-5"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 50,
                ""health"": 20,
                ""cost"": 70,
                ""name"": ""Bombshell Bombadier"",
                ""description"": """",
                ""abilities"": [
                    12
                ],
                ""attackStart"": 50,
                ""costStart"": 70,
                ""healthStart"": 40,
                ""healthMax"": 40,
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
        ""fieldBack"": [
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
        ""cardCount"": 33,
        ""deckSize"": 30,
        ""mode"": 0,
        ""hand"": [],
        ""field"": [
            {
                ""id"": ""ID_ENEMY-24"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 10,
                ""cost"": 30,
                ""name"": ""Dusk Dweller"",
                ""description"": """",
                ""abilities"": [
                    34
                ],
                ""attackStart"": 20,
                ""costStart"": 30,
                ""healthStart"": 10,
                ""healthMax"": 10,
                ""buffs"": [],
                ""canAttack"": 0,
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
        ""fieldBack"": [
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
    public IEnumerator DuskDwellerBombshellBombadierTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-5");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-24");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual(null, playerCreature);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual("ID_ENEMY-34", enemyCreature.GetCardId());
        Assert.AreEqual("ID_ENEMY", enemyCreature.GetPlayerId());

        Player enemy = BattleState.Instance().GetPlayerById("ID_ENEMY");
        Assert.AreEqual(80, enemy.GetHealth());

        Assert.AreEqual(3, BattleState.Instance().GetDeadCards().Count);
        Assert.AreEqual("ID_ENEMY-24", BattleState.Instance().GetDeadCards()[0].Id);
        Assert.AreEqual("ID_PLAYER-5", BattleState.Instance().GetDeadCards()[1].Id);
        Assert.AreEqual("ID_ENEMY-33", BattleState.Instance().GetDeadCards()[2].Id);
    }
}
