using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {
    public enum Type: byte { Creature, Spell, Weapon, Structure };
    // enum Class : byte { Warrior, Hunter };
    public string name;
    public Type type; 
    public int cost;
    public string imagefile;   //public because unity serializer does public only, and json output won't use getter

    public Card(string name, Type type, int cost, string imagefile) {
        this.name = name;
        this.type = type;
        this.cost = cost;
        this.imagefile = imagefile;
    }
}
