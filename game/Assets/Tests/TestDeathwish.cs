using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestDeathwish
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
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""color"": 1,
                ""attack"": 20,
                ""health"": 10,
                ""cost"": 20,
                ""name"": ""Firebug Catelyn"",
                ""description"": """",
                ""abilities"": [
                    10
                ],
                ""attackStart"": 20,
                ""costStart"": 20,
                ""healthStart"": 10,
                ""healthMax"": 10,
                ""buffs"": [],
                ""canAttack"": 0,
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
                ""canAttack"": 0,
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
                ""canAttack"": 1,
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
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""spawnRank"": 1
            },
            {
                ""id"": ""ID_ENEMY-5"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 40,
                ""health"": 30,
                ""cost"": 20,
                ""name"": ""Ritual Hatchling"",
                ""description"": """",
                ""abilities"": [
                    24
                ],
                ""attackStart"": 40,
                ""costStart"": 20,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffs"": [],
                ""canAttack"": 1,
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
    public IEnumerator DamageEnemyFaceDeathwishTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-5");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-3");

        EffectManager.Instance.OnCreatureAttack(
            enemyCreature,
            playerCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        PlayerAvatar enemyAvatar = BattleState.Instance().Opponent.Avatar;
        Assert.AreEqual(90, enemyAvatar.Health);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual(null, playerCreature);
    }

    [UnityTest]
    public IEnumerator DamageAllEnemyCreaturesDeathwishTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-12");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-3");

        EffectManager.Instance.OnCreatureAttack(
            enemyCreature,
            playerCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 1);
        Assert.AreEqual(null, enemyCreature);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 2);
        Assert.AreEqual(10, enemyCreature.Health);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 1);
        Assert.AreEqual(null, playerCreature);
    }
}
