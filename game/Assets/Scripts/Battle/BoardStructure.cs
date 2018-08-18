using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoardStructure : Targetable
{
    private Player owner;
    public Player Owner => owner;

    private int fieldIndex;
    public int FieldIndex => fieldIndex;

    private int cost;
    public int Cost => cost;

    private int health;
    public int Health => health;

    private int spawnRank;
    public int SpawnRank => spawnRank;

    private StructureCard card;

    IBoardStructureObject boardStructureObject;

    public BoardStructure(
        ChallengeCard challengeCard,
        int fieldIndex,
        bool isResume
    )
    {
        this.owner = BattleState.Instance().GetPlayerById(challengeCard.PlayerId);
        this.fieldIndex = fieldIndex;
        this.card = challengeCard.GetCard() as StructureCard;

        this.cost = challengeCard.Cost;
        this.health = challengeCard.Health;

        this.spawnRank = challengeCard.SpawnRank;

        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            this.boardStructureObject = new BoardStructureMock();
        }
        else
        {
            GameObject boardStructureGameObject = new GameObject(GetCardId());
            this.boardStructureObject =
                boardStructureGameObject.AddComponent<BoardStructureObject>();
            this.boardStructureObject.Initialize(this);
        }
    }

    public void SummonWithCallback(UnityAction callback)
    {
        this.boardStructureObject.SummonWithCallback(callback);
    }

    public string GetCardId()
    {
        return this.card.Id;
    }

    public string GetPlayerId()
    {
        return this.owner.Id;
    }

    public bool CanAttackNow()
    {
        return false;
    }

    public Transform GetTargetableTransform()
    {
        if (this.boardStructureObject.GetType() == typeof(BoardStructureMock))
        {
            return null;
        }
        return (this.boardStructureObject as TargetableObject).transform;
    }

    public TargetableObject GetTargetableObject()
    {
        return this.boardStructureObject as TargetableObject;
    }

    public StructureCard GetCard()
    {
        return this.card;
    }

    public string GetCardName()
    {
        return GetCard().Name;
    }

    public void Redraw()
    {
        this.boardStructureObject.Redraw();
    }

    public int GetHealthMax()
    {
        int health = this.card.GetHealth();
        return health;
    }

    /*
     * @return int - amount of damage taken
     */
    public int TakeDamage(int amount)
    {
        int healthBefore = this.health;
        this.health -= amount;
        int damageTaken = Math.Min(healthBefore, amount);

        if (damageTaken > 0)
        {
            this.boardStructureObject.TakeDamage(damageTaken);
            this.boardStructureObject.Redraw();
        }

        return damageTaken;
    }

    public void DeathNote()
    {
        int healthBefore = this.health;
        this.health = 0;

        this.boardStructureObject.Redraw();
    }

    /*
     * @return int - amount of health healed
     */
    public int Heal(int amount)
    {
        int healthBefore = this.health;
        this.health += amount;
        this.health = Math.Min(this.health, GetHealthMax());

        int amountHealed = Math.Min(this.health - healthBefore, amount);

        if (amountHealed > 0)
        {
            this.boardStructureObject.Heal(amountHealed);
            this.boardStructureObject.Redraw();
        }

        return amountHealed;
    }

    /*
     * @return int - amount of health healed
     */
    public int HealMax()
    {
        int healthBefore = this.health;
        int healthMax = GetHealthMax();
        this.health = healthMax;

        int amountHealed = healthMax - healthBefore;

        if (amountHealed > 0)
        {
            this.boardStructureObject.Heal(amountHealed);
            this.boardStructureObject.Redraw();
        }

        return amountHealed;
    }

    public void Die()
    {
        this.boardStructureObject.Die();
    }

    public void OnStartTurn()
    {
        this.boardStructureObject.Redraw();
    }

    public void OnEndTurn()
    {
        this.boardStructureObject.Redraw();
    }

    public void SetHealth(int amount)
    {
        this.health = amount;
    }

    public ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();

        challengeCard.SetId(GetCardId());
        challengeCard.SetPlayerId(GetPlayerId());
        challengeCard.SetCategory((int)Card.CardType.Structure);
        challengeCard.SetColor(this.card.GetClassColor());
        challengeCard.SetName(this.card.Name);
        challengeCard.SetLevel(this.card.Level);
        challengeCard.SetCost(this.cost);
        challengeCard.SetCostStart(this.card.GetCost());
        challengeCard.SetHealth(this.health);
        challengeCard.SetHealthStart(this.card.GetHealth());
        challengeCard.SetHealthMax(GetHealthMax());
        challengeCard.SetSpawnRank(this.spawnRank);

        return challengeCard;
    }
}
