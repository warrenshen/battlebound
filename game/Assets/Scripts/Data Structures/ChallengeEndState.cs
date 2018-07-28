using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[System.Serializable]
public class ChallengeEndState
{
    private string id;
    public string Id => id;

    private int level;
    public int Level => level;

    private int levelPrevious;
    public int LevelPrevious => levelPrevious;

    private List<ExperienceCard> experienceCards;
    public List<ExperienceCard> ExperienceCards => experienceCards;

    public ChallengeEndState(string id, int level, int levelPrevious)
    {
        this.id = id;
        this.level = level;
        this.levelPrevious = levelPrevious;
    }

    public void SetExperienceCards(List<ExperienceCard> experienceCards)
    {
        this.experienceCards = experienceCards;
    }
}

[System.Serializable]
public class ExperienceCard
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
    private int cost;
    public int Cost => cost;

    [SerializeField]
    private int costPrevious;
    public int CostPrevious => costPrevious;

    [SerializeField]
    private int attack;
    public int Attack => attack;

    [SerializeField]
    private int attackPrevious;
    public int AttackPrevious => attackPrevious;

    [SerializeField]
    private int healthPrevious;
    public int HealthPrevious => healthPrevious;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private List<string> abilities;
    public List<string> Abilities => abilities;

    [SerializeField]
    private List<string> abilitiesPrevious;
    public List<string> AbilitiesPrevious => abilitiesPrevious;

    [SerializeField]
    private int level;
    public int Level => level;

    [SerializeField]
    private int levelPrevious;
    public int LevelPrevious => levelPrevious;

    [SerializeField]
    private int exp;
    public int Exp => exp;

    [SerializeField]
    private int expMax;
    public int ExpMax => expMax;

    [SerializeField]
    private int expPrevious;
    public int ExpPrevious => expPrevious;

    public ExperienceCard(
        ChallengeCard challengeCard,
        int costPrevious,
        int attackPrevious,
        int healthPrevious,
        int level,
        int levelPrevious,
        int exp,
        int expPrevious,
        int expMax
    )
    {
        this.id = challengeCard.Id;
        this.name = challengeCard.Name;
        this.description = challengeCard.Description;
        this.cost = challengeCard.CostStart;
        this.attack = challengeCard.AttackStart;
        this.health = challengeCard.HealthStart;
        this.abilities = challengeCard.GetAbilities();

        this.costPrevious = costPrevious;
        this.attackPrevious = attackPrevious;
        this.healthPrevious = healthPrevious;

        this.level = level;
        this.levelPrevious = levelPrevious;
        this.exp = exp;
        this.expPrevious = expPrevious;
        this.expMax = expMax;
    }
}
