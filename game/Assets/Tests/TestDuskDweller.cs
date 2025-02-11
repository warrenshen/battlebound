﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestDuskDweller
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
                ""id"": ""ID_ENEMY-25"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 40,
                ""health"": 30,
                ""cost"": 60,
                ""name"": ""Talusreaver"",
                ""description"": """",
                ""abilities"": [
                    35
                ],
                ""attackStart"": 40,
                ""costStart"": 60,
                ""healthStart"": 50,
                ""healthMax"": 50,
                ""buffs"": [],
                ""canAttack"": 0,
                ""isFrozen"": 0,
                ""spawnRank"": 1
            },
            {
                ""id"": ""EMPTY""
            },
            {
                ""id"": ""ID_ENEMY-26"",
                ""playerId"": ""ID_ENEMY"",
                ""level"": 1,
                ""category"": 0,
                ""attack"": 60,
                ""health"": 30,
                ""cost"": 100,
                ""name"": ""Krul, Phantom Skullcrusher"",
                ""description"": """",
                ""abilities"": [
                    36
                ],
                ""attackStart"": 60,
                ""costStart"": 100,
                ""healthStart"": 80,
                ""healthMax"": 80,
                ""buffs"": [],
                ""canAttack"": 1,
                ""isFrozen"": 0,
                ""spawnRank"": 2
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
    public IEnumerator ResummonDeathwishTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_TOUCH_OF_ZEUS, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-24");

        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 0);
        Assert.AreEqual("ID_ENEMY-33", enemyCreature.GetCardId());
        Assert.AreEqual("ID_ENEMY", enemyCreature.GetPlayerId());
    }

    [UnityTest]
    public IEnumerator SummonDuskDwellersDeathwishTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_TOUCH_OF_ZEUS, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-25");

        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 2);
        Assert.AreEqual("ID_ENEMY-33", enemyCreature.GetCardId());
        Assert.AreEqual("ID_ENEMY", enemyCreature.GetPlayerId());
        Assert.AreEqual(Card.CARD_NAME_DUSK_DWELLER, enemyCreature.GetCardName());

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 3);
        Assert.AreEqual("ID_ENEMY-34", enemyCreature.GetCardId());
        Assert.AreEqual("ID_ENEMY", enemyCreature.GetPlayerId());
        Assert.AreEqual(Card.CARD_NAME_DUSK_DWELLER, enemyCreature.GetCardName());
    }

    [UnityTest]
    public IEnumerator SummonTalusreaversDeathwishTest()
    {
        Card card = Card.CreateByNameAndLevel("ID_PLAYER-0", Card.CARD_NAME_TOUCH_OF_ZEUS, 1);
        ChallengeCard challengeCard = card.GetChallengeCard("ID_PLAYER");

        BoardCreature enemyCreature = Board.Instance().GetCreatureByPlayerIdAndCardId("ID_ENEMY", "ID_ENEMY-26");

        EffectManager.Instance.OnSpellTargetedPlay(
            challengeCard,
            enemyCreature
        );

        yield return new WaitForSeconds(3);

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 4);
        Assert.AreEqual("ID_ENEMY-33", enemyCreature.GetCardId());
        Assert.AreEqual("ID_ENEMY", enemyCreature.GetPlayerId());
        Assert.AreEqual(Card.CARD_NAME_TALUSREAVER, enemyCreature.GetCardName());

        enemyCreature = Board.Instance().GetCreatureByPlayerIdAndIndex("ID_ENEMY", 5);
        Assert.AreEqual("ID_ENEMY-34", enemyCreature.GetCardId());
        Assert.AreEqual("ID_ENEMY", enemyCreature.GetPlayerId());
        Assert.AreEqual(Card.CARD_NAME_TALUSREAVER, enemyCreature.GetCardName());
    }
}
