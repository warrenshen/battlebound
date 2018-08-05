using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestDivineShield
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
                ""color"": 1,
                ""attack"": 10,
                ""health"": 30,
                ""cost"": 30,
                ""name"": ""Pricklepillar"",
                ""description"": """",
                ""abilities"": [
                    1,
                    21
                ],
                ""attackStart"": 10,
                ""costStart"": 30,
                ""healthStart"": 30,
                ""healthMax"": 30,
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
                ""health"": 60,
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
                    1,
                    2
                ],
                ""attackStart"": 40,
                ""costStart"": 60,
                ""healthStart"": 60,
                ""healthMax"": 60,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
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

    [UnityTest]
    public IEnumerator ShieldBlocksAttackTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-12");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-12");
        Assert.AreEqual(20, enemyCreature.Health);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");
        Assert.AreEqual(false, playerCreature.HasAbility(Card.CARD_ABILITY_SHIELD));
        Assert.AreEqual(60, playerCreature.Health);
    }

    [UnityTest]
    public IEnumerator ShieldBlocksLethalTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-5");
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        yield return null;

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual(null, enemyCreature);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-9");
        Assert.AreEqual(false, playerCreature.HasAbility(Card.CARD_ABILITY_SHIELD));
        Assert.AreEqual(60, playerCreature.Health);
    }
}
