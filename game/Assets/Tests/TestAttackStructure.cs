using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestAttackStructure
{
    private const string PLAYER_STATE = @"{
        ""id"": ""ID_PLAYER"",
        ""displayName"": ""Player"",
        ""hasTurn"": 0,
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
                    6,
                    1
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
                ""buffsField"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
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
    public IEnumerator StructureRemoveTauntFromCreaturesTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_ENEMY-0", Card.CARD_NAME_TOUCH_OF_ZEUS, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_ENEMY");

        BoardStructure playerStructure = Board.Instance().GetStructureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-0");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            playerStructure
        );

        yield return new WaitForSeconds(3);

        playerStructure = Board.Instance().GetStructureByPlayerIdAndIndex("ID_PLAYER", 6);
        Assert.AreEqual(null, playerStructure);

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-16");
        Assert.AreEqual(false, playerCreature.HasAbility(Card.CARD_ABILITY_TAUNT));

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-14");
        Assert.AreEqual(true, playerCreature.HasAbility(Card.CARD_ABILITY_TAUNT));
    }
}
