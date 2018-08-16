using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleCardObject : CardObject
{
    public static Vector3 CARD_VISUAL_SIZE = new Vector3(3.85f, 3.2f, 3);

    [SerializeField]
    protected Player owner;
    public Player Owner => owner;

    private List<string> buffs;
    public List<string> Buffs => buffs;

    private int costFromServer;

    public void Initialize(Player player, Card card)
    {
        base.Initialize(card);

        this.owner = player;
        this.buffs = new List<string>();
        this.costFromServer = -1;

        StartFlippedIfNeeded();
    }

    public void Reinitialize(ChallengeCard challengeCard)
    {
        this.card = challengeCard.GetCard();
        card.wrapper = this;
        LoadCardArtwork();

        if (this.visual == null)
        {
            Debug.LogError("Reinitialize called on card object without card visual.");
            return;
        }

        Card.SetHyperCardFromData(ref this.visual, this.card);
        Card.SetHyperCardArtwork(ref this.visual, this.card);

        this.costFromServer = challengeCard.Cost;
        this.buffs = challengeCard.GetBuffsHand();

        if (this.costFromServer != this.card.GetCost())
        {
            SetHyperCardCost(this.visual, this);
        }
    }

    public string GetCardId()
    {
        return this.card.Id;
    }

    public string GetPlayerId()
    {
        return this.Owner.Id;
    }

    public int GetCost()
    {
        // If cost is from server - since device cannot
        // predict cost of cards in opponent hand, use that.
        if (this.costFromServer >= 0)
        {
            return this.costFromServer;
        }

        int cost = this.card.GetCost();

        if (this.buffs.Contains(Card.BUFF_HAND_DECREASE_COST_BY_COLOR))
        {
            cost -= 10;
        }

        // Cost cannot be negative.
        cost = Math.Max(cost, 0);
        return cost;
    }

    public CardTemplate.ClassColor GetClassColor()
    {
        return this.card.GetClassColor();
    }

    public void GrantDecreaseCostByColor()
    {
        if (!this.buffs.Contains(Card.BUFF_HAND_DECREASE_COST_BY_COLOR))
        {
            this.buffs.Add(Card.BUFF_HAND_DECREASE_COST_BY_COLOR);
            SetHyperCardCost(this.visual, this);
            this.visual.GetTextFieldWithKey("Cost").TmpObject.color = BoardCreatureObject.LIGHT_GREEN;
        }
    }

    public void RemoveDecreaseCostByColor()
    {
        if (this.buffs.Contains(Card.BUFF_HAND_DECREASE_COST_BY_COLOR))
        {
            this.buffs.Remove(Card.BUFF_HAND_DECREASE_COST_BY_COLOR);
            SetHyperCardCost(this.visual, this);
            this.visual.GetTextFieldWithKey("Cost").TmpObject.color = Color.white;
        }
    }

    private void StartFlippedIfNeeded()
    {
        if (FlagHelper.IsServerEnabled() && this.owner.Id == BattleState.Instance().Opponent.Id)
        {
            this.visual.transform.Rotate(0, 180, 0, Space.Self);
        }
    }

    public ChallengeCard GetChallengeCard()
    {
        ChallengeCard challengeCard = new ChallengeCard();
        challengeCard.SetId(this.card.Id);
        challengeCard.SetPlayerId(this.owner.Id);
        challengeCard.SetColor(this.card.GetClassColor());
        challengeCard.SetName(this.card.Name);
        challengeCard.SetLevel(this.card.Level);
        challengeCard.SetCost(GetCost()); // Note this is calling this' GetCost.
        challengeCard.SetCostStart(this.card.GetCost());
        challengeCard.SetBuffsHand(this.buffs);
        challengeCard.SetBuffsField(new List<string>());

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
        LeanTween.scale(
            cardVisual.GetTextFieldWithKey("Cost").TmpObject.gameObject,
            Vector3.one * BoardCreatureObject.UPDATE_STATS_GROWTH_FACTOR,
            0.5F
        ).setEasePunch();
        cardVisual.SetTextFieldWithKey("Cost", battleCardObject.GetCost().ToString());

        cardVisual.Redraw();
    }

    public override void EnterHover()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            SetVisualResetValues();
            if (this.visual.transform.localScale.x <= CARD_VISUAL_SIZE.x && !LeanTween.isTweening(this.visual.gameObject))
            {
                this.visual.transform.localScale *= 1.1f;
                ActionManager.Instance.SetCursor(1);
                SoundManager.Instance.PlaySound("HoverShimmerSFX", this.transform.position);
            }
            return;
        }

        // dont do anything if not your card
        if (FlagHelper.IsServerEnabled() && this.owner.Id != BattleState.Instance().You.Id)
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
            this.visual.transform.localScale = this.visual.reset.scale;
            return;
        }
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
            SoundManager.Instance.PlaySound("MulliganAction", this.transform.position);
            return;
        }
        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of cardobject
        this.SetThisResetValues();

        LeanTween.scale(this.visual.gameObject, this.visual.reset.scale, 0.05f);
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
}
