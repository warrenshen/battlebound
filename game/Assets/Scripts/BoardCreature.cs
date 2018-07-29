using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class BoardCreature : Targetable
{
    public const float UPDATE_STATS_GROWTH_FACTOR = 1.4F;
    public const float ATTACK_DELAY = 0.66F;

    private static Vector3 BOARD_CARD_SIZE = new Vector3(5, 3.7F, 1);
    private static Vector3 INSPECT_CARD_SIZE = new Vector3(5, 4.28F, 1);

    public static Color LIGHT_GREEN = new Color(0.33F, 1, 0.33F);
    public static Color LIGHT_RED = new Color(1, 0.33F, 0.33F);

    public const string FROZEN_STATUS = "FROZEN_STATUS";

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

    //Player owner / Owner exists in Targetable class

    private CreatureCard creatureCard;
    public CreatureCard CreatureCard => creatureCard;

    private List<string> buffs;
    public List<string> Buffs => buffs;

    [SerializeField]
    private List<string> abilities;
    public List<string> Abilities => abilities;

    private Dictionary<string, GameObject> abilitiesVFX;
    private Dictionary<string, GameObject> statusVFX;

    private bool isSilenced;
    public bool IsSilenced => isSilenced;

    private int isFrozen;
    public int IsFrozen => isFrozen;

    private bool raisedCard;

    Material dissolve;

    private HyperCard.Card visual;
    private const float BOARD_GROW_FACTOR = 1.25f;

    private GameObject summoned;
    private dynamic summonAnimation;
    private List<string> summonAnimClips;

    private AudioSource[] audioSources;

    private int spawnRank;
    public int SpawnRank => spawnRank;

    private Color initialOutlineStartColor;
    private Color initialOutlineEndColor;


    public void Initialize(BattleCardObject battleCardObject, Player owner, int spawnRank)
    {
        this.owner = owner;
        this.creatureCard = battleCardObject.Card as CreatureCard;

        this.cost = battleCardObject.GetCost();
        this.attack = this.creatureCard.GetAttack();
        this.health = this.creatureCard.GetHealth();
        this.maxHealth = this.creatureCard.GetHealth();
        this.abilities = this.creatureCard.GetAbilities();

        if (this.abilities.Contains(Card.CARD_ABILITY_CHARGE))
        {
            this.canAttack = 1;
        }
        else
        {
            this.canAttack = 0;
        }

        this.isFrozen = 0;
        this.isSilenced = false;

        this.spawnRank = spawnRank;

        InitializeHelper();
    }

    public void InitializeFromChallengeCard(
        BattleCardObject battleCardObject,
        ChallengeCard challengeCard
    )
    {
        this.owner = battleCardObject.Owner;
        this.creatureCard = battleCardObject.Card as CreatureCard;

        // Use challenge card stats.
        this.cost = challengeCard.Cost;
        this.attack = challengeCard.Attack;
        this.health = challengeCard.Health;
        this.maxHealth = challengeCard.HealthMax;
        this.abilities = challengeCard.GetAbilities();
        this.canAttack = challengeCard.CanAttack;

        this.isFrozen = challengeCard.IsFrozen;
        this.isSilenced = challengeCard.IsSilenced == 1;

        this.spawnRank = challengeCard.SpawnRank;

        InitializeHelper();
        Summon(battleCardObject);
    }

    private void InitializeHelper()
    {
        this.maxAttacks = 1;

        this.gameObject.layer = 9;

        this.statusVFX = new Dictionary<string, GameObject>();
        this.abilitiesVFX = new Dictionary<string, GameObject>();

        this.buffs = new List<string>();
    }

    public void Summon(BattleCardObject battleCardObject)
    {
        BoxCollider newCollider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
        BoxCollider oldCollider = battleCardObject.GetComponent<BoxCollider>() as BoxCollider;
        newCollider.size = oldCollider.size + new Vector3(0, 0, 1);
        newCollider.size *= BOARD_GROW_FACTOR;
        newCollider.center = oldCollider.center;

        this.RepurposeCardVisual(battleCardObject);
        this.SummonCreature();

        //method calls
        Redraw();

        //post-collider-construction visuals
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.5f).setEaseOutBack();
    }

    private void RepurposeCardVisual(BattleCardObject previousOwner)
    {
        this.visual = previousOwner.visual;
        previousOwner.visual = null;

        this.visual.gameObject.SetLayer(9);

        this.visual.transform.parent = this.transform;
        this.visual.transform.localPosition = Vector3.zero;
        this.visual.transform.localRotation = Quaternion.identity;
        this.visual.transform.Rotate(0, 180, 0, Space.Self);
        this.visual.transform.localScale = BOARD_CARD_SIZE;

        this.initialOutlineStartColor = this.visual.OutlineColor;
        this.initialOutlineEndColor = this.visual.OutlineEndColor;

        this.visual.SetOpacity(0.8f);
        this.visual.SetBlackAndWhite(true);
        this.visual.Renderer.enabled = true;
    }

    private void SummonCreature()
    {
        SoundManager.Instance.PlaySound("SummonSFX", transform.position);

        GameObject prefab = ResourceSingleton.Instance.GetCreaturePrefabByName(this.name);

        GameObject created = Instantiate(prefab) as GameObject;
        created.transform.parent = this.transform;
        created.transform.localPosition = new Vector3(0, 0, -0.3f);
        created.transform.Rotate(-15, 0, 0, Space.Self);
        //created.transform.LookAt(Camera.main.transform);

        if (this.owner.Id != BattleManager.Instance.You.Id)
        {
            created.transform.Rotate(30, 180, 0, Space.Self);
        }

        this.audioSources = created.GetComponents<AudioSource>();
        this.summonAnimClips = new List<string>();

        this.summoned = created.transform.GetChild(0).gameObject;
        this.summonAnimation = this.summoned.GetComponent<Animation>();
        if (this.summonAnimation != null)
        {
            foreach (AnimationState state in this.summonAnimation)
            {
                state.speed = 5f;
                this.summonAnimClips.Add(state.clip.name);
            }
        }
        else
        {
            this.summonAnimation = this.summoned.GetComponent<Animator>();
            if (this.summonAnimation == null)
            {
                Debug.LogError(String.Format("No animation or animator on gameobject: {0}", this.gameObject.name));
            }
            foreach (AnimationClip clip in this.summonAnimation.runtimeAnimatorController.animationClips)
            {
                this.summonAnimClips.Add(clip.name);
            }
            this.summonAnimation.speed = 1.33f;
        }
        this.summonAnimation.Play(this.summonAnimClips[0]);
        this.summonAnimation.CrossFade(this.summonAnimClips[1], 1F);
    }

    public override string GetCardId()
    {
        return this.creatureCard.Id;
    }

    public override string GetPlayerId()
    {
        return this.owner.Id;
    }

    public override bool CanAttackNow()
    {
        return this.owner.HasTurn && this.isFrozen <= 0 && this.canAttack > 0;
    }

    public void DecrementCanAttack()
    {
        this.canAttack -= 1;
    }

    public void FightAnimationWithCallback(Targetable other, UnityAction onFightFinish)
    {
        if (this.audioSources != null && this.audioSources.Length >= 2 && this.audioSources[1] != null)
        {
            this.audioSources[1].PlayDelayed(BoardCreature.ATTACK_DELAY / 3); //sound
        }
        else
        {
            Debug.LogWarning(string.Format("Missing audio source for card {0}", this.name));
        }

        if (this.summonAnimClips != null && this.summonAnimClips.Count >= 2)
        {
            this.summonAnimation.Play(summonAnimClips[0]);
            this.summonAnimation.CrossFade(summonAnimClips[1], 3F);    //should group with sound as a method
        }
        else
        {
            Debug.LogWarning(string.Format("Missing summon animation for card {0}", this.name));
        }

        //move/animate
        Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
        Vector3 originalPosition = this.summonAnimation.transform.position;

        LeanTween.scale(this.summoned, this.summoned.transform.localScale * 1.2f, 2f).setEasePunch();
        LeanTween
            .move(this.summoned, this.transform.position - delta, 0.3F)
            .setEaseOutCubic()
            .setDelay(BoardCreature.ATTACK_DELAY)
            .setOnComplete(
                () =>
                {
                    LeanTween
                        .move(this.summoned, originalPosition, 0.3F)
                        .setEaseInCubic();
                    onFightFinish();
                }
        );

        StartCoroutine("PlaySoundWithDelay", new object[3] { "PunchSFX", other.transform.position, BoardCreature.ATTACK_DELAY + 0.25f });
        StartCoroutine("PlaySoundWithDelay", new object[3] { "SlashSFX", other.transform.position, BoardCreature.ATTACK_DELAY + 0.4f });

        if (!other.IsAvatar)
        {
            StartCoroutine("PlayVFXWithDelay", new object[3] { "SlashVFX", other.transform.position, BoardCreature.ATTACK_DELAY });

            if (((BoardCreature)other).HasAbility(Card.CARD_ABILITY_TAUNT))  //to-do this string should be chosen from some dict set by text file later
                StartCoroutine("PlaySoundWithDelay", new object[3] { "HitTauntSFX", other.transform.position, BoardCreature.ATTACK_DELAY + 0.5f });
        }
    }

    IEnumerator PlayVFXWithDelay(object[] args)
    {
        string VFXName = (string)args[0];
        Vector3 location = (Vector3)args[1];
        float delay = (float)args[2];

        yield return new WaitForSeconds(delay);
        FXPoolManager.Instance.PlayEffect(VFXName, location);
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
        int damageTaken = 0;

        if (HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            // TODO: play shield pop sound.
            RemoveShield();
        }
        else
        {
            LeanTween.scale(this.summoned, this.summoned.transform.localScale * 1.1f, 1F).setEasePunch();

            int healthBefore = this.health;
            this.health -= amount;

            PlayAudioTakeDamage();

            damageTaken = Math.Min(healthBefore, amount);
            TextManager.Instance.ShowTextAtTarget(this.transform, damageTaken.ToString(), Color.red);
        }

        Redraw();
        return damageTaken;
    }

    public int TakeDamageWithLethal()
    {
        int damageTaken = 0;

        if (HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            // TODO: play shield pop sound.
            RemoveShield();
        }
        else
        {
            int healthBefore = this.health;
            this.health = 0;

            PlayAudioTakeDamage();

            damageTaken = healthBefore;
            TextManager.Instance.ShowTextAtTarget(this.transform, damageTaken.ToString(), Color.red);
        }

        Redraw();
        return damageTaken;
    }

    public void DeathNote()
    {
        if (HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            // TODO: play shield pop sound.
            RemoveShield();
        }

        int healthBefore = this.health;
        this.health = 0;

        PlayAudioTakeDamage();

        Redraw();
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
            TextManager.Instance.ShowTextAtTarget(transform, amountHealed.ToString(), Color.green);
            FXPoolManager.Instance.PlayEffect("HealPillarVFX", transform.position);
        }

        Redraw();
        return amountHealed;
    }

    /*
     * @return int - amount of health healed
     */
    public int HealMax()
    {
        int healthBefore = this.health;
        this.health = this.maxHealth;
        return this.maxHealth - healthBefore;
    }

    public override void OnStartTurn()
    {
        this.canAttack = this.maxAttacks;

        Redraw();
    }

    public void OnEndTurn()
    {
        DecrementIsFrozen();
        Redraw();
    }

    private bool DecrementIsFrozen()
    {
        if (this.isFrozen > 0)
        {
            this.isFrozen -= 1;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetHealth(int amount)
    {
        this.health = amount;
    }

    public void AddAttack(int amount)
    {
        this.attack += amount;
        // TODO: animate.
        this.UpdateStatText();
    }

    public void Die()
    {
        //to-do, delay creature death
        StartCoroutine("Dissolve", 2);
    }

    private IEnumerator Dissolve(float duration)
    {
        this.noInteraction = true;
        FXPoolManager.Instance.PlayEffect("CreatureDeathVFX", this.transform.position);
        SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);

        float elapsedTime = 0;
        LeanTween.scale(this.summoned, Vector3.zero, duration / 3).setEaseInOutCubic();
        while (elapsedTime < duration)
        {
            this.visual.BurningAmount = Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public void GrantShield()
    {
        if (!HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            this.abilities.Add(Card.CARD_ABILITY_SHIELD);
            Redraw();
        }
    }

    private void RemoveShield()
    {
        if (!HasAbility(Card.CARD_ABILITY_SHIELD))
        {
            Debug.LogError("Remove shield called on creature without shield.");
            return;
        }

        this.abilities.Remove(Card.CARD_ABILITY_SHIELD);
    }

    public void GrantTaunt()
    {
        if (!HasAbility(Card.CARD_ABILITY_TAUNT))
        {
            this.abilities.Add(Card.CARD_ABILITY_TAUNT);
            Redraw();
        }
    }

    public void Freeze(int amount)
    {
        this.isFrozen += amount;
        Redraw();
    }

    public void Silence()
    {
        this.isSilenced = true;
        Redraw();
    }

    public void Redraw()
    {
        UpdateStatText();

        this.visual.SetOutline(CanAttackNow());
        this.visual.Redraw();

        RenderStatus();
        RenderAbilitiesAndBuffs();
    }

    private void UpdateStatText()
    {
        if (this.visual == null)
        {
            return;
        }

        HyperCard.Card.TextMeshProParam attackText = this.visual.GetTextFieldWithKey("Attack");
        HyperCard.Card.TextMeshProParam healthText = this.visual.GetTextFieldWithKey("Health");

        if (!this.visual.GetTextFieldWithKey("Cost").Value.Equals(this.cost.ToString()))
        {
            LeanTween.scale(this.visual.GetTextFieldWithKey("Cost").TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }
        if (!attackText.Value.Equals(this.attack.ToString()))
        {
            LeanTween.scale(attackText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }
        if (!healthText.Value.Equals(this.health.ToString()))
        {
            LeanTween.scale(healthText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }

        this.visual.SetTextFieldWithKey("Title", this.name);
        this.visual.SetTextFieldWithKey("Cost", this.cost.ToString());
        this.visual.SetTextFieldWithKey("Attack", this.attack.ToString());
        this.visual.SetTextFieldWithKey("Health", this.health.ToString());

        if (this.attack > creatureCard.GetAttack())
        {
            attackText.TmpObject.color = BoardCreature.LIGHT_GREEN;
        }
        else if (this.attack < creatureCard.GetAttack())
        {
            attackText.TmpObject.color = BoardCreature.LIGHT_RED;
        }
        else
        {
            attackText.TmpObject.color = Color.white;
        }

        if (this.health > creatureCard.GetHealth())
        {
            healthText.TmpObject.color = BoardCreature.LIGHT_GREEN;
        }
        else if (this.health < creatureCard.GetHealth())
        {
            healthText.TmpObject.color = BoardCreature.LIGHT_RED;
        }
        else
        {
            attackText.TmpObject.color = Color.white;
        }
    }

    private void RenderStatus()
    {
        float MAX_RANDOM_DELAY = 0.8F;

        //for frozen or silenced etc status
        if (this.isFrozen > 0)
        {
            if (!this.statusVFX.ContainsKey(BoardCreature.FROZEN_STATUS))
            {
                this.summonAnimation.enabled = false;
                this.statusVFX[BoardCreature.FROZEN_STATUS] = FXPoolManager.Instance.AssignEffect(BoardCreature.FROZEN_STATUS, this.transform).gameObject;
                this.statusVFX[BoardCreature.FROZEN_STATUS].transform.Rotate(Vector3.up * UnityEngine.Random.Range(-180, 180));
                LeanTween.scale(this.statusVFX[BoardCreature.FROZEN_STATUS], Vector3.one, ActionManager.TWEEN_DURATION)
                         .setDelay(UnityEngine.Random.Range(0, MAX_RANDOM_DELAY))
                         .setEaseOutCubic()
                         .setOnStart(() =>
                {
                    SoundManager.Instance.PlaySound("FreezeSFX", this.transform.position, pitchVariance: 0.3F, pitchBias: 0.5F);
                });
            }
        }
        else
        {
            if (this.statusVFX.ContainsKey(BoardCreature.FROZEN_STATUS))
            {
                LeanTween.scale(this.statusVFX[BoardCreature.FROZEN_STATUS], Vector3.zero, ActionManager.TWEEN_DURATION)
                         .setDelay(UnityEngine.Random.Range(0, MAX_RANDOM_DELAY))
                         .setEaseInCubic().setOnComplete(() =>
                {
                    SoundManager.Instance.PlaySound("UnfreezeSFX", this.transform.position, pitchVariance: 0.3F, pitchBias: -0.3F);
                    FXPoolManager.Instance.PlayEffect("UnfreezeVFX", this.transform.position);
                    FXPoolManager.Instance.UnassignEffect(BoardCreature.FROZEN_STATUS, this.statusVFX[BoardCreature.FROZEN_STATUS]);
                    this.summonAnimation.enabled = true;
                    this.statusVFX.Remove(BoardCreature.FROZEN_STATUS);
                });
            }
        }
    }

    private void RenderAbilitiesAndBuffs()
    {
        //doing additions
        foreach (string ability in this.abilities)
        {
            if (abilitiesVFX.ContainsKey(ability))
            {
                continue;
            }
            if (!FXPoolManager.Instance.HasEffect(ability))
            {
                continue;
            }

            abilitiesVFX[ability] = FXPoolManager.Instance.AssignEffect(ability, this.transform).gameObject;
        }

        //do removals
        foreach (string ability in new List<string>(this.abilitiesVFX.Keys))
        {
            if (HasAbility(ability))
            {
                continue;
            }

            //if not continue, needs removal
            GameObject effect = abilitiesVFX[ability];
            FXPoolManager.Instance.UnassignEffect(ability, effect);
            abilitiesVFX.Remove(ability);
        }
    }

    public bool HasAbility(string ability)
    {
        if (this.isSilenced)
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


    private void RaiseCardVisual()
    {
        this.raisedCard = true;
        this.visual.SetOpacity(1);

        LeanTween.moveLocal(this.visual.gameObject, this.visual.transform.localPosition + Vector3.back * 7 + Vector3.down, 0.01f)
                 .setDelay(ActionManager.TWEEN_DURATION)
                 .setOnStart(() =>
            {
                ActionManager.Instance.SetCursor(3);
                this.visual.gameObject.SetLayer(LayerMask.NameToLayer("UI"));
            });
        LeanTween.scale(this.visual.gameObject, INSPECT_CARD_SIZE, 0.01f)
                 .setDelay(ActionManager.TWEEN_DURATION);
        LeanTween.rotateX(this.visual.gameObject, 15, 0.01f)
                 .setDelay(ActionManager.TWEEN_DURATION);
    }

    private void LowerCardVisual()
    {
        ActionManager.Instance.SetCursor(0);
        this.raisedCard = false;
        this.visual.SetOpacity(0.8f);

        LeanTween.cancel(this.visual.gameObject);
        this.visual.transform.localPosition = Vector3.zero;
        this.visual.transform.rotation = Quaternion.Euler(0, 180, 0);
        this.visual.transform.localScale = BOARD_CARD_SIZE;
        this.visual.gameObject.SetLayer(LayerMask.NameToLayer("Battle"));
    }

    //begin mouse interactions for showing card when hovering
    public override void EnterHover()
    {
        this.visual.SetOutlineColors(Color.white, this.initialOutlineEndColor);
        if (!this.raisedCard)
        {
            RaiseCardVisual();
        }
    }

    public override void MouseUp()
    {
        if (!this.raisedCard)
        {
            RaiseCardVisual();
        }
    }

    public override void ExitHover()
    {
        this.visual.SetOutlineColors(this.initialOutlineStartColor, this.initialOutlineEndColor);
        if (this.raisedCard)
        {
            LowerCardVisual();
        }
    }

    public override void MouseDown()
    {
        if (this.owner.Id == BattleManager.Instance.ActivePlayer.Id && this.raisedCard)
        {
            LowerCardVisual();
        }
    }

    public void PlayParticle0()
    {
        //Called during attack animation
    }

    private void PlayAudioTakeDamage()
    {
        if (this.audioSources != null && this.audioSources.Length >= 3 && this.audioSources[2] != null)
        {
            this.audioSources[2].PlayDelayed(ATTACK_DELAY / 2);
        }
        else
        {
            Debug.LogWarning(string.Format("Missing audio source for card {0}", this.name));
        }
    }

    public ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();

        challengeCard.SetId(this.creatureCard.Id);
        challengeCard.SetPlayerId(this.owner.Id);
        challengeCard.SetCategory((int)Card.CardType.Creature);
        challengeCard.SetColor(this.creatureCard.GetClassColor());
        challengeCard.SetName(this.creatureCard.Name);
        //challengeCard.SetDescription(boardCreature.CreatureCard.Description);
        challengeCard.SetLevel(this.creatureCard.Level);
        challengeCard.SetCost(this.cost);
        challengeCard.SetCostStart(this.creatureCard.GetCost());
        challengeCard.SetHealth(this.health);
        challengeCard.SetHealthStart(this.creatureCard.GetHealth());
        challengeCard.SetHealthMax(this.maxHealth);
        challengeCard.SetAttack(this.attack);
        challengeCard.SetAttackStart(this.creatureCard.GetAttack());
        challengeCard.SetCanAttack(this.canAttack);
        challengeCard.SetIsFrozen(this.isFrozen);
        challengeCard.SetIsSilenced(this.isSilenced ? 1 : 0);
        challengeCard.SetSpawnRank(this.spawnRank);
        challengeCard.SetAbilities(this.abilities);
        challengeCard.SetAbilitiesStart(this.creatureCard.GetAbilities());

        return challengeCard;
    }
}
