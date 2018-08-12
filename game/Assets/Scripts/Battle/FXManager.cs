﻿using UnityEngine;
using UnityEngine.Events;

public class FXManager : IFXManager
{
    public void PlayEffectLookAt(
        string effectName,
        Transform fromTransform,
        Transform toTransform
    )
    {
        FXPoolManager.Instance.PlayEffectLookAt(
            effectName,
            fromTransform,
            toTransform
        );
    }

    public void PlayEffectWithCallback(
        string effectName,
        string soundName,
        Transform transform,
        UnityAction onEffectFinish
    )
    {
        FXPoolManager.Instance.PlayEffect(effectName, transform.position);
        SoundManager.Instance.PlaySound(soundName, transform.position);
        EffectManager.Instance.WaitAndInvokeCallback(0.5f, onEffectFinish);
    }

    public void ThrowEffectWithCallback(
        string effectName,
        Transform fromTransform,
        Transform toTransform,
        UnityAction onEffectFinish
    )
    {
        GameObject effectGameObject = FXPoolManager.Instance.PlayEffect(
            effectName,
            fromTransform.position
        );

        LeanTween
            .move(effectGameObject, toTransform.position, ActionManager.TWEEN_DURATION * 3)
            .setEaseInOutCirc()
            .setOnComplete(() =>
            {
                effectGameObject.SetActive(false); // TODO: does this need to be recycled or something?
                onEffectFinish.Invoke();
            });
    }
}
