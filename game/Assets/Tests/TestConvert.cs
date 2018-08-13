using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestConvert
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
                ""id"": ""ID_ENEMY-25"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 20,
                ""health"": 20,
                ""cost"": 20,
                ""name"": ""Young Kyo"",
                ""description"": """",
                ""abilities"": [
                    2,
                    35
                ],
                ""attackStart"": 20,
                ""costStart"": 20,
                ""healthStart"": 20,
                ""healthMax"": 20,
                ""buffs"": [
                    2000
                ],
                ""canAttack"": 0,
                ""isFrozen"": 1,
                ""isSilenced"": 1,
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
    public IEnumerator TheSevenTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_THE_SEVEN, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        EffectManager.Instance.OnSpellUntargetedPlay(
            challengeCard
        );

        yield return new WaitForSeconds(3);

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual("ID_PLAYER-30", playerCreature.GetCardId());
        Assert.AreEqual(20, playerCreature.Cost);
        Assert.AreEqual(20, playerCreature.GetAttack());
        Assert.AreEqual(20, playerCreature.Health);
        Assert.AreEqual(1, playerCreature.IsFrozen);
        Assert.AreEqual(true, playerCreature.IsSilenced);
        Assert.AreEqual(2, playerCreature.Abilities.Count);

        Assert.AreEqual(0, BattleState.Instance().GetDeadCards().Count);
    }
}
