using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestPiercing
{
    private const string PLAYER_STATE = @"{
        ""id"": ""ID_PLAYER"",
        ""displayName"": ""Player"",
        ""hasTurn"": 1,
        ""manaCurrent"": 20,
        ""manaMax"": 20,
        ""health"": 90,
        ""healthMax"": 100,
        ""armor"": 0,
        ""cardCount"": 30,
        ""deckSize"": 30,
        ""mode"": 0,
        ""hand"": [],
        ""field"": [
            {
                ""id"": ""ID_PLAYER-13"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""color"": 1,
                ""attack"": 40,
                ""health"": 30,
                ""cost"": 30,
                ""name"": ""Lighthunter"",
                ""description"": """",
                ""abilities"": [
                    45
                ],
                ""attackStart"": 40,
                ""costStart"": 30,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 6
            },
            {
                ""id"": ""ID_PLAYER-14"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 10,
                ""cost"": 30,
                ""name"": ""Lux"",
                ""description"": """",
                ""abilities"": [
                    28
                ],
                ""attackStart"": 20,
                ""costStart"": 30,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffsField"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 7
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
        ""manaMax"": 70,
        ""health"": 100,
        ""healthMax"": 100,
        ""armor"": 0,
        ""cardCount"": 30,
        ""deckSize"": 30,
        ""mode"": 0,
        ""hand"": [],
        ""field"": [
            {
                ""id"": ""ID_ENEMY-8"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 10,
                ""cost"": 30,
                ""name"": ""Lux"",
                ""description"": """",
                ""abilities"": [
                    28
                ],
                ""attackStart"": 20,
                ""costStart"": 30,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffsField"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 1
            },
            {
                ""id"": ""ID_ENEMY-9"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""color"": 1,
                ""attack"": 40,
                ""health"": 30,
                ""cost"": 30,
                ""name"": ""Lighthunter"",
                ""description"": """",
                ""abilities"": [
                    45
                ],
                ""attackStart"": 40,
                ""costStart"": 30,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
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
    public IEnumerator DamageEnemyHeroOnPierceTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-13");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-8");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-13");
        Assert.AreEqual(10, playerCreature.Health);

        Player enemy = BattleState.Instance().GetPlayerById("ID_ENEMY");
        Assert.AreEqual(70, enemy.GetHealth());

        Player player = BattleState.Instance().GetPlayerById("ID_PLAYER");
        Assert.AreEqual(90, player.GetHealth());
    }

    [UnityTest]
    public IEnumerator DamagePlayerHeroByPierceTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-14");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");
        Assert.AreEqual(10, enemyCreature.Health);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 1);
        Assert.AreEqual(null, playerCreature);

        Player player = BattleState.Instance().GetPlayerById("ID_PLAYER");
        Assert.AreEqual(60, player.GetHealth());

        Player enemy = BattleState.Instance().GetPlayerById("ID_ENEMY");
        Assert.AreEqual(100, enemy.GetHealth());
    }
}
