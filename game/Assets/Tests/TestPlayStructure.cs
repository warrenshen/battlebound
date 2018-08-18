using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestPlayStructure
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
                ""id"": ""ID_PLAYER-16"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 50,
                ""health"": 40,
                ""cost"": 50,
                ""name"": ""Cereboarus"",
                ""abilities"": [
                    5,
                    6
                ],
                ""attackStart"": 50,
                ""costStart"": 50,
                ""healthStart"": 40,
                ""healthMax"": 40,
                ""buffsField"": [],
                ""canAttack"": 0,
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
                ""abilities"": [
                    28,
                    1
                ],
                ""attackStart"": 20,
                ""costStart"": 30,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""buffsField"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 5
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
                ""id"": ""ID_PLAYER-0"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 2,
                ""health"": 10,
                ""cost"": 20,
                ""name"": ""Warden's Outpost"",
                ""costStart"": 20,
                ""healthStart"": 30,
                ""healthMax"": 30,
                ""spawnRank"": 2
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
    public IEnumerator StructureGrantTauntToCreaturesTest()
    {
        EffectManager.Instance.OnStructurePlay(
            "ID_PLAYER",
            "ID_PLAYER-0"
        );

        yield return null;

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-16");
        Assert.AreEqual(playerCreature.HasAbility(Card.CARD_ABILITY_TAUNT), true);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-14");
        Assert.AreEqual(playerCreature.HasAbility(Card.CARD_ABILITY_TAUNT), true);
    }

    [UnityTest]
    public IEnumerator CreatureGetTauntFromStructureTest()
    {
        EffectManager.Instance.OnCreaturePlay(
            "ID_PLAYER",
            "ID_PLAYER-16"
        );

        yield return null;

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-16");
        Assert.AreEqual(playerCreature.HasAbility(Card.CARD_ABILITY_TAUNT), true);
    }
}
