using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using TMPro;

public class PlayerAvatar : Targetable
{
    public static string TARGET_ID_FACE = "TARGET_ID_FACE";

    [SerializeField]
    private int armor;
    public int Armor => armor;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private int maxHealth;
    public int MaxHealth => maxHealth;

    [SerializeField]
    private BoardWeapon weapon;
    public BoardWeapon Weapon => weapon;

    [SerializeField]
    private string avatar;
    public string Avatar => avatar;

    //int canAttack / CanAttack in Targetable class
    //int maxAttacks in Targetable class
    //Player owner / Owner exists in Targetable class

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool frozen;
    public bool Frozen => frozen;

    TextMeshPro healthLabel;
    TextMeshPro nameLabel;

    AudioSource audioSource;


    public void Initialize(Player player)
    {
        this.armor = 0;
        this.maxHealth = 100;
        this.health = this.maxHealth;

        this.weapon = null;
        this.isAvatar = true;

        this.owner = player;

        this.maxAttacks = 1;
        this.canAttack = 1;

        this.audioSource = GetComponent<AudioSource>();
        InitializeRender();
    }

    public void Initialize(Player player, PlayerState playerState)
    {
        this.armor = playerState.Armor;
        this.maxHealth = playerState.HealthMax;
        this.health = playerState.Health;

        this.weapon = null;
        this.isAvatar = true;

        this.owner = player;

        this.maxAttacks = 1;
        this.canAttack = 1;

        this.audioSource = GetComponent<AudioSource>();
        InitializeRender();
    }

    private void InitializeRender()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -5;

        CreateLabelOnAvatar(ref this.nameLabel, Vector3.left * 1.05f);
        CreateLabelOnAvatar(ref this.healthLabel, Vector3.right * 1.05f);

        this.nameLabel.alignment = TextAlignmentOptions.Right;
        this.healthLabel.alignment = TextAlignmentOptions.Left;
        this.nameLabel.text = this.owner.DisplayName;
        //already has cylinder collider in scene
        UpdateStatText();
    }

    private void CreateLabelOnAvatar(ref TextMeshPro assignTo, Vector3 offset)
    {
        GameObject textHolder = new GameObject("Health Label");
        assignTo = textHolder.AddComponent<TextMeshPro>();
        assignTo.fontSize = 4;
        assignTo.fontStyle = FontStyles.Bold;
        assignTo.alignment = TextAlignmentOptions.Center;

        RectTransform textContainer = assignTo.GetComponent<RectTransform>();
        textContainer.sizeDelta = new Vector2(2, 1);
        textContainer.anchoredPosition = new Vector3(0, 2.6F, -0.5F) + offset;
        textHolder.transform.SetParent(gameObject.transform, false);
        textHolder.layer = textHolder.transform.parent.gameObject.layer;
    }

    public override string GetCardId()
    {
        return TARGET_ID_FACE;
    }

    public override string GetPlayerId()
    {
        return this.owner.Id;
    }

    public override bool CanAttackNow()
    {
        return false;
    }

    public override void OnStartTurn()
    {
        this.canAttack = this.maxAttacks;
    }

    public bool HasWeapon()
    {
        return this.weapon != null;
    }

    //returns true if replaced existing weapon, false if not
    public bool EquipWeapon(WeaponCard weapon)
    {
        GameObject created = new GameObject(weapon.Name);
        BoardWeapon weaponObject = created.AddComponent<BoardWeapon>();
        weaponObject.Initialize(this.owner, weapon);

        this.weapon = weaponObject;
        return false;
    }

    public int Fight(Targetable other)
    {
        int damageDone = 0;

        if (this.canAttack <= 0)
        {
            Debug.LogError("Fight called when canAttack is 0 or below!");
            return 0;
        }
        if (!this.HasWeapon())
        {
            Debug.LogError("Fight called when avatar has no weapon!");
            return 0;
        }

        this.canAttack -= 1;
        //move/animate
        Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
        LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
        //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

        FXPoolManager.Instance.PlayEffect("SlashVFX", other.transform.position);
        SoundManager.Instance.PlaySound("SlashSFX", other.transform.position);
        damageDone = this.weapon.AttackMade(other); //diff from boardcreature fight!!

        if (!other.IsAvatar)
        {
            FXPoolManager.Instance.PlayEffect("SlashVFX", this.transform.position);
            this.TakeDamage(((BoardCreature)other).Attack);

            if (((BoardCreature)other).HasAbility(Card.CARD_ABILITY_TAUNT))  //to-do this string should be chosen from some dict set by text file later                
                SoundManager.Instance.PlaySound("HitTauntSFX", other.transform.position);
        }

        this.UpdateStatText();
        return damageDone;
    }

    public override int TakeDamage(int amount)
    {
        //to-do check immunity
        LeanTween.scale(gameObject, transform.localScale * 1.1f, 1).setEasePunch();
        this.audioSource.Play();

        int healthBefore = this.health;
        this.health -= amount;

        this.UpdateStatText();
        int damageTaken = Math.Min(healthBefore, amount);
        TextManager.Instance.ShowTextAtTarget(this.transform, damageTaken.ToString(), Color.red);

        return damageTaken;
    }

    public override int Heal(int amount)
    {
        int healthBefore = this.health;
        this.health += amount;
        this.health = Math.Min(this.health, this.maxHealth);

        int amountHealed = Math.Min(this.health - healthBefore, amount);
        if (amountHealed > 0)
        {
            // TODO: animate.
        }

        this.UpdateStatText();
        return amountHealed;
    }

    public void Die()
    {
        StartCoroutine("Dissolve", 2);
    }

    private IEnumerator Dissolve(float duration)
    {
        SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            spriteRenderer.material.SetFloat("_Progress", Mathf.Lerp(1, 0, (elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void Redraw()
    {
        UpdateStatText();
    }

    private void UpdateStatText()
    {
        float scaleFactor = 1.6f;
        LeanTween.scale(healthLabel.gameObject, new Vector3(scaleFactor, scaleFactor, scaleFactor), 1).setEasePunch();
        healthLabel.text = String.Format("{0}/{1}", this.health, this.maxHealth);
    }
}
