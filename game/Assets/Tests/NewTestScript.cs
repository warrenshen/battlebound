using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

public class NewTestScript
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
                ""canAttack"": 0,
                ""isFrozen"": 0,
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

    Board board;
    Player player;
    Player enemy;

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
        //List<ChallengeCard> playerHand = new List<ChallengeCard>();
        //List<Card> cards = new List<Card>
        //{
        //    new CreatureCard("ID_PLAYER-0", "Unkindled Junior", 1),
        //};
        //foreach (Card card in cards)
        //{
        //    playerHand.Add(card.GetChallengeCard());
        //}
        //playerState.SetHand(playerHand);

        BattleState.InstantiateWithState(
            playerState,
            enemyState,
            0
        );

        //GameObject boardGameObject = new GameObject("Board");
        //this.board = boardGameObject.AddComponent<Board>();

        //GameObject playerBoardPlaceZero = new GameObject("Player 0");
        //GameObject playerBoardPlaceOne = new GameObject("Player 1");
        //GameObject playerBoardPlaceTwo = new GameObject("Player 2");
        //GameObject playerBoardPlaceThree = new GameObject("Player 3");
        //GameObject playerBoardPlaceFour = new GameObject("Player 4");
        //GameObject playerBoardPlaceFive = new GameObject("Player 5");

        //GameObject playerAvatarGameObject = new GameObject("Player Avatar");
        //PlayerAvatar playerAvatar = playerAvatarGameObject.AddComponent<PlayerAvatar>();

        //GameObject enemyBoardPlaceZero = new GameObject("Enemy 0");
        //GameObject enemyBoardPlaceOne = new GameObject("Enemy 1");
        //GameObject enemyBoardPlaceTwo = new GameObject("Enemy 2");
        //GameObject enemyBoardPlaceThree = new GameObject("Enemy 3");
        //GameObject enemyBoardPlaceFour = new GameObject("Enemy 4");
        //GameObject enemyBoardPlaceFive = new GameObject("Enemy 5");

        //GameObject enemyAvatarGameObject = new GameObject("Enemy Avatar");
        ////PlayerAvatar enemyAvatar = enemyAvatarGameObject.AddComponent<PlayerAvatar>();

        //PlayerState playerState = JsonUtility.FromJson<PlayerState>(PLAYER_STATE);

        //List<ChallengeCard> playerHand = new List<ChallengeCard>();
        //List<Card> cards = new List<Card>
        //{
        //    new CreatureCard("ID_PLAYER-0", "Unkindled Junior", 1),
        //};
        //foreach (Card card in cards)
        //{
        //    playerHand.Add(card.GetChallengeCard());
        //}
        //playerState.SetHand(playerHand);

        //PlayerState enemyState = JsonUtility.FromJson<PlayerState>(ENEMY_STATE);

        //this.player = new Player(playerState, "Player");
        //this.enemy = new Player(enemyState, "Enemy");

        //Board.Instance().RegisterPlayer(this.player, playerState.Field);
        ////Board.Instance().RegisterPlayer(this.opponent, BattleSingleton.Instance.OpponentState.Field);

        //this.board.RegisterPlayer(player);
        //this.board.RegisterPlayer(enemy);
    }

    [TearDown]
    public void TearDown()
    {

    }

    [Test]
    public void NewTestScriptSimplePasses()
    {
        //Assert.AreEqual("ID_PLAYER", player.Id);
        // Use the Assert class to test conditions.
        Assert.AreEqual(false, BattleSingleton.Instance.ChallengeStarted);
        Assert.AreEqual(0, BattleState.Instance().SpawnCount);
    }

    [Test]
    public void CreaturesExistTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual("ID_PLAYER-4", playerCreature.GetCardId());

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual("ID_ENEMY-3", enemyCreature.GetCardId());
    }

    [Test]
    public void CreatureAttackCreatureTest()
    {
        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );

        playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual(null, playerCreature.GetCardId());

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual(null, enemyCreature.GetCardId());
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        Assert.AreEqual("ID_PLAYER-4", playerCreature.GetCardId());

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual("ID_ENEMY-3", enemyCreature.GetCardId());
    }

    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPass()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;

        BoardCreature playerCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_PLAYER", 0);
        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);

        EffectManager.Instance.OnCreatureAttack(
            playerCreature,
            enemyCreature
        );
    }
}
