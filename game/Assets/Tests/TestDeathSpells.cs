using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestDeathSpells
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
                ""id"": ""ID_PLAYER-5"",
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
                ""spawnRank"": 1
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
    public IEnumerator DeathNoteNotTriggerOnDamageTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_DEATH_NOTE, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-8");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 1);
        Assert.AreEqual(null, enemyCreature);

        PlayerAvatar playerAvatar = BattleState.Instance().GetPlayerById("ID_ENEMY").Avatar;
        Assert.AreEqual(100, playerAvatar.Health);
    }

    [UnityTest]
    public IEnumerator DeathNoteTriggerDeathwishTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_DEATH_NOTE, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-7");
        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        PlayerAvatar playerAvatar = BattleState.Instance().GetPlayerById("ID_PLAYER").Avatar;
        Assert.AreEqual(90, playerAvatar.Health);
    }

    [UnityTest]
    public IEnumerator BattleRoyaleTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_BATTLE_ROYALE, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        EffectManager.Instance.OnSpellUntargetedPlay(
            challengeCard
        );

        yield return new WaitForSeconds(3);

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 1);
        Assert.AreEqual(null, enemyCreature);

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_PLAYER", "ID_PLAYER-4");
        Assert.AreEqual(70, playerCreature.Health);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 1);
        Assert.AreEqual(null, playerCreature);

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 2);
        Assert.AreEqual(null, playerCreature);

        PlayerAvatar playerAvatar = BattleState.Instance().GetPlayerById("ID_PLAYER").Avatar;
        Assert.AreEqual(90, playerAvatar.Health);

        PlayerAvatar enemyAvatar = BattleState.Instance().GetPlayerById("ID_ENEMY").Avatar;
        Assert.AreEqual(90, playerAvatar.Health);
    }
}
