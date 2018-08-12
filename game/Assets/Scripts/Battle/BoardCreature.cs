using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BoardCreature : Targetable
{
    private Player owner;
    public Player Owner => owner;

    private int fieldIndex;
    public int FieldIndex => fieldIndex;

    private int cost;           //cost retained for conditional removal cards
    public int Cost => cost;    //e.g. remove all cards with cost 2 or less

    private int health;
    public int Health => health;

    private bool isSilenced;
    public bool IsSilenced => isSilenced;

    private int isFrozen;
    public int IsFrozen => isFrozen;

    private int spawnRank;
    public int SpawnRank => spawnRank;

    private int canAttack;
    public int CanAttack => canAttack;

    private List<string> abilities;
    public List<string> Abilities => abilities;

    private List<string> buffsField;
    private List<string> buffsHand;

    private CreatureCard card;

    IBoardCreatureObject boardCreatureObject;

    public BoardCreature(
        ChallengeCard challengeCard,
        int fieldIndex,
        bool isResume
    )
    {
        this.owner = BattleState.Instance().GetPlayerById(challengeCard.PlayerId);
        this.fieldIndex = fieldIndex;
        this.card = challengeCard.GetCard() as CreatureCard;

        this.cost = challengeCard.Cost;
        this.health = challengeCard.Health;
        this.abilities = challengeCard.GetAbilities();

        this.canAttack = challengeCard.CanAttack;
        this.isFrozen = challengeCard.IsFrozen;
        this.isSilenced = challengeCard.IsSilenced == 1;
        this.spawnRank = challengeCard.SpawnRank;

        if (!isResume && HasAbility(Card.CARD_ABILITY_CHARGE))
        {
            this.canAttack = 1;
        }

        this.buffsField = challengeCard.GetBuffsField();
        this.buffsHand = challengeCard.GetBuffsHand();

        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            this.boardCreatureObject = new BoardCreatureMock();
        }
        else
        {
            GameObject boardCreatureGameObject = new GameObject(GetCardId());
            this.boardCreatureObject =
                boardCreatureGameObject.AddComponent<BoardCreatureObject>();
            this.boardCreatureObject.Initialize(this);
        }
    }

    public void SummonWithCallback(UnityAction callback)
    {
        this.boardCreatureObject.SummonWithCallback(callback);
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
        return this.owner.HasTurn && this.isFrozen <= 0 && this.canAttack > 0;
    }

    public Transform GetTargetableTransform()
    {
        if (this.boardCreatureObject.GetType() == typeof(BoardCreatureMock))
        {
            return null;
        }
        return (this.boardCreatureObject as TargetableObject).transform;
    }

    public TargetableObject GetTargetableObject()
    {
        return this.boardCreatureObject as TargetableObject;
    }

    public CreatureCard GetCard()
    {
        return this.card;
    }

    public string GetCardName()
    {
        return GetCard().Name;
    }

    public void Redraw()
    {
        this.boardCreatureObject.Redraw();
    }

    public void DecrementCanAttack()
    {
        this.canAttack -= 1;
    }

    public int GetAttack()
    {
        int attack = this.card.GetAttack();

        if (this.isSilenced)
        {
            return attack;
        }

        foreach (string buff in this.buffsField)
        {
            switch (buff)
            {
                case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                    attack += 30;
                    break;
                case Card.BUFF_CATEGORY_BESTOWED_VIGOR:
                    attack += 20;
                    break;
                case Card.BUFF_CATEGORY_ZERO_TWENTY:
                    break;
                case Card.BUFF_CATEGORY_TEN_TEN:
                    attack += 10;
                    break;
                case Card.BUFF_CATEGORY_THIRTY_THIRTY:
                    attack += 30;
                    break;
                case Card.BUFF_CATEGORY_TWENTY_TWENTY:
                    attack += 20;
                    break;
                case Card.BUFF_CATEGORY_TEN_THIRTY:
                    attack += 10;
                    break;
                default:
                    Debug.LogError(string.Format("Unhandled buff: {0}", buff));
                    break;
            }
        }

        return attack;
    }

    /*
     * Returns number of times board creature can attack each turn.
     */
    public int GetAttackCount()
    {
        if (HasAbility(Card.CARD_ABILITY_DOUBLE_STRIKE))
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public int GetHealthMax()
    {
        int health = this.card.GetHealth();

        if (this.isSilenced)
        {
            return health;
        }

        foreach (string buff in this.buffsField)
        {
            switch (buff)
            {
                case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                    break;
                case Card.BUFF_CATEGORY_BESTOWED_VIGOR:
                    health += 10;
                    break;
                case Card.BUFF_CATEGORY_ZERO_TWENTY:
                    health += 20;
                    break;
                case Card.BUFF_CATEGORY_TEN_TEN:
                    health += 10;
                    break;
                case Card.BUFF_CATEGORY_THIRTY_THIRTY:
                    health += 30;
                    break;
                case Card.BUFF_CATEGORY_TWENTY_TWENTY:
                    health += 20;
                    break;
                case Card.BUFF_CATEGORY_TEN_THIRTY:
                    health += 30;
                    break;
                default:
                    Debug.LogError(string.Format("Unhandled buff: {0}", buff));
                    break;
            }
        }

        return health;
    }

    public void FightAnimationWithCallback(Targetable other, UnityAction onFightFinish)
    {
        this.boardCreatureObject.FightAnimationWithCallback(
            other.GetTargetableObject(),
            onFightFinish
        );
    }

    /*
     * @return int - amount of damage taken
     */
    public int TakeDamage(int amount)
    {
        int damageTaken = 0;

        if (HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            // TODO: play shield pop sound.
            RemoveShield();
        }
        else
        {
            int healthBefore = this.health;
            this.health -= amount;
            damageTaken = Math.Min(healthBefore, amount);
        }

        if (damageTaken > 0)
        {
            this.boardCreatureObject.TakeDamage(damageTaken);
            this.boardCreatureObject.Redraw();
        }

        return damageTaken;
    }

    public int TakeDamageWithLethal()
    {
        int damageTaken = 0;

        if (HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            // TODO: play shield pop sound.
            RemoveShield();
        }
        else
        {
            int healthBefore = this.health;
            this.health = 0;
            damageTaken = healthBefore;
        }

        this.boardCreatureObject.TakeDamage(damageTaken);
        this.boardCreatureObject.Redraw();

        return damageTaken;
    }

    public void DeathNote()
    {
        if (HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            // TODO: play shield pop sound.
            RemoveShield();
        }

        int healthBefore = this.health;
        this.health = 0;

        this.boardCreatureObject.Redraw();
    }

    public void Deathwish()
    {
        this.boardCreatureObject.RenderDeathwish();
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
            this.boardCreatureObject.Heal(amountHealed);
            this.boardCreatureObject.Redraw();
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
            this.boardCreatureObject.Heal(amountHealed);
            this.boardCreatureObject.Redraw();
        }

        return amountHealed;
    }

    public void Die()
    {
        this.boardCreatureObject.Die();
    }

    public void OnStartTurn()
    {
        this.canAttack = GetAttackCount();
        this.boardCreatureObject.Redraw();
    }

    public void OnEndTurn()
    {
        DecrementIsFrozen();
        this.boardCreatureObject.Redraw();
    }

    private bool DecrementIsFrozen()
    {
        if (this.isFrozen > 0)
        {
            this.isFrozen -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetHealth(int amount)
    {
        this.health = amount;
    }

    public void GrantShield()
    {
        if (!HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            this.abilities.Add(Card.CARD_ABILITY_SHIELD);
            this.boardCreatureObject.Redraw();
        }
    }

    private void RemoveShield()
    {
        if (!HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            Debug.LogError("Remove shield called on creature without shield.");
            return;
        }

        this.abilities.Remove(Card.CARD_ABILITY_SHIELD);
        this.boardCreatureObject.Redraw();
    }

    public void GrantTaunt()
    {
        if (!HasAbility(Card.CARD_ABILITY_TAUNT))
        {
            this.abilities.Add(Card.CARD_ABILITY_TAUNT);
            this.boardCreatureObject.Redraw();
        }
    }

    public void Freeze(int amount)
    {
        this.isFrozen = Math.Max(this.IsFrozen, amount);
        this.boardCreatureObject.Redraw();
    }

    public void Silence()
    {
        this.isSilenced = true;
        this.health = Math.Min(this.health, GetHealthMax());
        this.boardCreatureObject.Redraw();
    }

    public bool HasAbility(string ability)
    {
        if (this.isSilenced)
        {
            return false;
        }

        return this.abilities.Contains(ability);
    }

    public void AddBuff(string buff)
    {
        if (Array.IndexOf(Card.VALID_BUFFS_FIELD, buff) < 0)
        {
            Debug.LogError("Invalid buff.");
            return;
        }

        this.buffsField.Add(buff);

        switch (buff)
        {
            case Card.BUFF_CATEGORY_UNSTABLE_POWER:
                break;
            case Card.BUFF_CATEGORY_BESTOWED_VIGOR:
                this.health += 10;
                break;
            case Card.BUFF_CATEGORY_ZERO_TWENTY:
                this.health += 20;
                break;
            case Card.BUFF_CATEGORY_TEN_TEN:
                this.health += 10;
                break;
            case Card.BUFF_CATEGORY_THIRTY_THIRTY:
                this.health += 30;
                break;
            case Card.BUFF_CATEGORY_TWENTY_TWENTY:
                this.health += 20;
                break;
            case Card.BUFF_CATEGORY_TEN_THIRTY:
                this.health += 30;
                break;
            default:
                Debug.LogError(string.Format("Unhandled buff: {0}", buff));
                break;
        }

        Redraw();
    }

    public bool HasBuff(string buff)
    {
        return this.buffsField.Contains(buff);
    }

    public ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();

        challengeCard.SetId(GetCardId());
        challengeCard.SetPlayerId(GetPlayerId());
        challengeCard.SetCategory((int)Card.CardType.Creature);
        challengeCard.SetColor(this.card.GetClassColor());
        challengeCard.SetName(this.card.Name);
        challengeCard.SetLevel(this.card.Level);
        challengeCard.SetCost(this.cost);
        challengeCard.SetCostStart(this.card.GetCost());
        challengeCard.SetHealth(this.health);
        challengeCard.SetHealthStart(this.card.GetHealth());
        challengeCard.SetHealthMax(GetHealthMax());
        challengeCard.SetAttack(GetAttack());
        challengeCard.SetAttackStart(this.card.GetAttack());
        challengeCard.SetCanAttack(this.canAttack);
        challengeCard.SetIsFrozen(this.isFrozen);
        challengeCard.SetIsSilenced(this.isSilenced ? 1 : 0);
        challengeCard.SetSpawnRank(this.spawnRank);
        challengeCard.SetAbilities(this.abilities);
        challengeCard.SetAbilitiesStart(this.card.GetAbilities());
        challengeCard.SetBuffsHand(this.buffsHand);
        challengeCard.SetBuffsField(this.buffsField);

        return challengeCard;
    }
}
