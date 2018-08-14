using System.Collections.Generic;
using UnityEngine;
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

    public void PlayEffectsWithCallback(
        List<string> effectNames,
        List<string> soundNames,
        Transform transform,
        UnityAction onEffectsFinish
    )
    {
        foreach (string effectName in effectNames)
        {
            FXPoolManager.Instance.PlayEffect(effectName, transform.position);
        }
        foreach (string soundName in soundNames)
        {
            SoundManager.Instance.PlaySound(soundName, transform.position);
        }
        EffectManager.Instance.WaitAndInvokeCallback(0.5f, onEffectsFinish);
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

        Vector3 delta = fromTransform.position - toTransform.position;
        Vector3 parabolaMidpoint = (fromTransform.position + toTransform.position) / 2.0f + Vector3.back * delta.magnitude / 2;
        LTSpline ltSpline = new LTSpline(new Vector3[] { fromTransform.position,
                                                         fromTransform.position,
                                                         parabolaMidpoint,
                                                         toTransform.position,
                                                         toTransform.position });

        LeanTween.moveSpline(effectGameObject, ltSpline, ActionManager.TWEEN_DURATION * 3)
                 .setOrientToPath(true)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() =>
                 {
                     effectGameObject.SetActive(false); // TODO: does this need to be recycled or something?
                     onEffectFinish.Invoke();
                 });
    }
}
