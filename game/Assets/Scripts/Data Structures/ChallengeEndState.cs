using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[System.Serializable]
public class ChallengeEndState
{
    [SerializeField]
    private string id;
    public string Id => id;

    [SerializeField]
    private int level;
    public int Level => level;

    [SerializeField]
    private int levelPrevious;
    public int LevelPrevious => levelPrevious;

    [SerializeField]
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
    private string name;
    public string Name => name;

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
        string id,
        string name,
        int level,
        int levelPrevious,
        int exp,
        int expPrevious,
        int expMax
    )
    {
        this.id = id;
        this.name = name;

        this.level = level;
        this.levelPrevious = levelPrevious;
        this.exp = exp;
        this.expPrevious = expPrevious;
        this.expMax = expMax;
    }

    public Card GetCard()
    {
        if (this.name == null)
        {
            Debug.LogError("Experience card does not have a name.");
            return null;
        }
        else if (this.level <= 1)
        {
            Debug.LogError("Experience card does not have a valid level.");
        }

        return Card.CreateByNameAndLevel(this.id, this.name, this.level);
    }

    public Card GetCardPrevious()
    {
        if (this.name == null)
        {
            Debug.LogError("Experience card does not have a name.");
            return null;
        }
        else if (this.levelPrevious <= 1)
        {
            Debug.LogError("Experience card does not have a valid level previous.");
        }

        return Card.CreateByNameAndLevel(this.id, this.name, this.levelPrevious);
    }
}
