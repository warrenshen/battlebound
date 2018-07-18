using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerState
{
    [SerializeField]
    private string id;
    public string Id => id;

    [SerializeField]
    private int hasTurn;
    public int HasTurn => hasTurn;

    [SerializeField]
    private int manaCurrent;
    public int ManaCurrent => manaCurrent;

    [SerializeField]
    private int manaMax;
    public int ManaMax => manaMax;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private int healthMax;
    public int HealthMax => healthMax;

    [SerializeField]
    private int armor;
    public int Armor => armor;

    [SerializeField]
    private List<ChallengeCard> hand;
    public List<ChallengeCard> Hand => hand;

    [SerializeField]
    private int deckSize;
    public int DeckSize => deckSize;

    [SerializeField]
    private ChallengeCard[] field;
    public ChallengeCard[] Field => field;

    [SerializeField]
    private int mode;
    public int Mode => mode;

    [SerializeField]
    private List<ChallengeCard> mulliganCards;
    public List<ChallengeCard> MulliganCards => mulliganCards;

    public void SetId(string id)
    {
        this.id = id;
    }

    public void SetHasTurn(int hasTurn)
    {
        this.hasTurn = hasTurn;
    }

    public void SetManaCurrent(int manaCurrent)
    {
        this.manaCurrent = manaCurrent;
    }

    public void SetManaMax(int manaMax)
    {
        this.manaMax = manaMax;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void SetHealthMax(int healthMax)
    {
        this.healthMax = healthMax;
    }

    public void SetArmor(int armor)
    {
        this.armor = armor;
    }

    public void SetHand(List<ChallengeCard> hand)
    {
        this.hand = hand;
    }

    public void SetDeckSize(int deckSize)
    {
        this.deckSize = deckSize;
    }

    public void SetField(ChallengeCard[] field)
    {
        this.field = field;
    }

    public void SetMode(int mode)
    {
        this.mode = mode;
    }

    public void SetMulliganCards(List<ChallengeCard> mulliganCards)
    {
        this.mulliganCards = mulliganCards;
    }

    public bool Equals(PlayerState other)
    {
        if (this.field.Length != other.Field.Length)
        {
            return false;
        }
        if (this.hand.Count != other.Hand.Count)
        {
            return false;
        }

        bool areHandsEqual = true;
        bool areFieldsEqual = true;

        for (int i = 0; i < this.hand.Count; i += 1)
        {
            areHandsEqual &= this.hand.ElementAt(i).Equals(other.Hand.ElementAt(i));
        }
        for (int i = 0; i < this.field.Length; i += 1)
        {
            areFieldsEqual &= this.field.ElementAt(i).Equals(other.Field.ElementAt(i));
        }

        if (!areHandsEqual || !areFieldsEqual)
        {
            return false;
        }

        // TODO: mulliganCards
        return this.id == other.Id &&
               this.hasTurn == other.HasTurn &&
               this.manaCurrent == other.ManaCurrent &&
               this.manaMax == other.ManaMax &&
               this.health == other.Health &&
               this.healthMax == other.HealthMax &&
               this.armor == other.Armor &&
               this.deckSize == other.DeckSize &&
               this.mode == other.Mode;
    }

    /*
     * Returns the first difference between this and other PlayerState instance.
     */
    public string FirstDiff(PlayerState other)
    {
        if (this.id != other.id)
        {
            return string.Format("Id: {0} vs {1}", this.id, other.Id);
        }
        else if (this.hasTurn != other.HasTurn)
        {
            return string.Format("HasTurn: {0} vs {1}", this.hasTurn, other.HasTurn);
        }
        else if (this.manaCurrent != other.ManaCurrent)
        {
            return string.Format("ManaCurrent: {0} vs {1}", this.manaCurrent, other.ManaCurrent);
        }
        else if (this.manaMax != other.ManaMax)
        {
            return string.Format("ManaMax: {0} vs {1}", this.manaMax, other.ManaMax);
        }
        else if (this.health != other.Health)
        {
            return string.Format("Health: {0} vs {1}", this.health, other.Health);
        }
        else if (this.healthMax != other.HealthMax)
        {
            return string.Format("HealthMax: {0} vs {1}", this.healthMax, other.HealthMax);
        }
        else if (this.armor != other.Armor)
        {
            return string.Format("Armor: {0} vs {1}", this.armor, other.Armor);
        }
        else if (this.deckSize != other.DeckSize)
        {
            return string.Format("Deck size: {0} vs {1}", this.deckSize, other.DeckSize);
        }
        else if (this.mode != other.Mode)
        {
            return string.Format("Mode: {0} vs {1}", this.mode, other.Mode);
        }

        if (this.hand.Count != other.Hand.Count)
        {
            return string.Format("Hand size: {0} vs {1}", this.hand.Count, other.Hand.Count);
        }
        else
        {
            for (int i = 0; i < this.hand.Count; i += 1)
            {
                string firstDiff = this.hand.ElementAt(i).FirstDiff(other.Hand.ElementAt(i));
                if (firstDiff != null)
                {
                    return firstDiff;
                }
            }
        }

        if (this.field.Length != other.field.Length)
        {
            return string.Format("Field length: {0} vs {1}", this.field.Length, other.Field.Length);
        }
        else
        {
            for (int i = 0; i < this.hand.Count; i += 1)
            {
                string firstDiff = this.field.ElementAt(i).FirstDiff(other.Field.ElementAt(i));
                if (firstDiff != null)
                {
                    return firstDiff;
                }
            }
        }

        return null;
    }

    public List<Card> GetCardsHand()
    {
        List<Card> cards = new List<Card>();
        foreach (ChallengeCard challengeCard in this.hand)
        {
            cards.Add(challengeCard.GetCard());
        }
        return cards;
    }

    public List<Card> GetCardsMulligan()
    {
        List<Card> cards = new List<Card>();
        foreach (ChallengeCard challengeCard in this.mulliganCards)
        {
            cards.Add(challengeCard.GetCard());
        }
        return cards;
    }

    [System.Serializable]
    public class ChallengeCard
    {
        [SerializeField]
        private string id;
        public string Id => id;

        [SerializeField]
        private int category;
        public int Category => category;

        [SerializeField]
        private string name;
        public string Name => name;

        [SerializeField]
        private string description;
        public string Description => description;

        [SerializeField]
        private string image;
        public string Image => image;

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
        private int hasShield;
        public int HasShield => hasShield;

        [SerializeField]
        private List<string> abilities;
        public List<string> Abilities => abilities;

        [SerializeField]
        private int spawnRank;
        public int SpawnRank => spawnRank;

        // TODO
        //[SerializeField]
        //private List<Buff> buffs;
        //public List<Buff> Buffs => buffs;

        public void SetId(string id)
        {
            this.id = id;
        }

        public void SetCategory(int category)
        {
            this.category = category;
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

        public void SetHasShield(int hasShield)
        {
            this.hasShield = hasShield;
        }

        public void SetAbilities(List<string> abilities)
        {
            this.abilities = abilities;
        }

        public void SetSpawnRank(int spawnRank)
        {
            this.spawnRank = spawnRank;
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
            else if (this.category != other.Category)
            {
                return string.Format("Category: {0} vs {1}", this.category, other.Category);
            }
            else if (this.name != other.Name)
            {
                return string.Format("Name: {0} vs {1}", this.name, other.Name);
            }
            else if (this.level != other.Level)
            {
                return string.Format("Level: {0} vs {1}", this.level, other.Level);
            }
            else if (this.cost != other.Cost)
            {
                return string.Format("Cost: {0} vs {1}", this.cost, other.Cost);
            }
            else if (this.costStart != other.CostStart)
            {
                return string.Format("CostStart: {0} vs {1}", this.costStart, other.CostStart);
            }
            else if (this.health != other.Health)
            {
                return string.Format("Health: {0} vs {1}", this.health, other.Health);
            }
            else if (this.healthStart != other.HealthStart)
            {
                return string.Format("HealthStart: {0} vs {1}", this.healthStart, other.HealthStart);
            }
            else if (this.healthMax != other.HealthMax)
            {
                return string.Format("HealthMax: {0} vs {1}", this.healthMax, other.HealthMax);
            }
            else if (this.attack != other.Attack)
            {
                return string.Format("Level: {0} vs {1}", this.attack, other.Attack);
            }
            else if (this.attackStart != other.AttackStart)
            {
                return string.Format("AttackStart: {0} vs {1}", this.attackStart, other.AttackStart);
            }
            else if (this.canAttack != other.CanAttack)
            {
                return string.Format("CanAttack: {0} vs {1}", this.canAttack, other.CanAttack);
            }
            else if (this.hasShield != other.HasShield)
            {
                return string.Format("HasShield: {0} vs {1}", this.hasShield, other.HasShield);
            }
            else if (this.spawnRank != other.SpawnRank)
            {
                return string.Format("SpawnRank: {0} vs {1}", this.spawnRank, other.SpawnRank);
            }

            string abilitiesDiff = GetAbilitiesDiff(this.abilities, other.abilities);
            if (abilitiesDiff != null)
            {
                return abilitiesDiff;
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

            List<string> exceptAbilities = abilitiesOne.Except(abilitiesTwo).ToList();
            if (exceptAbilities.Count > 0)
            {
                return string.Format("Abilities: {0} vs {1}", string.Join(",", abilitiesOne), string.Join(",", abilitiesTwo));
            }

            return null;
        }

        public Card GetCard(bool shouldCompare = true)
        {
            if (this.category == Card.CARD_CATEGORY_MINION)
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
}
