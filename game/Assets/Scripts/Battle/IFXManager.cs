using System.Collections.Generic;
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

    void PlayEffectsWithCallback(
        List<string> effectNames,
        List<string> soundNames,
        Transform transform,
        UnityAction onEffectsFinish
    );

    void ThrowEffectWithCallback(
        string effectName,
        Transform fromTransform,
        Transform toTransform,
        UnityAction onEffectFinish,
        float delay = 0.0f
    );
}
