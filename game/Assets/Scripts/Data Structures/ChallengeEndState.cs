using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[System.Serializable]
public class ChallengeEndState
{
    List<ExperienceCard> experienceCards;

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
        private string image;
        public string Image => image;

        [SerializeField]
        private int cost;
        public int Cost => cost;

        [SerializeField]
        private int health;
        public int Health => health;

        [SerializeField]
        private int attack;
        public int Attack => attack;

        [SerializeField]
        private List<string> abilities;
        public List<string> Abilities => abilities;

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
        private int expPrevious;
        public int ExpPrevious => expPrevious;

        [SerializeField]
        private int expMax;
        public int ExpMax => expMax;

        // TODO
        //[SerializeField]
        //private List<Buff> buffs;
        //public List<Buff> Buffs => buffs;
    }
}
