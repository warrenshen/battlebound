using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class BoardCreature : Targetable
{
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
    protected int maxHealth;
    public int MaxHealth => maxHealth;

    [SerializeField]
    private string image;
    public string Image => image;

    //int canAttack / CanAttack in Targetable class
    //int maxAttacks in Targetable class

    private int attacksThisTurn;

    //Player owner / Owner exists in Targetable class

    private CreatureCard creatureCard;
    public CreatureCard CreatureCard => creatureCard;

    [SerializeField]
    private List<string> abilities;
    private Dictionary<string, GameObject> abilitiesFX;

    private bool silenced;
    public bool Silenced => silenced;

    Material dissolve;

    private HyperCard.Card visual;


    public void Initialize(CardObject cardObject)
    {
        //data structure stuff
        this.creatureCard = cardObject.Card as CreatureCard;
        this.cost = this.creatureCard.Cost;
        this.attack = this.creatureCard.Attack;
        this.health = this.creatureCard.Health;
        this.maxHealth = this.creatureCard.Health;
        this.image = this.creatureCard.Image;
        this.abilities = this.creatureCard.Abilities;

        if (this.abilities.Contains(Card.CARD_ABILITY_CHARGE))
        {
            this.canAttack = 1;
        }
        else
        {
            this.canAttack = 0;
        }

        this.maxAttacks = 1;
        this.attacksThisTurn = 0;

        this.owner = cardObject.Owner;
        this.gameObject.layer = 9;

        if (this.abilities != null)
        {
            this.abilitiesFX = new Dictionary<string, GameObject>();
        }

        //Render everything (labels, image, etc) method call here
        //sp = gameObject.AddComponent<SpriteRenderer>();
        //sp.sortingOrder = -1;
        //Texture2D texture = cardObject.Renderer.sprite.texture;
        //sp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 80.0f);

        BoxCollider coll = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        coll.size += new Vector3(0, 0, 1.2f);
        coll.center += new Vector3(0, 0, -0.45f);

        //ehh
        //dissolve = Resources.Load("Dissolve", typeof(Material)) as Material;
        //sp.material = dissolve;
        this.visual = cardObject.visual;
        RepurposeCardVisual();

        //method calls
        this.Redraw();
        this.RenderAbilities(); //this should go inside redraw

        //post-collider-construction visuals
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.5f).setEaseOutBack();
    }

    private void RepurposeCardVisual()
    {
        this.visual.transform.parent = this.transform;
        this.visual.transform.localPosition = Vector3.zero;
        this.visual.transform.localRotation = Quaternion.identity;
        this.visual.transform.Rotate(0, 180, 0, Space.Self);
        this.visual.Renderer.enabled = true;
    }

    public override string GetCardId()
    {
        return this.creatureCard.Id;
    }

    public override string GetPlayerId()
    {
        return this.owner.Id;
    }

    public override void Fight(Targetable other)
    {
        if (this.canAttack <= 0)
        {
            Debug.LogError("Fight called when canAttack is 0 or below!");
            return;
        }

        this.canAttack -= 1;
        //move/animate
        Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
        LeanTween.move(this.gameObject, this.transform.position - delta, 1).setEasePunch();
        //LeanTween.move(other.gameObject, other.transform.position + delta, 1).setEasePunch();

        FXPoolManager.Instance.PlayEffect("Slash", other.transform.position);
        SoundManager.Instance.PlaySound("Splatter", other.transform.position);
        other.TakeDamage(this.attack);

        if (!other.IsAvatar)
        {
            FXPoolManager.Instance.PlayEffect("Slash", this.transform.position);
            this.TakeDamage(((BoardCreature)other).Attack);

            if (((BoardCreature)other).HasAbility("taunt"))  //to-do this string should be chosen from some dict set by text file later
                SoundManager.Instance.PlaySound("HitTaunt", other.transform.position);
        }
        this.Redraw();
    }

    //taking damage
    public override bool TakeDamage(int amount)
    {
        if (this.HasAbility("shielded"))
        {
            this.abilities.Remove("shielded");
            RenderAbilities(); //to-do: needs review in future
            return false;
        }

        this.health -= amount;
        if (CheckAlive())
        {
            //do something
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
        SoundManager.Instance.PlaySound("BurnDestroy", this.transform.position);
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            //sp.material.SetFloat("_Progress", Mathf.Lerp(1, 0, (elapsedTime / duration)));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void Redraw()
    {
        this.visual.SetVisualOutline(canAttack > 0);
        UpdateStatText();
        this.visual.Redraw();
    }

    private void UpdateStatText()
    {
        //0 = title, 1 = description, 2 = price, 3 = attack, 4 = health
        this.visual.TmpTextObjects[0].Value = this.name;
        this.visual.TmpTextObjects[2].Value = this.cost.ToString();
        this.visual.TmpTextObjects[3].Value = this.attack.ToString();
        this.visual.TmpTextObjects[4].Value = this.health.ToString();

        //float scaleFactor = 1.6f;
        //LeanTween.scale(textMesh.gameObject, new Vector3(scaleFactor, scaleFactor, scaleFactor), 1).setEasePunch();
    }

    new public void RecoverAttack()
    {
        this.canAttack = this.maxAttacks;
        this.Redraw();
    }

    private void RenderAbilities()
    {
        if (abilities == null)
            return;

        //doing additions
        foreach (string ability in abilities)
        {
            if (abilitiesFX.ContainsKey(ability))
                continue;
            if (!FXPoolManager.Instance.HasEffect(ability))
                continue;
            abilitiesFX[ability] = FXPoolManager.Instance.AssignEffect(ability, this.transform).gameObject;
        }
        //do removals
        foreach (string key in abilitiesFX.Keys)
        {
            if (abilities.Contains(key))
                continue;
            //if not continue, needs removal
            GameObject effect = abilitiesFX[key];
            FXPoolManager.Instance.UnassignEffect(key, effect, this.transform);
        }
    }

    public void AddAbility(string ability)
    {
        if (abilities == null)
        {
            abilities = new List<string>();
            abilitiesFX = new Dictionary<string, GameObject>();
        }
        abilities.Add(ability);
        RenderAbilities();
    }

    public bool HasAbility(string ability)
    {
        if (this.silenced)
            return false;
        switch (ability)
        {
            case "taunt":
                return abilities != null && abilities.Contains("taunt");
            case "shielded":
                return abilities != null && abilities.Contains("shielded");
            default:
                return false;
        }
    }
}
