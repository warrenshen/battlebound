using UnityEngine;
using UnityEngine.Events;

public interface IFXManager
{
    void PlayEffectLookAt(
        string effectName,
        Transform fromTransform,
        Transform toTransform
    );

    void ThrowEffectWithCallback(
        string effectName,
        Transform fromTransform,
        Transform toTransform,
        UnityAction onEffectFinish
    );
}
