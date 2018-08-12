using UnityEngine;
using UnityEngine.Events;

public interface IFXManager
{
    void PlayEffectLookAt(
        string effectName,
        Transform fromTransform,
        Transform toTransform
    );

    void PlayEffectWithCallback(
        string effectName,
        string soundName,
        Transform transform,
        UnityAction onEffectFinish
    );

    void ThrowEffectWithCallback(
        string effectName,
        Transform fromTransform,
        Transform toTransform,
        UnityAction onEffectFinish
    );
}
