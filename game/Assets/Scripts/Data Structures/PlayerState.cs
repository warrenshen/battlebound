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
    private string displayName;
    public string DisplayName => displayName;

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
    private int cardCount;
    public int CardCount => cardCount;

    [SerializeField]
    private int deckSize;
    public int DeckSize => deckSize;

    [SerializeField]
    private int mode;
    public int Mode => mode;

    [SerializeField]
    private List<ChallengeCard> hand;
    public List<ChallengeCard> Hand => hand;

    [SerializeField]
    private ChallengeCard[] field;
    public ChallengeCard[] Field => field;

    [SerializeField]
    private List<ChallengeCard> mulliganCards;
    public List<ChallengeCard> MulliganCards => mulliganCards;

    public void SetId(string id)
    {
        this.id = id;
    }

    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
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

    public void SetCardCount(int cardCount)
    {
        this.cardCount = cardCount;
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
        return FirstDiff(other) == null;
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
        else if (this.cardCount != other.CardCount)
        {
            return string.Format("CardCount: {0} vs {1}", this.cardCount, other.CardCount);
        }
        else if (this.deckSize != other.DeckSize)
        {
            return string.Format("DeckSize: {0} vs {1}", this.deckSize, other.DeckSize);
        }
        else if (this.mode != other.Mode)
        {
            return string.Format("Mode: {0} vs {1}", this.mode, other.Mode);
        }

        if (this.hand.Count != other.Hand.Count)
        {
            return string.Format("HandSize: {0} vs {1}", this.hand.Count, other.Hand.Count);
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
            for (int i = 0; i < this.field.Length; i += 1)
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
            cards.Add(challengeCard.GetCard(false));
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
}
