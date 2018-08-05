using UnityEngine.Events;

public interface IPlayerAvatarObject
{
    void Initialize(PlayerAvatar playerAvatar);

    void TakeDamage(int amount);

    void Heal(int amount);

    void Die();

    void Redraw();
}
