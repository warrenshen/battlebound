using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicButton : ObjectUI
{
    public GameObject target;
    public string functionName;

    // Use this for initialization
    void Start()
    {
        base.Initialize();
        this.scalingFactor = 1.10f;
    }

    public override void EnterHover()
    {
        base.EnterHover();
        Cursor.SetCursor(ActionManager.Instance.cursors[1], Vector2.zero, CursorMode.Auto);
    }

    public override void ExitHover()
    {
        base.ExitHover();
        BattleManager.Instance.SetPassiveCursor();
    }

    public override void MouseUp()
    {
        base.MouseUp();
        if (target != null)
            target.SendMessage(functionName);
    }
}
