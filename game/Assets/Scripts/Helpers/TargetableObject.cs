using UnityEngine;
using UnityEngine.Events;

[System.Serializable]  //serialization won't work because abstract
public abstract class TargetableObject : MouseWatchable
{
    public abstract bool IsAvatar();
    public abstract string GetCardId();
    public abstract string GetPlayerId();
    public abstract Targetable GetTargetable();
}
