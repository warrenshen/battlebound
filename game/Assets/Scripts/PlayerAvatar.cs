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

    private SpriteRenderer spriteRenderer;
    private bool frozen;
    public bool Frozen => frozen;

    TextMeshPro textMesh;

    public void Initialize(Player player)
    {
        this.armor = 0;
        this.maxHealth = 30;
        this.health = this.maxHealth;

        this.weapon = null;
        this.isAvatar = true;

        this.owner = player;

        //sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 80.0f);

        this.maxAttacks = 1;
        this.canAttack = 1;

        InitializeRender();
    }

    public void Initialize(Player player, PlayerState playerState)
    {
        this.armor = playerState.Armor;
        this.maxHealth = playerState.Health;
        this.health = this.maxHealth;

        this.weapon = null;
        this.isAvatar = true;

        this.owner = player;

        //sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 80.0f);

        this.maxAttacks = 1;
        this.canAttack = 1;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -5;

        InitializeRender();
    }

    private void InitializeRender()
    {
        //maybe do this manually in prefab later
        GameObject textHolder = new GameObject("Text Label");
        textMesh = textHolder.AddComponent<TextMeshPro>();
        textMesh.fontSize = 4;
        textMesh.fontStyle = FontStyles.Bold;
        textMesh.alignment = TextAlignmentOptions.Center;
        RectTransform textContainer = textMesh.GetComponent<RectTransform>();
        textContainer.sizeDelta = new Vector2(2, 1);
        textContainer.anchoredPosition = new Vector3(0, 1.85f, -0.5f);
        textHolder.transform.SetParent(gameObject.transform, false);

        //already has cylinder collider in scene
        UpdateStatText();
    }

    public override string GetCardId()
    {
        return TARGET_ID_FACE;
    }

    public override string GetPlayerId()
    {
        return this.owner.Id;
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

    public override void Fight(Targetable other)
    {
        if (this.canAttack <= 0)
        {
            Debug.LogError("Fight called when canAttack is 0 or below!");
            return;
        }
        if (!this.HasWeapon())
        {
            Debug.LogError("Fight called when avatar has no weapon!");
            return;
        }

        this.canAttack -= 1;
        //move/animate
        Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
        LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
        //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

        FXPoolManager.Instance.PlayEffect("Slash", other.transform.position);
        SoundManager.Instance.PlaySound("Splatter", other.transform.position);
        this.weapon.AttackMade(other); //diff from boardcreature fight!!

        if (!other.IsAvatar)
        {
            FXPoolManager.Instance.PlayEffect("Slash", this.transform.position);
            this.TakeDamage(((BoardCreature)other).Attack);

            if (((BoardCreature)other).HasAbility("taunt"))  //to-do this string should be chosen from some dict set by text file later
                SoundManager.Instance.PlaySound("HitTaunt", other.transform.position);
        }
    }

    public override bool TakeDamage(int amount)
    {
        //to-do check immunity
        this.health -= amount;
        if (CheckAlive())
        {
            this.UpdateStatText();
        }
        return true;
    }

    public bool CheckAlive()
    {
        if (this.health > 0)
        {
            return true;
        }
        else
        {
            //to-do, delay creature death
            StartCoroutine("Dissolve", 2);
            return false;
        }
    }

    private IEnumerator Dissolve(float duration)
    {
        SoundManager.Instance.PlaySound("BurnDestroy", this.transform.position);
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            spriteRenderer.material.SetFloat("_Progress", Mathf.Lerp(1, 0, (elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    private void UpdateStatText()
    {
        float scaleFactor = 1.6f;
        LeanTween.scale(textMesh.gameObject, new Vector3(scaleFactor, scaleFactor, scaleFactor), 1).setEasePunch();
        textMesh.text = String.Format("{0}/{1}", this.health, this.maxHealth);
    }
}
