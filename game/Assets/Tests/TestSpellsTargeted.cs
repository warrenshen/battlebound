using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestSpellsTargeted
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
                ""attack"": 80,
                ""health"": 70,
                ""cost"": 50,
                ""name"": ""Hellbringer"",
                ""description"": """",
                ""abilities"": [
                    17
                ],
                ""attackStart"": 80,
                ""costStart"": 50,
                ""healthStart"": 70,
                ""healthMax"": 70,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 0
            },
            {
                ""id"": ""ID_PLAYER-6"",
                ""playerId"": ""ID_PLAYER"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 10,
                ""health"": 10,
                ""cost"": 10,
                ""name"": ""Firebug Catelyn"",
                ""description"": """",
                ""abilities"": [
                    10
                ],
                ""attackStart"": 10,
                ""costStart"": 10,
                ""healthStart"": 10,
                ""healthMax"": 10,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
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
                ""id"": ""ID_ENEMY-7"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 10,
                ""health"": 10,
                ""cost"": 10,
                ""name"": ""Firebug Catelyn"",
                ""description"": """",
                ""abilities"": [
                    10
                ],
                ""attackStart"": 10,
                ""costStart"": 10,
                ""healthStart"": 10,
                ""healthMax"": 10,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""isSilenced"": 0,
                ""spawnRank"": 3
            },
            {
                ""id"": ""ID_ENEMY-8"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 80,
                ""health"": 70,
                ""cost"": 50,
                ""name"": ""Hellbringer"",
                ""description"": """",
                ""abilities"": [
                    17
                ],
                ""attackStart"": 80,
                ""costStart"": 50,
                ""healthStart"": 70,
                ""healthMax"": 70,
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
    public IEnumerator TouchOfZeusTriggerOnDamageTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_TOUCH_OF_ZEUS, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-8");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-8");
        Assert.AreEqual(40, enemyCreature.Health);

        Player enemy = BattleState.Instance().GetPlayerById("ID_ENEMY");
        Assert.AreEqual(70, enemy.GetHealth());
    }

    [UnityTest]
    public IEnumerator TouchOfZeusTriggerDeathwishTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_TOUCH_OF_ZEUS, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-7");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        Player player = BattleState.Instance().GetPlayerById("ID_PLAYER");
        Assert.AreEqual(90, player.GetHealth());
    }

    [UnityTest]
    public IEnumerator DeepFreezeTriggerOnDamageTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_DEEP_FREEZE, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-8");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-8");
        Assert.AreEqual(60, enemyCreature.Health);
        Assert.AreEqual(1, enemyCreature.IsFrozen);

        Player enemy = BattleState.Instance().GetPlayerById("ID_ENEMY");
        Assert.AreEqual(70, enemy.GetHealth());
    }

    [UnityTest]
    public IEnumerator DeepFreezeTriggerDeathwishTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_DEEP_FREEZE, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-7");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        Player player = BattleState.Instance().GetPlayerById("ID_PLAYER");
        Assert.AreEqual(90, player.GetHealth());
    }
}
