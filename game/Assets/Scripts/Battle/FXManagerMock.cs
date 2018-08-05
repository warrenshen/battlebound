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

    public void ThrowEffectWithCallback(
        string effectName,
        Vector3 fromPosition,
        Vector3 toPosition,
        UnityAction onEffectFinish
    )
    {
        onEffectFinish.Invoke();
    }
}
