using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BattleCardObject : CardObject
{
    [SerializeField]
    protected Player owner;
    public Player Owner => owner;


    public void Initialize(Player player, Card card)
    {
        this.owner = player;

        if (card.Id == "HIDDEN")
        {
            this.templateData = new CardTemplate();  //to-do: pretty sure this will break things?
        }
        else if (!BattleManager.Instance.cardTemplates.ContainsKey(card.Name))
        {
            Debug.LogError(string.Format("Card {0} does not exist in codex.", card.Name));
            this.templateData = new CardTemplate();
        }
        else
        {
            this.templateData = BattleManager.Instance.cardTemplates[card.Name];
        }

        //does the visual stuff using templateData
        base.Initialize(card);
    }

    protected override void LoadCardArtwork()
    {
        base.LoadCardArtwork();
        this.visual.Stencil = BattleManager.Instance.stencilCount;
        BattleManager.Instance.stencilCount += 1 % 255;
    }

    public override void EnterHover()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            SetVisualResetValues();
            this.visual.transform.localScale = 1.15F * this.visual.reset.scale;
            return;
        }

        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;
        //set defaults of hypercard
        this.SetVisualResetValues();

        float scaling = 1.8f;
        this.visual.transform.localScale = scaling * this.visual.reset.scale;

        this.visual.transform.Translate(Vector3.up * 3.5f, Space.Self);
        this.visual.transform.Translate(Vector3.forward * 1, Space.Self);
    }

    public override void ExitHover()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            BattleManager.Instance.SetPassiveCursor();
            this.visual.transform.localScale = this.visual.reset.scale;
            return;
        }

        if (!this.owner.HasTurn)
            return;
        if (ActionManager.Instance.HasDragTarget())
            return;

        this.visual.transform.localScale = this.visual.reset.scale;
        this.visual.transform.localPosition = this.visual.reset.position;
    }

    public override void MouseDown()
    {
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
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            BattleManager.Instance.ToggleMulliganCard(this);
            return;
        }

        if (!this.owner.HasTurn)
        {
            return;
        }
        if (!ActionManager.Instance.HasDragTarget())
        {
            return;
        }

        //// card object position reset to original, handled in actionmanager already
        if (Time.time - lastClicked < 0.5f)
            DoubleClickUp();
        lastClicked = Time.time;
    }

    public void DoubleClickUp()
    {
        Debug.Log(gameObject.name + " double clicked.");
    }
}
