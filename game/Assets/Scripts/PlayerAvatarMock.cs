using UnityEngine.Events;

public class PlayerAvatarMock : IPlayerAvatarObject
{
    public void Initialize(PlayerAvatar playerAvatar)
    { }

    public void TakeDamage(int amount)
    { }

    public void Heal(int amount)
    { }

    public void Die()
    { }

    public void Redraw()
    { }
}
