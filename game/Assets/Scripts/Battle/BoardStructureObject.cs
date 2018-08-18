using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class BoardStructureObject : TargetableObject, IBoardStructureObject
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

    private BoardStructure boardStructure;

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

    public void Initialize(BoardStructure boardStructure)
    {
        this.boardStructure = boardStructure;
        this.gameObject.layer = 9;
    }

    public void SummonWithCallback(UnityAction onSummonFinish)
    {
        Transform boardPlace = Board.Instance().GetBoardPlaceByPlayerIdAndIndex(
            this.boardStructure.GetPlayerId(),
            this.boardStructure.FieldIndex
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
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), 0.5f).setEaseOutBack();

        onSummonFinish.Invoke();
    }

    public override bool IsAvatar()
    {
        return false;
    }

    public override string GetCardId()
    {
        return this.boardStructure.GetCardId();
    }

    public override string GetPlayerId()
    {
        return this.boardStructure.GetPlayerId();
    }

    public override Targetable GetTargetable()
    {
        return this.boardStructure;
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
        StartCoroutine("Dissolve", 2);
    }

    public void Redraw()
    {
        if (this.visual == null)
        {
            Debug.LogError("Board structure visual does not exist.");
            return;
        }
        UpdateStatText();
        this.visual.Redraw();
    }

    private HyperCard.Card VisualizeCard()
    {
        HyperCard.Card cardVisual = CardSingleton.Instance.TakeCardFromPool();

        cardVisual.transform.parent = this.transform;
        cardVisual.transform.localPosition = Vector3.zero;
        cardVisual.transform.localRotation = Quaternion.identity;
        cardVisual.transform.Rotate(0, 180, 0, Space.Self);

        Card.SetHyperCardFromData(ref cardVisual, this.boardStructure.GetCard());
        Card.SetHyperCardArtwork(ref cardVisual, this.boardStructure.GetCard());

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

        GameObject prefab = ResourceSingleton.Instance.GetPrefabByName(this.boardStructure.GetCardName());

        GameObject created = Instantiate(prefab) as GameObject;
        created.transform.parent = this.transform;
        created.transform.localPosition = new Vector3(0, 0, -0.3f);

        if (this.boardStructure.GetPlayerId() != BattleState.Instance().You.Id)
        {
            created.transform.Rotate(15, 180, 0, Space.Self);
        }
        else
        {
            created.transform.Rotate(-15, 0, 0, Space.Self);
        }

        this.audioSources = created.GetComponents<AudioSource>();
        this.summonAnimClips = new List<string>();

        this.summonAnimation = created.GetComponentInChildren<Animator>();
        this.summoned = this.summonAnimation.gameObject;

        if (this.summonAnimation == null)
        {
            Debug.LogError(String.Format("No animation or animator on gameobject: {0}", this.gameObject.name));
        }
        foreach (AnimationClip clip in this.summonAnimation.runtimeAnimatorController.animationClips)
        {
            this.summonAnimClips.Add(clip.name);
        }
        this.summonAnimation.speed = 1.33f;
        this.summonAnimation.Play(this.summonAnimClips[0]);
        if (this.summonAnimClips.Count > 1)
        {
            this.summonAnimation.CrossFade(this.summonAnimClips[1], 1F);
        }
        else
        {
            Debug.LogWarning("Missing second summon anim clip");
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

        int cost = this.boardStructure.Cost;
        int health = this.boardStructure.Health;
        int healthMax = this.boardStructure.GetHealthMax();
        StructureCard structureCard = this.boardStructure.GetCard();

        HyperCard.Card.TextMeshProParam costText = this.visual.GetTextFieldWithKey("Cost");
        HyperCard.Card.TextMeshProParam attackText = this.visual.GetTextFieldWithKey("Attack");
        HyperCard.Card.TextMeshProParam healthText = this.visual.GetTextFieldWithKey("Health");

        if (!costText.Value.Equals(cost.ToString()))
        {
            LeanTween.scale(costText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }
        if (!healthText.Value.Equals(health.ToString()))
        {
            LeanTween.scale(healthText.TmpObject.gameObject, Vector3.one * UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        }

        this.visual.SetTextFieldWithKey("Title", this.boardStructure.GetCard().GetName());
        this.visual.SetTextFieldWithKey("Cost", cost.ToString());
        this.visual.SetTextFieldWithKey("Health", health.ToString());

        //if (isSilenced)
        //{
        //    this.visual.SetTextFieldWithKey("Description", "");
        //}
        //else
        //{
        //    this.visual.SetTextFieldWithKey("Description", Card.GetDecriptionByAbilities(this.boardCreature.Abilities));
        //}

        if (cost < structureCard.GetCost())
        {
            costText.TmpObject.color = LIGHT_GREEN;
        }
        else
        {
            costText.TmpObject.color = Color.white;
        }

        if (health > structureCard.GetHealth() && health == healthMax)
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

    private void PlayAudioTakeDamage()
    {
        if (this.audioSources != null && this.audioSources.Length >= 3 && this.audioSources[2] != null)
        {
            this.audioSources[2].PlayDelayed(ATTACK_DELAY / 2);
        }
        else
        {
            Debug.LogWarning(string.Format("Missing audio source for card {0}", this.boardStructure.GetCardName()));
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
            this.boardStructure.GetPlayerId() == BattleState.Instance().ActivePlayer.Id &&
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
