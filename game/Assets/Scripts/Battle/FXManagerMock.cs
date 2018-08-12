using UnityEngine;
using UnityEngine.Events;

public class FXManagerMock : IFXManager
{
    public void PlayEffectLookAt(
        string effectName,
        Transform fromTransform,
        Transform toTransform
    )
    { }

    public void PlayEffectWithCallback(
        string effectName,
        string soundName,
        Transform transform,
        UnityAction onEffectFinish
    )
    {
        onEffectFinish.Invoke();
    }

    public void ThrowEffectWithCallback(
        string effectName,
        Transform fromTransform,
        Transform toTransform,
        UnityAction onEffectFinish
    )
    {
        onEffectFinish.Invoke();
    }
}
