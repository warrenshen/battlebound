public interface Targetable
{
    string GetCardId();

    string GetPlayerId();

    bool CanAttackNow();

    TargetableObject GetTargetableObject();
}
