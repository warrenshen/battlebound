using System.Collections;
using System;

using UnityEngine;
using TMPro;

public class PlayerAvatar : Targetable
{
    public const string TARGET_ID_FACE = "TARGET_ID_FACE";

    protected Player owner;
    public Player Owner => owner;

    protected int canAttack;
    public int CanAttack => canAttack;
    protected int maxAttacks;

    [SerializeField]
    private int armor;
    public int Armor => armor;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private int maxHealth;
    public int MaxHealth => maxHealth;

    //[SerializeField]
    //private BoardWeapon weapon;
    //public BoardWeapon Weapon => weapon;

    IPlayerAvatarObject playerAvatarObject;

    public PlayerAvatar(Player player)
    {
        this.armor = 0;
        this.maxHealth = 100;
        this.health = this.maxHealth;

        //this.weapon = null;
        this.owner = player;

        this.maxAttacks = 1;
        this.canAttack = 1;

        InitializeHelper();
    }

    public PlayerAvatar(Player player, PlayerState playerState)
    {
        this.armor = playerState.Armor;
        this.maxHealth = playerState.HealthMax;
        this.health = playerState.Health;

        //this.weapon = null;
        this.owner = player;

        this.maxAttacks = 1;
        this.canAttack = 1;

        InitializeHelper();
    }

    private void InitializeHelper()
    {
        if (BattleSingleton.Instance.IsEnvironmentTest())
        {
            this.playerAvatarObject = new PlayerAvatarMock();
        }
        else
        {
            this.playerAvatarObject = GameObject
                .Find(String.Format("{0} Avatar", this.owner.Name))
                .GetComponent<PlayerAvatarObject>();
            this.playerAvatarObject.Initialize(this);
        }
    }

    public string GetCardId()
    {
        return TARGET_ID_FACE;
    }

    public string GetPlayerId()
    {
        return this.owner.Id;
    }

    public TargetableObject GetTargetableObject()
    {
        return this.playerAvatarObject as TargetableObject;
    }

    public string GetDisplayName()
    {
        return this.Owner.DisplayName;
    }

    public bool CanAttackNow()
    {
        return false;
    }

    public void OnStartTurn()
    {
        this.canAttack = this.maxAttacks;
    }

    public bool HasWeapon()
    {
        return false;
    }

    //returns true if replaced existing weapon, false if not
    public bool EquipWeapon(WeaponCard weapon)
    {
        //GameObject created = new GameObject(weapon.Name);
        //BoardWeapon weaponObject = created.AddComponent<BoardWeapon>();
        //weaponObject.Initialize(this.owner, weapon);

        //this.weapon = weaponObject;
        return false;
    }

    //public int Fight(Targetable other)
    //{
    //    int damageDone = 0;

    //    if (this.canAttack <= 0)
    //    {
    //        Debug.LogError("Fight called when canAttack is 0 or below!");
    //        return 0;
    //    }
    //    if (!this.HasWeapon())
    //    {
    //        Debug.LogError("Fight called when avatar has no weapon!");
    //        return 0;
    //    }

    //    this.canAttack -= 1;
    //    //move/animate
    //    Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
    //    LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
    //    //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

    //    FXPoolManager.Instance.PlayEffect("SlashVFX", other.transform.position);
    //    SoundManager.Instance.PlaySound("SlashSFX", other.transform.position);
    //    damageDone = this.weapon.AttackMade(other); //diff from boardcreature fight!!

    //    if (!other.IsAvatar)
    //    {
    //        FXPoolManager.Instance.PlayEffect("SlashVFX", this.transform.position);
    //        this.TakeDamage(((BoardCreature)other).GetAttack());

    //        if (((BoardCreature)other).HasAbility(Card.CARD_ABILITY_TAUNT))  //to-do this string should be chosen from some dict set by text file later                
    //            SoundManager.Instance.PlaySound("HitTauntSFX", other.transform.position);
    //    }

    //    this.UpdateStatText();
    //    return damageDone;
    //}

    public int TakeDamage(int amount)
    {
        int healthBefore = this.health;
        this.health -= amount;

        int damageTaken = Math.Min(healthBefore, amount);

        if (damageTaken > 0)
        {
            this.playerAvatarObject.TakeDamage(damageTaken);
            this.playerAvatarObject.Redraw();
        }

        return damageTaken;
    }

    public int Heal(int amount)
    {
        int healthBefore = this.health;
        this.health += amount;
        this.health = Math.Min(this.health, this.maxHealth);

        int amountHealed = Math.Min(this.health - healthBefore, amount);
        if (amountHealed > 0)
        {
            this.playerAvatarObject.Heal(amountHealed);
            this.playerAvatarObject.Redraw();
        }

        return amountHealed;
    }

    public void Die()
    {
        this.playerAvatarObject.Die();
    }
}
