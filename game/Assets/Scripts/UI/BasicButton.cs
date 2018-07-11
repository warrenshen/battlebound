using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButton : ObjectUI
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private string functionName;


    // Use this for initialization
    void Start()
    {
        base.Initialize();
        this.scalingFactor = 1.10f;
    }

    public override void EnterHover()
    {
        base.EnterHover();
        ActionManager.Instance.SetCursor(1);
    }

    public override void ExitHover()
    {
        base.ExitHover();
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SetPassiveCursor();
        }
    }

    public override void MouseUp()
    {
        base.MouseUp();
        if (target != null)
            target.SendMessage(functionName);
    }
}
