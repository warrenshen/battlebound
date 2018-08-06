using UnityEngine;

public interface Targetable
{
    string GetCardId();

    string GetPlayerId();

    bool CanAttackNow();

    Transform GetTargetableTransform();

    TargetableObject GetTargetableObject();
}
