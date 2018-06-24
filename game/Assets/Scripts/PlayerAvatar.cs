using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using TMPro;

public class PlayerAvatar : MonoBehaviour {
    
    [SerializeField]
    private int armor;
    public int Armor => armor;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private BoardWeapon weapon;
    public BoardWeapon Weapon => weapon;

    [SerializeField]
    private string avatar;
    public string Avatar => avatar;

    private int maxAttacks;
    [SerializeField]
    private int canAttack;
    public int CanAttack => canAttack;

    [SerializeField]
    private Player player;
    public Player Player => player;

    private SpriteRenderer spriteRenderer;
    private bool frozen;
    public bool Frozen => frozen;

    TextMeshPro textMesh;


    public void Initialize(Player player)
    {
        this.armor = 0;
        this.health = 30;
        this.weapon = null;
        //to-do: load avatar from player
        //sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 80.0f);

        this.maxAttacks = 1;
        this.canAttack = 1;

        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -5;

        //maybe do this manually in prefab later
        GameObject textHolder = new GameObject("Text Label");
        textMesh = textHolder.AddComponent<TextMeshPro>();
        textMesh.fontSize = 6;
        textMesh.fontStyle = FontStyles.Bold;
        textMesh.alignment = TextAlignmentOptions.Center;
        RectTransform textContainer = textMesh.GetComponent<RectTransform>();
        textContainer.sizeDelta = new Vector2(3, 2);
        textContainer.anchoredPosition = new Vector3(0, 2.2f, -0.5f);
        textHolder.transform.SetParent(gameObject.transform, false);

        //already has cylinder collider in scene
    }

    public void Fight(BoardCreature other)
    {
        if (this.canAttack <= 0)
        {
            Debug.LogError("Fight called when canAttack is 0 or below!");
            return;
        }
        if(this.weapon == null)
        {
            Debug.LogError("Avatar Fight() called without posession of a weapon.");
        }
        this.canAttack -= 1;
        //move/animate
        Vector3 delta = (this.transform.position - other.transform.position) / 1.2f;
        LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
        //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

        //to-do this string should be chosen from some dict set by text file later
        FXPoolManager.Instance.PlayEffect("Slash", this.transform.position);
        FXPoolManager.Instance.PlayEffect("Slash", other.transform.position);

        if (other.HasAbility("taunt"))
            SoundManager.Instance.PlaySound("HitTaunt", other.transform.position);
        else
            SoundManager.Instance.PlaySound("Splatter", other.transform.position);

        this.weapon.MakeAttack(other);
        this.TakeDamage(other.Attack);
    }

    public bool TakeDamage(int amount)
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

    public void UpdateStatText()
    {
        textMesh.text = String.Format("{0} hp", this.health);
    }

	// Update is called once per frame
	void Update ()
    {
		
	}
}
