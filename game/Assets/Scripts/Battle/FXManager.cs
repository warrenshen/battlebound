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

    public void ThrowEffectWithCallback(
        string effectName,
        Vector3 fromPosition,
        Vector3 toPosition,
        UnityAction onEffectFinish
    )
    {
        GameObject effectGameObject = FXPoolManager.Instance.PlayEffect(
            effectName,
            fromPosition
        );

        LeanTween
            .move(effectGameObject, toPosition, ActionManager.TWEEN_DURATION * 3)
            .setEaseInOutCirc()
            .setOnComplete(() =>
            {
                effectGameObject.SetActive(false); // TODO: does this need to be recycled or something?
                onEffectFinish.Invoke();
            });
    }
}
