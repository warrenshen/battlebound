using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BoardCreature : MonoBehaviour
{

    [SerializeField]
    private string uid;     //for server unique tracking/reference?
    public string Uid => uid;

    [SerializeField]
    private int cost;           //cost retained for conditional removal cards
    public int Cost => cost;    //e.g. remove all cards with cost 2 or less

    [SerializeField]
    private int attack;
    public int Attack => attack;

    [SerializeField]
    private int health;
    public int Health => health;

    [SerializeField]
    private string image;
    public string Image => image;

    [SerializeField]
    private bool canAttack;
    public bool CanAttack => canAttack;

    private int attacksThisTurn;

    [SerializeField]
    private Player owner;
    public Player Owner => owner;

    private SpriteRenderer sp;
    private CreatureCard card;
    public CreatureCard Card => card;

    [SerializeField]
    private List<string> abilities;

    Material dissolve;
    TextMeshPro textMesh;

    public void Initialize(CardObject cardObject, Player owner)
    {
        this.card = cardObject.card as CreatureCard;

        this.uid = this.card.Id;
        this.cost = this.card.Cost;
        this.attack = this.card.Attack;
        this.health = this.card.Health;
        this.image = this.card.Image;
        this.abilities = this.card.Abilities;

        this.canAttack = false;
        this.attacksThisTurn = 0;

        this.owner = owner;
        this.gameObject.layer = 9;

        //Render everything (labels, image, etc) method call here
        sp = gameObject.AddComponent<SpriteRenderer>();
        sp.sortingOrder = -1;
        Texture2D texture = cardObject.Renderer.sprite.texture;
        sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 80.0f);
        BoxCollider coll = gameObject.AddComponent<BoxCollider>() as BoxCollider;

        //maybe do this manually in prefab later
        GameObject textHolder = new GameObject();
        textMesh = textHolder.AddComponent<TextMeshPro>();
        textMesh.fontSize = 6;
        textMesh.fontStyle = FontStyles.Bold;
        RectTransform textContainer = textMesh.GetComponent<RectTransform>();
        textContainer.sizeDelta = new Vector2(3, 2);
        textContainer.anchoredPosition = new Vector3(0.8f, 1.5f, -0.5f);
        textHolder.transform.SetParent(gameObject.transform, false);

        //ehh
        dissolve = Resources.Load("Dissolve", typeof(Material)) as Material;
        sp.material = dissolve;

        //method calls
        this.UpdateStatText();
        this.RenderAbilities();
    }

    public void Fight(BoardCreature other)
    {
        this.health -= other.attack;
        other.health -= this.attack;

        //to-do this string should be chosen from some dict set by text file later
        FXPoolManager.Instance.PlayEffect("Slash", this.transform.position);
        FXPoolManager.Instance.PlayEffect("Slash", other.transform.position);

        if (other.Taunt())
            SoundManager.Instance.PlaySound("HitTaunt", other.transform.position);
        else
            SoundManager.Instance.PlaySound("Splatter", other.transform.position);

        Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
        LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
        //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

        if (this.CheckAlive())
            this.UpdateStatText();
        if (other.CheckAlive())
            other.UpdateStatText();
    }

    //taking damage
    public bool TakeDamage(int amount)
    {
        if (this.Shielded())
        {
            abilities.Remove("shielded");
            RenderAbilities(); //to-do: needs review in future
            return false;
        }

        this.health -= amount;
        if (CheckAlive())
        {
            this.UpdateStatText();
        }
        return true; //true implies did take damage, e.g. for poison
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
            Board.Instance().RemoveCreature(this);
            FXPoolManager.Instance.PlayEffect("CreatureDeath", this.transform.position);
            StartCoroutine("Dissolve", 2);
            return false;
        }
    }

    private IEnumerator Dissolve(float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            sp.material.SetFloat("_Progress", Mathf.Lerp(1, 0, (elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void UpdateStatText()
    {
        textMesh.text = String.Format("{0} / {1}", this.attack, this.health);
    }

    private void RenderAbilities()
    {
        if (this.Taunt())
            this.transform.localScale *= 1.2f;  //to-do, do better than this pls
    }

    public bool Taunt()
    {
        return abilities != null && abilities.Contains("taunt");
    }

    public bool Shielded()
    {
        return abilities != null && abilities.Contains("shielded");
    }

}
