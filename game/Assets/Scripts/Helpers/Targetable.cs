using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Targetable : MonoBehaviour
{
    protected bool isAvatar = false;
    public bool IsAvatar => isAvatar;

    public abstract string GetCardId();

    public abstract string GetPlayerId();
}
