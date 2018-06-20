using UnityEngine;

[System.Serializable]
public class CardRaw {
    public enum CardType: byte { Creature, Spell, Weapon, Structure };

    [SerializeField]
	private string id;
    public string Id => id;

    [SerializeField]
	private string name;
    public string Name => name;

    [SerializeField]
	private CardType category;
    public CardType Category => category;

    [SerializeField]
	private int cost;
	public int Cost => cost;

    [SerializeField]
	private string image;
    public string Image => image;

    protected Player owner;
    public Player Owner => owner;

    [SerializeField]
	private int attack;
    public int Attack => attack;

    [SerializeField]
    private int health;
    public int Health => health;
}
