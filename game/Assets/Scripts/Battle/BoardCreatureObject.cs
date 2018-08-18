using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class BoardCreatureObject : TargetableObject, IBoardCreatureObject
{
    public const float UPDATE_STATS_GROWTH_FACTOR = 1.4F;
    public const float ATTACK_DELAY = 0.66F;
    private const float MAX_RANDOM_DELAY = 0.8F;

    private static Vector3 BOARD_CARD_SIZE = new Vector3(5.25F, 3.7F, 1);
    private static Vector3 COLLIDER_SIZE = new Vector3(3.8F, 4.5F, 1);
    private static Vector3 INSPECT_CARD_SIZE = new Vector3(5, 4.28F, 1);

    public static Color LIGHT_GREEN = new Color(0.33F, 1, 0.33F);
    public static Color LIGHT_RED = new Color(1, 0.33F, 0.33F);

    public const string FROZEN_STATUS = "FROZEN_STATUS";
    public const string CONDEMNED_STATUS = "CONDEMNED_STATUS";

    private BoardCreature boardCreature;

    private Dictionary<string, GameObject> abilitiesVFX;
    private Dictionary<string, GameObject> statusVFX;

    private bool raisedCard;

    Material dissolve;

    private HyperCard.Card visual;
    private const float BOARD_GROW_FACTOR = 1.25f;

    private GameObject summoned;
    private dynamic summonAnimation;
    private List<string> summonAnimClips;

    private AudioSource[] audioSources;

    public void Initialize(BoardCreature boardCreature)
    {
        this.boardCreature = boardCreature;

        this.gameObject.layer = 9;

        this.statusVFX = new Dictionary<string, GameObject>();
        this.abilitiesVFX = new Dictionary<string, GameObject>();
    }

    public void SummonWithCallback(UnityAction onSummonFinish)
    {
        Transform boardPlace = Board.Instance().GetBoardPlaceByPlayerIdAndIndex(
            this.boardCreature.GetPlayerId(),
            this.boardCreature.FieldIndex
        );
        this.gameObject.transform.position = boardPlace.position;

        StartCoroutine("SummonCoroutine", new object[2] { boardPlace, onSummonFinish });
    }

    IEnumerator SummonCoroutine(object[] args)
    {
        Transform boardPlace = args[0] as Transform;
        UnityAction onSummonFinish = args[1] as UnityAction;

        FXPoolManager.Instance.PlayEffect(
            "SpawnVFX",
            boardPlace.position + new Vector3(0f, 0f, -0.1f)
        );
        yield return new WaitForSeconds(0.2f);

        BoxCollider creatureCollider = this.gameObject.AddComponent<BoxCollider>() as BoxCollider;
        creatureCollider.size = COLLIDER_SIZE;

        this.visual = VisualizeCard();
        Summon();

        //method calls
        Redraw();

        //post-collider-construction visuals
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween
            .scale(gameObject, new Vector3(1, 1, 1), ActionManager.TWEEN_DURATION)
            .setEaseOutBack()
            .setOnComplete(() => onSummonFinish.Invoke());
    }

    public override bool IsAvatar()
    {
        return false;
    }

    public override string GetCardId()
    {
        return this.boardCreature.GetCardId();
    }

    public override string GetPlayerId()
    {
        return this.boardCreature.GetPlayerId();
    }

    public override Targetable GetTargetable()
    {
        return this.boardCreature;
    }

    public void FightAnimationWithCallback(TargetableObject other, UnityAction onFightFinish)
    {
        if (this.audioSources != null && this.audioSources.Length >= 2 && this.audioSources[1] != null)
        {
            this.audioSources[1].PlayDelayed(ATTACK_DELAY / 3); //sound
        }
        else
        {
            Debug.LogWarning(string.Format("Missing audio source for card {0}", this.boardCreature.GetCardName()));
        }

        if (
            this.summonAnimation != null &&
            this.summonAnimClips != null &&
            this.summonAnimClips.Count >= 2
        )
        {
            this.summonAnimation.Play(summonAnimClips[0]);
            this.summonAnimation.CrossFade(summonAnimClips[1], 3F);    //should group with sound as a method
        }
        else
        {
            Debug.LogWarning(string.Format("Missing summon animation for card {0}", this.boardCreature.GetCardName()));
        }

        //move/animate
        Vector3 delta = (this.transform.position - other.transform.position) / 1.5f;
        Vector3 originalPosition = this.summoned.transform.position;

        LeanTween.scale(this.summoned, this.summoned.transform.localScale * 1.2f, 2f).setEasePunch();
        LeanTween
            .move(this.summoned, this.transform.position - delta, 0.3F)
            .setEaseOutCubic()
            .setDelay(ATTACK_DELAY)
            .setOnComplete(
                () =>
                {
                    LeanTween
                        .move(this.summoned, originalPosition, 0.3F)
                        .setEaseInCubic();
                    onFightFinish();
                }
        );

        StartCoroutine("PlaySoundWithDelay", new object[3] { "PunchSFX", other.transform.position, ATTACK_DELAY + 0.25f });
        StartCoroutine("PlaySoundWithDelay", new object[3] { "SlashSFX", other.transform.position, ATTACK_DELAY + 0.4f });

        StartCoroutine(
            "PlayVFXWithDelay",
            new object[3] {
                this.boardCreature.GetCard().GetEffectPrefab(),
                other.transform.position,
                ATTACK_DELAY
            }
        );
        if (HasAbility(Card.CARD_ABILITY_LIFE_STEAL))
        {
            StartCoroutine("PlayVFXWithDelay", new object[3] { "LifestealVFX", other.transform.position, ATTACK_DELAY });
        }

        if (other.GetType() == typeof(BoardCreatureObject))
        {
            BoardCreatureObject otherObject = other as BoardCreatureObject;
            if (otherObject.HasAbility(Card.CARD_ABILITY_TAUNT))  //to-do this string should be chosen from some dict set by text file later
            {
                StartCoroutine("PlaySoundWithDelay", new object[3] { "HitTauntSFX", other.transform.position, ATTACK_DELAY + 0.5f });
            }
        }
    }

    /*
     * @return int - amount of damage taken
     */
    public void TakeDamage(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Take damage called with amount <= 0.");
            return;
        }

        LeanTween.scale(
            this.summoned,
            this.summoned.transform.localScale * 1.1f, 1F
        ).setEasePunch();
        PlayAudioTakeDamage();
        TextManager.Instance.ShowTextAtTarget(
            this.transform,
            amount.ToString(),
            Color.red
        );
    }

    public void RenderDeathwish()
    {
        FXPoolManager.Instance.PlayEffect("DeathwishVFX", this.transform.position);
    }

    public void RenderWarcry()
    {
        FXPoolManager.Instance.PlayEffect("WarcryVFX", this.transform.position);
    }

    /*
     * @return int - amount of health healed
     */
    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Heal called with amount <= 0.");
            return;
        }

        TextManager.Instance.ShowTextAtTarget(
            transform,
            amount.ToString(),
            Color.green
        );
        FXPoolManager.Instance.PlayEffect("HealPillarVFX", transform.position);
    }

    public void Die()
    {
        UnassignVFX();
        StartCoroutine("Dissolve", 2);
    }

    private void UnassignVFX()
    {
        //Unassign VFXs for creatures that die etc..
        foreach (string ability in this.abilitiesVFX.Keys)
        {
            //to-do: scale effects down before set active false
            GameObject effect = this.abilitiesVFX[ability];
            FXPoolManager.Instance.UnassignEffect(ability, effect);
        }

        if (this.statusVFX.ContainsKey(FROZEN_STATUS))
        {
            Unfreeze();
        }

        if (this.statusVFX.ContainsKey(CONDEMNED_STATUS))
        {
            //to-do: scale effects down before set active false
            GameObject effect = this.statusVFX[CONDEMNED_STATUS];
            FXPoolManager.Instance.UnassignEffect(CONDEMNED_STATUS, effect);
        }
    }

    public bool HasAbility(string ability)
    {
        return this.boardCreature.HasAbility(ability);
    }

    public void Redraw()
    {
        if (this.visual == null)
        {
            return;
        }

        UpdateStatText();

        this.visual.SetOutline(this.boardCreature.CanAttackNow());
        this.visual.Redraw();

        RenderStatus();
        RenderAbilitiesAndBuffs();
    }

    private HyperCard.Card VisualizeCard()
    {
        HyperCard.Card cardVisual = CardSingleton.Instance.TakeCardFromPool();

        cardVisual.transform.parent = this.transform;
        cardVisual.transform.localPosition = Vector3.zero;
        cardVisual.transform.localRotation = Quaternion.identity;
        cardVisual.transform.Rotate(0, 180, 0, Space.Self);

        Card.SetHyperCardFromData(ref cardVisual, this.boardCreature.GetCard());
        Card.SetHyperCardArtwork(ref cardVisual, this.boardCreature.GetCard());

        cardVisual.gameObject.SetLayer(9);

        cardVisual.transform.parent = this.transform;
        cardVisual.transform.localPosition = Vector3.zero;
        cardVisual.transform.localRotation = Quaternion.identity;
        cardVisual.transform.Rotate(0, 180, 0, Space.Self);
        cardVisual.transform.localScale = BOARD_CARD_SIZE;

        foreach (HyperCard.Card.CustomSpriteParam spriteParam in cardVisual.SpriteObjects)
        {
            spriteParam.IsAffectedByFilters = false;
        }

        cardVisual.SetOpacity(0.8f);
        cardVisual.SetBlackAndWhite(true);
        cardVisual.SetOutlineColors(HyperCard.Card.DEFAULT_OUTLINE_START_COLOR, HyperCard.Card.DEFAULT_OUTLINE_END_COLOR);

        return cardVisual;
    }

    private void Summon()
    {
        SoundManager.Instance.PlaySound("SummonSFX", transform.position);

        GameObject prefab = ResourceSingleton.Instance.GetPrefabByName(this.boardCreature.GetCardName());

        GameObject created = Instantiate(prefab) as GameObject;
        created.transform.parent = this.transform;
        created.transform.localPosition = new Vector3(0, 0, -0.3f);

        if (this.boardCreature.GetPlayerId() != BattleState.Instance().You.Id)
        {
            created.transform.Rotate(15, 180, 0, Space.Self);
        }
        else
        {
            created.transform.Rotate(-15, 0, 0, Space.Self);
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

    private IEnumerator Dissolve(float duration)
    {
        this.noInteraction = true;
        FXPoolManager.Instance.PlayEffect("CreatureDeathVFX", this.transform.position);
        SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);

        Vector3 originalScale = this.visual.transform.localScale;
        float elapsedTime = 0;
        LeanTween.scale(this.summoned, Vector3.zero, duration / 3).setOnComplete(() =>
        {
            this.summoned.SetActive(false);
        });
        while (elapsedTime < duration)
        {
            this.visual.transform.localScale = Mathf.Lerp(1, 0, (elapsedTime / duration)) * originalScale;
            this.visual.BurningAmount = Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    private void UpdateStatText()
    {
        if (this.visual == null)
        {
            return;
        }

        int cost = this.boardCreature.Cost;
        int attack = this.boardCreature.GetAttack();
        int health = this.boardCreature.Health;
        int healthMax = this.boardCreature.GetHealthMax();
        bool isSilenced = this.boardCreature.IsSilenced;
        CreatureCard creatureCard = this.boardCreature.GetCard();

        HyperCard.Card.TextMeshProParam costText = this.visual.GetTextFieldWithKey("Cost");
        HyperCard.Card.TextMeshProParam attackText = this.visual.GetTextFieldWithKey("Attack");
        HyperCard.Card.TextMeshProParam healthText = this.visual.GetTextFieldWithKey("Health");

        if (!costText.Value.Equals(cost.ToString()))
        {
            LeanTween.scale(costText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }
        if (!attackText.Value.Equals(attack.ToString()))
        {
            LeanTween.scale(attackText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }
        if (!healthText.Value.Equals(health.ToString()))
        {
            LeanTween.scale(healthText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }

        this.visual.SetTextFieldWithKey("Title", this.boardCreature.GetCard().GetName());
        this.visual.SetTextFieldWithKey("Cost", cost.ToString());
        this.visual.SetTextFieldWithKey("Attack", attack.ToString());
        this.visual.SetTextFieldWithKey("Health", health.ToString());

        if (isSilenced)
        {
            this.visual.SetTextFieldWithKey("Description", "");
        }
        else
        {
            this.visual.SetTextFieldWithKey("Description", Card.GetDecriptionByAbilities(this.boardCreature.Abilities));
        }

        if (cost < creatureCard.GetCost())
        {
            costText.TmpObject.color = LIGHT_GREEN;
        }
        else
        {
            costText.TmpObject.color = Color.white;
        }

        if (attack > creatureCard.GetAttack())
        {
            attackText.TmpObject.color = LIGHT_GREEN;
        }
        else if (attack < creatureCard.GetAttack())
        {
            attackText.TmpObject.color = LIGHT_RED;
        }
        else
        {
            attackText.TmpObject.color = Color.white;
        }

        if (health > creatureCard.GetHealth() && health == healthMax)
        {
            healthText.TmpObject.color = LIGHT_GREEN;
        }
        else if (health < healthMax)
        {
            healthText.TmpObject.color = LIGHT_RED;
        }
        else
        {
            healthText.TmpObject.color = Color.white;
        }
    }

    private void RenderStatus()
    {
        int isFrozen = this.boardCreature.IsFrozen;

        //for frozen or silenced etc status
        if (isFrozen > 0)
        {
            if (!this.statusVFX.ContainsKey(FROZEN_STATUS))
            {
                Freeze();
            }
        }
        else
        {
            if (this.statusVFX.ContainsKey(FROZEN_STATUS))
            {
                Unfreeze();
            }
        }

        bool isCondemned = this.boardCreature.IsSilenced;

        if (isCondemned)
        {
            if (!this.statusVFX.ContainsKey(CONDEMNED_STATUS))
            {
                this.statusVFX[CONDEMNED_STATUS] = FXPoolManager.Instance.AssignEffect(
                    CONDEMNED_STATUS,
                    this.transform
                ).gameObject;

                //to-do refactor this spawn/tween with variance logic for freeze, silence, and other status
                LeanTween
                    .scale(this.statusVFX[CONDEMNED_STATUS], Vector3.one, ActionManager.TWEEN_DURATION)
                    .setDelay(UnityEngine.Random.Range(0, MAX_RANDOM_DELAY))
                    .setEaseOutCubic()
                    .setOnStart(() =>
                    {
                        SoundManager.Instance.PlaySound("CondemnedSFX", this.transform.position, pitchVariance: 0.3F, pitchBias: 0.5F);
                    });
            }
        }
    }

    private void Freeze()
    {
        this.summonAnimation.enabled = false;
        this.statusVFX[FROZEN_STATUS] = FXPoolManager.Instance.AssignEffect(
            FROZEN_STATUS,
            this.transform
        ).gameObject;

        this.statusVFX[FROZEN_STATUS].transform.Rotate(Vector3.up * UnityEngine.Random.Range(-180, 180));
        LeanTween
            .scale(this.statusVFX[FROZEN_STATUS], Vector3.one * 2 * UnityEngine.Random.Range(0.9f, 1.1f), ActionManager.TWEEN_DURATION)
            .setDelay(UnityEngine.Random.Range(0, MAX_RANDOM_DELAY))
            .setEaseOutCubic()
            .setOnStart(() =>
            {
                SoundManager.Instance.PlaySound("FreezeSFX", this.transform.position, pitchVariance: 0.3F, pitchBias: 0.5F);
            });
    }

    private void Unfreeze()
    {
        FXPoolManager.Instance.UnassignEffect(FROZEN_STATUS, this.statusVFX[FROZEN_STATUS]);
        GameObject freezeObject = this.statusVFX[FROZEN_STATUS];
        this.statusVFX.Remove(FROZEN_STATUS);

        LeanTween
            .scale(freezeObject, Vector3.zero, ActionManager.TWEEN_DURATION)
            .setDelay(UnityEngine.Random.Range(0, MAX_RANDOM_DELAY))
            .setEaseInCubic().setOnComplete(() =>
            {
                SoundManager.Instance.PlaySound("UnfreezeSFX", this.transform.position, pitchVariance: 0.3F, pitchBias: -0.3F);
                FXPoolManager.Instance.PlayEffect("UnfreezeVFX", this.transform.position);

                this.summonAnimation.enabled = true;
            });
    }

    private void RenderAbilitiesAndBuffs()
    {
        List<string> abilities = this.boardCreature.Abilities;
        bool isSilenced = this.boardCreature.IsSilenced;

        //doing additions
        foreach (string ability in abilities)
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

    private void PlayAudioTakeDamage()
    {
        if (this.audioSources != null && this.audioSources.Length >= 3 && this.audioSources[2] != null)
        {
            this.audioSources[2].PlayDelayed(ATTACK_DELAY / 2);
        }
        else
        {
            Debug.LogWarning(string.Format("Missing audio source for card {0}", this.boardCreature.GetCardName()));
        }
    }


    // MouseWatchable functions.
    public override void EnterHover()
    {
        this.visual.SetOutlineColors(Color.white, HyperCard.Card.DEFAULT_OUTLINE_END_COLOR);
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
        this.visual.SetOutlineColors(HyperCard.Card.DEFAULT_OUTLINE_START_COLOR, HyperCard.Card.DEFAULT_OUTLINE_END_COLOR);
        if (this.raisedCard)
        {
            LowerCardVisual();
        }
    }

    public override void MouseDown()
    {
        if (
            this.boardCreature.GetPlayerId() == BattleState.Instance().ActivePlayer.Id &&
            this.raisedCard
        )
        {
            LowerCardVisual();
        }
    }

    private void RaiseCardVisual()
    {
        this.raisedCard = true;
        this.visual.SetOpacity(1);
        this.visual.SetGrayscale(false);

        LeanTween
            .rotateX(this.visual.gameObject, 0, ActionManager.TWEEN_DURATION)
            .setDelay(ActionManager.TWEEN_DURATION)
            .setOnStart(() =>
            {
                BattleManager.Instance.HoverCard.gameObject.SetActive(true);
                BattleManager.Instance.HoverCard.Copy(this.visual);
            })
            .setOnComplete(() =>
            {
                ActionManager.Instance.SetCursor(3);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(this.visual.transform.position);
                Vector2 hoverOffset = new Vector2(150, 0);
                if (screenPos.x > Screen.width / 2)
                {
                    hoverOffset.x *= -1;
                }

                CanvasScaler canvasScaler = BattleManager.Instance.canvas.GetComponent<CanvasScaler>();
                float scaleFactorX = canvasScaler.referenceResolution.x / Screen.width;
                float scaleFactorY = canvasScaler.referenceResolution.y / Screen.height;
                BattleManager.Instance.HoverCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(screenPos.x * scaleFactorX, screenPos.y * scaleFactorY) + hoverOffset;
            });
    }

    private void LowerCardVisual()
    {
        ActionManager.Instance.SetCursor(0);
        this.raisedCard = false;
        this.visual.SetGrayscale(true);
        this.visual.SetOpacity(0.8f);
        LeanTween.cancel(this.visual.gameObject);
        BattleManager.Instance.HoverCard.gameObject.SetActive(false);
        BattleManager.Instance.HoverCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000, -1000);
    }
}
