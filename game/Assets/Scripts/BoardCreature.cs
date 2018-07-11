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

    //removed "image" field because this reuses BattleCardObject initialized visuals

    //int canAttack / CanAttack in Targetable class
    //int maxAttacks in Targetable class

    private int attacksThisTurn;

    //Player owner / Owner exists in Targetable class

    private CreatureCard creatureCard;
    public CreatureCard CreatureCard => creatureCard;

    private List<string> buffs;
    public List<string> Buffs => buffs;

    [SerializeField]
    private List<string> abilities;
    private Dictionary<string, GameObject> abilitiesFX;

    private bool silenced;
    public bool Silenced => silenced;

    private bool hasShield;
    public bool HasShield => hasShield;

    Material dissolve;

    private HyperCard.Card visual;
    private const float BOARD_GROW_FACTOR = 1.25f;

    private string summonPrefabPath;
    private Animation summonAnimation;
    private List<AnimationState> summonAnimStates;


    public void Initialize(BattleCardObject battleCardObject)
    {
        //data structure stuff
        this.creatureCard = battleCardObject.Card as CreatureCard;
        this.cost = this.creatureCard.Cost;
        this.attack = this.creatureCard.Attack;
        this.health = this.creatureCard.Health;
        this.maxHealth = this.creatureCard.Health;
        this.abilities = new List<string>(battleCardObject.TemplateData.abilities);
        this.summonPrefabPath = battleCardObject.TemplateData.summonPrefab;

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

        if (this.abilities.Contains(Card.CARD_ABILITY_SHIELD))
        {
            this.hasShield = true;
        }
        else
        {
            this.hasShield = false;
        }

        this.owner = battleCardObject.Owner;
        this.gameObject.layer = 9;

        if (this.abilities != null)
        {
            this.abilitiesFX = new Dictionary<string, GameObject>();
        }

        BoxCollider newCollider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        BoxCollider oldCollider = battleCardObject.GetComponent<BoxCollider>() as BoxCollider;
        newCollider.size = oldCollider.size + new Vector3(0, 0, 1);
        newCollider.size *= BOARD_GROW_FACTOR;
        newCollider.center = oldCollider.center;

        this.visual = battleCardObject.visual;
        this.RepurposeCardVisual();
        this.SummonCreature();

        //method calls
        this.Redraw();

        //post-collider-construction visuals
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.5f).setEaseOutBack();

        this.buffs = new List<string>();
    }

    private void RepurposeCardVisual()
    {
        this.visual.gameObject.SetLayer(9);

        this.visual.transform.parent = this.transform;
        this.visual.transform.localPosition = Vector3.zero;
        this.visual.transform.localRotation = Quaternion.identity;
        this.visual.transform.Rotate(0, 180, 0, Space.Self);
        this.visual.transform.localScale = this.visual.reset.scale * BOARD_GROW_FACTOR;

        //this.visual.TmpTextObjects[0].TmpObject.enabled = false;
        //this.visual.TmpTextObjects[1].TmpObject.enabled = false;
        this.visual.SetBlackAndWhite(true);
        this.visual.Renderer.enabled = true;
    }

    private void SummonCreature()
    {
        SoundManager.Instance.PlaySound("SummonSFX", transform.position);

        GameObject created = Instantiate(Resources.Load(this.summonPrefabPath)) as GameObject;
        created.transform.parent = this.transform;
        created.transform.localPosition = new Vector3(0, 0, -0.3f);
        created.transform.Rotate(-15, 0, 0, Space.Self);

        this.summonAnimation = created.transform.GetChild(0).GetComponent<Animation>();
        this.summonAnimStates = new List<AnimationState>();
        foreach (AnimationState state in this.summonAnimation)
        {
            this.summonAnimStates.Add(state);
        }
        this.summonAnimation.Play(summonAnimStates[0].name);
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

        FXPoolManager.Instance.PlayEffect("SlashVFX", other.transform.position);
        StartCoroutine("PlaySoundWithDelay", new object[3] { "PunchSFX", other.transform.position, 0.25f });
        StartCoroutine("PlaySoundWithDelay", new object[3] { "SlashSFX", other.transform.position, 0.4f });

        int damageDone = other.TakeDamage(this.attack);
        OnDamageDone(damageDone);

        if (!other.IsAvatar)
        {
            FXPoolManager.Instance.PlayEffect("SlashVFX", this.transform.position);
            this.TakeDamage(((BoardCreature)other).Attack);

            if (((BoardCreature)other).HasAbility(Card.CARD_ABILITY_TAUNT))  //to-do this string should be chosen from some dict set by text file later
                StartCoroutine("PlaySoundWithDelay", new object[3] { "HitTauntSFX", other.transform.position, 0.5f });
        }

        this.Redraw();
    }

    IEnumerator PlaySoundWithDelay(object[] args)
    {
        string spellName = (string)args[0];
        Vector3 location = (Vector3)args[1];
        float delay = (float)args[2];

        yield return new WaitForSeconds(delay);
        SoundManager.Instance.PlaySound(spellName, location);
    }

    /*
     * @return int - amount of damage taken
     */
    public override int TakeDamage(int amount)
    {
        if (this.hasShield)
        {
            this.hasShield = false;
            Redraw();
            return 0;
        }
        else
        {
            int healthBefore = this.health;
            this.health -= amount;
            this.health = Math.Max(this.health, 0);

            if (CheckAlive())
            {
                //do something
            }

            Redraw();
            return Math.Min(healthBefore, amount);
        }
    }

    /*
     * @return int - amount of health healed
     */
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

        this.Redraw();
        return amountHealed;
    }

    public override void OnStartTurn()
    {
        this.canAttack = this.maxAttacks;

        if (HasBuff(Card.BUFF_CATEGORY_UNSTABLE_POWER))
        {
            this.health = 0;
        }

        CheckAlive();
        this.Redraw();
    }

    public override void OnEndTurn()
    {
        if (HasAbility(Card.CARD_ABILITY_END_TURN_HEAL_TEN))
        {
            Heal(10);
        }
        else if (HasAbility(Card.CARD_ABILITY_END_TURN_HEAL_TWENTY))
        {
            Heal(20);
        }

        if (HasAbility(Card.CARD_ABILITY_END_TURN_DRAW_CARD))
        {
            if (!InspectorControlPanel.Instance.DevelopmentMode)
            {
                this.Owner.DrawCards(1);
            }
        }

        this.Redraw();
    }

    public void OnPlay()
    {
        if (HasAbility(Card.CARD_ABILITY_BATTLE_CRY_DRAW_CARD))
        {
            if (!InspectorControlPanel.Instance.DevelopmentMode)
            {
                this.Owner.DrawCards(1);
            }
        }

        this.Redraw();
    }

    private void OnDeath()
    {
        if (HasAbility(Card.CARD_ABILITY_DEATH_RATTLE_DRAW_CARD))
        {
            if (!InspectorControlPanel.Instance.DevelopmentMode)
            {
                this.Owner.DrawCards(1);
            }
        }

        if (HasAbility(Card.CARD_ABILITY_DEATH_RATTLE_ATTACK_FACE_TWENTY))
        {
            // TODO: animate.
            // TODO: attack opponent face.
        }

        this.Redraw();
    }

    private void OnDamageDone(int amount)
    {
        if (HasAbility(Card.CARD_ABILITY_LIFE_STEAL))
        {
            this.Owner.Heal(amount);
        }

        this.Redraw();
    }

    private void OnKill()
    {
        if (HasAbility(Card.CARD_ABILITY_EACH_KILL_DRAW_CARD))
        {
            if (!InspectorControlPanel.Instance.DevelopmentMode)
            {
                this.Owner.DrawCards(1);
            }
        }

        this.Redraw();
    }

    public void AddAttack(int amount)
    {
        this.attack += amount;
        // TODO: animate.
        Redraw();
    }

    private bool CheckAlive()
    {
        if (this.health > 0)
        {
            return true;
        }
        else
        {
            //to-do, delay creature death
            Board.Instance.RemoveCreature(this);
            StartCoroutine("Dissolve", 2);
            return false;
        }
    }

    private IEnumerator Dissolve(float duration)
    {
        FXPoolManager.Instance.PlayEffect("CreatureDeathVFX", this.transform.position);
        SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);

        float elapsedTime = 0;
        LeanTween.scale(this.summonAnimation.gameObject, Vector3.zero, duration / 3).setEaseInOutCubic();
        while (elapsedTime < duration)
        {
            this.visual.BurningAmount = Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void Redraw()
    {
        UpdateStatText();
        this.visual.SetOutline(this.Owner.HasTurn && this.canAttack > 0);
        this.visual.Redraw();
        RenderAbilitiesAndBuffs();
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

    private void RenderAbilitiesAndBuffs()
    {
        //if (abilities == null)
        //return;

        if (this.hasShield && !this.abilitiesFX.ContainsKey(Card.CARD_ABILITY_SHIELD))
        {
            this.abilitiesFX[Card.CARD_ABILITY_SHIELD] = FXPoolManager.Instance.AssignEffect(Card.CARD_ABILITY_SHIELD, this.transform).gameObject;
        }
        else if (!this.hasShield && this.abilitiesFX.ContainsKey(Card.CARD_ABILITY_SHIELD))
        {
            GameObject effect = abilitiesFX[Card.CARD_ABILITY_SHIELD];
            FXPoolManager.Instance.UnassignEffect(Card.CARD_ABILITY_SHIELD, effect, this.transform);
        }

        //doing additions
        foreach (string ability in abilities)
        {
            if (abilitiesFX.ContainsKey(ability))
            {
                continue;
            }
            if (!FXPoolManager.Instance.HasEffect(ability))
            {
                continue;
            }

            abilitiesFX[ability] = FXPoolManager.Instance.AssignEffect(ability, this.transform).gameObject;
        }

        //do removals
        foreach (string key in abilitiesFX.Keys)
        {
            if (abilities.Contains(key))
            {
                continue;
            }

            //if not continue, needs removal
            GameObject effect = abilitiesFX[key];
            FXPoolManager.Instance.UnassignEffect(key, effect, this.transform);
        }
    }

    public void AddAbility(string ability)
    {
        if (abilities == null)
        {
            this.abilities = new List<string>();
            this.abilitiesFX = new Dictionary<string, GameObject>();
        }
        abilities.Add(ability);
        RenderAbilitiesAndBuffs();
    }

    public bool HasAbility(string ability)
    {
        if (this.silenced)
        {
            return false;
        }

        return this.abilities.Contains(ability);
    }

    public void AddBuff(string buff)
    {
        if (Array.IndexOf(Card.VALID_BUFFS, buff) < 0)
        {
            Debug.LogError("Invalid buff.");
            return;
        }

        this.buffs.Add(buff);
    }

    public bool HasBuff(string buff)
    {
        return this.buffs.Contains(buff);
    }
}
