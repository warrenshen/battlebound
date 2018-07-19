using UnityEngine;

[System.Serializable]
public class BattleCardObject : CardObject
{
    [SerializeField]
    protected Player owner;
    public Player Owner => owner;


    public void Initialize(Player player, Card card)
    {
        this.owner = player;
        base.Initialize(card);
    }

    public override void EnterHover()
    {
        if (this.owner.Mode == Player.PLAYER_STATE_MODE_MULLIGAN)
        {
            SetVisualResetValues();
            this.visual.transform.localScale = 1.15F * this.visual.reset.scale;
            return;
        }

        if (InspectorControlPanel.Instance.DevelopmentMode)
        {
            // Do not do anything when hovering over opponent card.
            if (this.owner.Id != BattleManager.Instance.You.Id)
            {
                return;
            }
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
}
