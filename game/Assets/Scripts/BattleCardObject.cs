﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BattleCardObject : CardObject
{
    public static Vector3 CARD_VISUAL_SIZE = new Vector3(3.55f, 3, 3);
    public const string HAND_CARD_DECREASE_COST_BY_COLOR = "HAND_CARD_DECREASE_COST_BY_COLOR";

    [SerializeField]
    protected Player owner;
    public Player Owner => owner;

    private List<string> buffs;
    public List<string> Buffs => buffs;

    public void Initialize(Player player, Card card)
    {
        this.owner = player;
        base.Initialize(card);
        this.StartFlippedIfNeeded();

        this.buffs = new List<string>();
    }

    public int GetCost()
    {
        int cost = this.card.GetCost();

        if (this.buffs.Contains(HAND_CARD_DECREASE_COST_BY_COLOR))
        {
            cost -= 10;
        }

        return cost;
    }

    public CardTemplate.ClassColor GetClassColor()
    {
        return this.card.GetClassColor();
    }

    public void GrantDecreaseCostByColor()
    {
        if (!this.buffs.Contains(HAND_CARD_DECREASE_COST_BY_COLOR))
        {
            this.buffs.Add(HAND_CARD_DECREASE_COST_BY_COLOR);
            SetHyperCardCost(this.visual, this);
            this.visual.GetTextFieldWithKey("Cost").TmpObject.color = BoardCreature.LIGHT_GREEN;
        }
    }

    public void RemoveDecreaseCostByColor()
    {
        if (this.buffs.Contains(HAND_CARD_DECREASE_COST_BY_COLOR))
        {
            this.buffs.Remove(HAND_CARD_DECREASE_COST_BY_COLOR);
            SetHyperCardCost(this.visual, this);
            this.visual.GetTextFieldWithKey("Cost").TmpObject.color = Color.white;
        }
    }

    private void StartFlippedIfNeeded()
    {
        if (FlagHelper.IsServerEnabled() && this.owner.Id == BattleManager.Instance.Opponent.Id)
        {
            this.visual.transform.Rotate(0, 180, 0, Space.Self);
        }
    }

    public override void EnterHover()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            SetVisualResetValues();
            if (this.visual.transform.localScale.x < 4 && !LeanTween.isTweening(this.visual.gameObject))
            {
                LeanTween.scale(this.visual.gameObject, this.visual.transform.localScale * 1.1f, 0.05f);
            }
            return;
        }

        if (FlagHelper.IsServerEnabled() && this.owner.Id != BattleManager.Instance.You.Id)
        {
            return;
        }
        else if (!this.owner.HasTurn)
        {
            return;
        }

        if (ActionManager.Instance.HasDragTarget())
        {
            return;
        }

        //set defaults of hypercard
        this.SetVisualResetValues();

        float scaling = 1.8f;
        this.visual.transform.localScale = scaling * this.visual.reset.scale;

        this.visual.transform.Translate(Vector3.up * 3.5f, Space.Self);
        this.visual.transform.Translate(Vector3.forward * 1, Space.Self);
        this.gameObject.SetLayer(LayerMask.NameToLayer("UI"));
    }

    public override void ExitHover()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            BattleManager.Instance.SetPassiveCursor();
            //this.visual.transform.localScale = this.visual.reset.scale;
            LeanTween.cancel(this.visual.gameObject);
            LeanTween.scale(this.visual.gameObject, CARD_VISUAL_SIZE, 0.05f);
            return;
        }

        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;

        this.visual.transform.localScale = this.visual.reset.scale;
        this.visual.transform.localPosition = this.visual.reset.position;
        this.gameObject.SetLayer(LayerMask.NameToLayer("Card"));
    }

    public override void MouseDown()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            BattleManager.Instance.ToggleMulliganCard(this);
            return;
        }

        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of cardobject
        this.SetThisResetValues();

        LeanTween.scale(this.visual.gameObject, this.visual.reset.scale, 0.1f);
        //this.visual.transform.localScale = this.visual.reset.scale;
        this.visual.transform.localPosition = this.visual.reset.position;

        ActionManager.Instance.SetDragTarget(this);
    }

    public override void MouseUp()
    {
        //nothing being done.. conditional checks since removed
    }

    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
    }

    public override void Release()
    {

    }

    public void Burn()
    {
        StartCoroutine("Dissolve", 1);  //called when overdraw
    }

    private IEnumerator Dissolve(float duration)
    {
        SoundManager.Instance.PlaySound("BurnDestroySFX", this.transform.position);

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            this.visual.BurningAmount = Mathf.Lerp(0, 1, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        this.Recycle();
    }

    public ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();
        challengeCard.SetId(this.card.Id);
        challengeCard.SetColor(this.card.GetClassColor());
        challengeCard.SetName(this.card.Name);
        challengeCard.SetLevel(this.card.Level);
        challengeCard.SetCost(GetCost()); // Note this is calling this' GetCost.
        challengeCard.SetCostStart(this.card.GetCost());

        if (this.card.GetType() == typeof(CreatureCard))
        {
            challengeCard.SetCategory((int)Card.CardType.Creature);

            CreatureCard creatureCard = this.card as CreatureCard;
            challengeCard.SetHealth(creatureCard.GetHealth());
            challengeCard.SetHealthStart(creatureCard.GetHealth());
            challengeCard.SetHealthMax(creatureCard.GetHealth());
            challengeCard.SetAttack(creatureCard.GetAttack());
            challengeCard.SetAttackStart(creatureCard.GetAttack());
            challengeCard.SetAbilities(creatureCard.GetAbilities());
            challengeCard.SetAbilitiesStart(creatureCard.GetAbilities());
        }
        else if (this.card.GetType() == typeof(SpellCard))
        {
            challengeCard.SetCategory((int)Card.CardType.Spell);
        }
        else
        {
            Debug.LogError("Unsupported!");
        }

        return challengeCard;
    }

    private static void SetHyperCardCost(
        HyperCard.Card cardVisual,
        BattleCardObject battleCardObject
    )
    {
        LeanTween.scale(cardVisual.GetTextFieldWithKey("Cost").TmpObject.gameObject, Vector3.one * BoardCreature.UPDATE_STATS_GROWTH_FACTOR, 0.5F).setEasePunch();
        cardVisual.SetTextFieldWithKey("Cost", battleCardObject.GetCost().ToString());

        cardVisual.Redraw();
    }
}
