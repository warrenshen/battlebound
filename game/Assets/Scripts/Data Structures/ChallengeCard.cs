using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChallengeCard
{
    [SerializeField]
    private string id;
    public string Id => id;

    [SerializeField]
    private string playerId;
    public string PlayerId => playerId;

    [SerializeField]
    private int category;
    public int Category => category;

    [SerializeField]
    private CardTemplate.ClassColor color;
    public CardTemplate.ClassColor Color => color;

    [SerializeField]
    private string name;
    public string Name => name;

    [SerializeField]
    private string description;
    public string Description => description;

    [SerializeField]
    private int level;
    public int Level => level;

    [SerializeField]
    private int cost;
    public int Cost => cost;

    [SerializeField]
    private int costStart;
    public int CostStart => costStart;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private int healthStart;
    public int HealthStart => healthStart;

    [SerializeField]
    private int healthMax;
    public int HealthMax => healthMax;

    [SerializeField]
    private int attack;
    public int Attack => attack;

    [SerializeField]
    private int attackStart;
    public int AttackStart => attackStart;

    [SerializeField]
    private int canAttack;
    public int CanAttack => canAttack;

    [SerializeField]
    private int isFrozen;
    public int IsFrozen => isFrozen;

    [SerializeField]
    private int isSilenced;
    public int IsSilenced => isSilenced;

    [SerializeField]
    private int spawnRank;
    public int SpawnRank => spawnRank;

    [SerializeField]
    private List<string> abilities;

    [SerializeField]
    private List<string> abilitiesStart;

    [SerializeField]
    private List<string> buffsHand;

    [SerializeField]
    private List<string> buffsField;

    public void SetId(string id)
    {
        this.id = id;
    }

    public void SetPlayerId(string playerId)
    {
        this.playerId = playerId;
    }

    public void SetCategory(int category)
    {
        this.category = category;
    }

    public void SetColor(CardTemplate.ClassColor color)
    {
        this.color = color;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetDescription(string description)
    {
        this.description = description;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void SetCost(int cost)
    {
        this.cost = cost;
    }

    public void SetCostStart(int costStart)
    {
        this.costStart = costStart;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void SetHealthStart(int healthStart)
    {
        this.healthStart = healthStart;
    }

    public void SetHealthMax(int healthMax)
    {
        this.healthMax = healthMax;
    }

    public void SetAttack(int attack)
    {
        this.attack = attack;
    }

    public void SetAttackStart(int attackStart)
    {
        this.attackStart = attackStart;
    }

    public void SetCanAttack(int canAttack)
    {
        this.canAttack = canAttack;
    }

    public void SetIsFrozen(int isFrozen)
    {
        this.isFrozen = isFrozen;
    }

    public void SetIsSilenced(int isSilenced)
    {
        this.isSilenced = isSilenced;
    }

    public void SetSpawnRank(int spawnRank)
    {
        this.spawnRank = spawnRank;
    }

    public void SetAbilities(List<string> abilities)
    {
        this.abilities = abilities;
    }

    public void SetAbilitiesStart(List<string> abilitiesStart)
    {
        this.abilitiesStart = abilitiesStart;
    }

    public void SetBuffsHand(List<string> buffsHand)
    {
        this.buffsHand = buffsHand;
    }

    public void SetBuffsField(List<string> buffsField)
    {
        this.buffsField = buffsField;
    }

    public List<string> GetAbilities()
    {
        return Card.GetAbilityStringsByCodes(this.abilities);
    }

    public List<string> GetAbilitiesStart()
    {
        return Card.GetAbilityStringsByCodes(this.abilitiesStart);
    }

    public List<string> GetBuffsHand()
    {
        return Card.GetBuffStringByCodes(this.buffsHand);
    }

    public List<string> GetBuffsField()
    {
        return Card.GetBuffStringByCodes(this.buffsField);
    }

    public bool Equals(ChallengeCard other)
    {
        return FirstDiff(other) == null;
    }

    public string FirstDiff(ChallengeCard other)
    {
        if (this.id != other.Id)
        {
            return string.Format("Id: {0} vs {1}", this.id, other.Id);
        }
        else if (this.id == "HIDDEN")
        {
            return null;
        }
        else if (this.name != other.Name)
        {
            return string.Format("Name: {0} vs {1} [{2}]", this.name, other.Name, this.id);
        }
        else if (this.playerId != other.PlayerId)
        {
            return string.Format("PlayerId: {0} vs {1} [{2}]", this.playerId, other.PlayerId, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.category != other.Category)
        {
            return string.Format("Category: {0} vs {1} [{2}]", this.category, other.Category, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.color != other.Color)
        {
            return string.Format("Color: {0} vs {1} [{2}]", this.color, other.Color, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.level != other.Level)
        {
            return string.Format("Level: {0} vs {1} [{2}]", this.level, other.Level, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.cost != other.Cost)
        {
            return string.Format("Cost: {0} vs {1} [{2}]", this.cost, other.Cost, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.costStart != other.CostStart)
        {
            return string.Format("CostStart: {0} vs {1} [{2}]", this.costStart, other.CostStart, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.health != other.Health)
        {
            return string.Format("Health: {0} vs {1} [{2}]", this.health, other.Health, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.healthStart != other.HealthStart)
        {
            return string.Format("HealthStart: {0} vs {1} [{2}]", this.healthStart, other.HealthStart, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.healthMax != other.HealthMax)
        {
            return string.Format("HealthMax: {0} vs {1} [{2}]", this.healthMax, other.HealthMax, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.attack != other.Attack)
        {
            return string.Format("Level: {0} vs {1} [{2}]", this.attack, other.Attack, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.attackStart != other.AttackStart)
        {
            return string.Format("AttackStart: {0} vs {1} [{2}]", this.attackStart, other.AttackStart, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.canAttack != other.CanAttack)
        {
            return string.Format("CanAttack: {0} vs {1} [{2}]", this.canAttack, other.CanAttack, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.isFrozen != other.IsFrozen)
        {
            return string.Format("IsFrozen: {0} vs {1} [{2}]", this.isFrozen, other.IsFrozen, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.isSilenced != other.IsSilenced)
        {
            return string.Format("IsSilenced: {0} vs {1} [{2}]", this.isSilenced, other.IsSilenced, string.Format("{0}, {1}", this.id, other.Name));
        }
        else if (this.spawnRank != other.SpawnRank)
        {
            return string.Format("SpawnRank: {0} vs {1} [{2}]", this.spawnRank, other.SpawnRank, string.Format("{0}, {1}", this.id, other.Name));
        }

        string abilitiesDiff = GetAbilitiesDiff(this.abilities, other.abilities);
        if (abilitiesDiff != null)
        {
            return string.Format("Abilities: {0} [{1}]", abilitiesDiff, string.Format("{0}, {1}", this.id, other.Name));
        }

        string abilitiesStartDiff = GetAbilitiesDiff(this.abilitiesStart, other.abilitiesStart);
        if (abilitiesStartDiff != null)
        {
            return string.Format("AbilitiesStart: {0} [{1}]", abilitiesStartDiff, string.Format("{0}, {1}", this.id, other.Name));
        }

        string buffsHandDiff = GetAbilitiesDiff(this.buffsHand, other.buffsHand);
        if (buffsHandDiff != null)
        {
            return string.Format("BuffsHand: {0} [{1}]", buffsHandDiff, string.Format("{0}, {1}", this.id, other.Name));
        }

        string buffsFieldDiff = GetAbilitiesDiff(this.buffsField, other.buffsField);
        if (abilitiesStartDiff != null)
        {
            return string.Format("BuffsField: {0} [{1}]", buffsFieldDiff, string.Format("{0}, {1}", this.id, other.Name));
        }

        return null;
    }

    private static string GetAbilitiesDiff(List<string> abilityCodes, List<string> abilitiesTwo)
    {
        if (abilityCodes == null)
        {
            abilityCodes = new List<string>();
        }

        List<string> abilitiesOne = Card.GetAbilityStringsByCodes(abilityCodes);

        if (abilitiesTwo == null)
        {
            abilitiesTwo = new List<string>();
        }

        abilitiesOne = new List<string>(abilitiesOne.Where(ability => !string.IsNullOrEmpty(ability) && ability != Card.CARD_EMPTY_ABILITY));
        abilitiesTwo = new List<string>(abilitiesTwo.Where(ability => !string.IsNullOrEmpty(ability) && ability != Card.CARD_EMPTY_ABILITY));

        List<string> exceptOneAbilities = abilitiesOne.Except(abilitiesTwo).ToList();
        List<string> exceptTwoAbilities = abilitiesTwo.Except(abilitiesOne).ToList();
        if (exceptOneAbilities.Count > 0 || exceptTwoAbilities.Count > 0)
        {
            return string.Format("Abilities: {0} vs {1}", string.Join(",", abilitiesOne), string.Join(",", abilitiesTwo));
        }

        return null;
    }

    private static string GetBuffsDiff(List<string> abilityCodes, List<string> abilitiesTwo)
    {
        if (abilityCodes == null)
        {
            abilityCodes = new List<string>();
        }

        List<string> abilitiesOne = Card.GetAbilityStringsByCodes(abilityCodes);

        if (abilitiesTwo == null)
        {
            abilitiesTwo = new List<string>();
        }

        abilitiesOne = new List<string>(abilitiesOne.Where(ability => !string.IsNullOrEmpty(ability) && ability != Card.CARD_EMPTY_ABILITY));
        abilitiesTwo = new List<string>(abilitiesTwo.Where(ability => !string.IsNullOrEmpty(ability) && ability != Card.CARD_EMPTY_ABILITY));

        List<string> exceptOneAbilities = abilitiesOne.Except(abilitiesTwo).ToList();
        List<string> exceptTwoAbilities = abilitiesTwo.Except(abilitiesOne).ToList();
        if (exceptOneAbilities.Count > 0 || exceptTwoAbilities.Count > 0)
        {
            return string.Format("Abilities: {0} vs {1}", string.Join(",", abilitiesOne), string.Join(",", abilitiesTwo));
        }

        return null;
    }

    public Card GetCard(bool shouldCompare = true)
    {
        if (this.category == (int)Card.CardType.Creature)
        {
            CreatureCard creatureCard = new CreatureCard(
                this.id,
                this.name,
                this.level
            );

            if (shouldCompare)
            {
                if (this.cost != creatureCard.GetCost())
                {
                    Debug.LogWarning(
                        string.Format(
                            "Server db vs device codex {0} cost mismatch: {1} vs {2}",
                            this.name,
                            this.cost,
                            creatureCard.GetCost()
                        )
                    );
                }
                if (this.attackStart != creatureCard.GetAttack())
                {
                    Debug.LogWarning(
                        string.Format(
                            "Server db vs device codex {0} attack mismatch: {1} vs {2}",
                            this.name,
                            this.attack,
                            creatureCard.GetAttack()
                        )
                    );
                }
                if (this.healthStart != creatureCard.GetHealth())
                {
                    Debug.LogWarning(
                        string.Format(
                            "Server db vs device codex {0} health mismatch: {1} vs {2}",
                            this.name,
                            this.health,
                            creatureCard.GetAttack()
                        )
                    );
                }

                string abilitiesDiff = GetAbilitiesDiff(this.abilities, creatureCard.GetAbilities());
                if (abilitiesDiff != null)
                {
                    Debug.LogWarning(
                        string.Format(
                            "Server db vs device codex {0} abilities mismatch: {1}",
                            this.name,
                            abilitiesDiff
                        )
                    );
                }
            }

            return creatureCard;
        }
        else
        {
            return new SpellCard(
                this.id,
                this.name,
                this.level
            );
        }
    }
}