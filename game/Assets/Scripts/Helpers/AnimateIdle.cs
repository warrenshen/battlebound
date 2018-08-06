using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class AnimateIdle : MonoBehaviour
{
    protected dynamic summonAnimation;
    protected List<string> summonAnimClips;

    protected virtual void Awake()
    {
        this.summonAnimClips = new List<string>();
        this.summonAnimation = this.transform.GetChild(0).GetComponent<Animation>();

        if (this.summonAnimation != null)
        {
            foreach (AnimationState state in this.summonAnimation)
            {
                state.speed = 5f;
                this.summonAnimClips.Add(state.clip.name);
            }
        }
        else
        {
            this.summonAnimation = this.transform.GetChild(0).GetComponent<Animator>();
            if (this.summonAnimation == null)
            {
                Debug.LogError(String.Format("No animation or animator on gameobject: {0}", this.gameObject.name));
            }
            foreach (AnimationClip clip in this.summonAnimation.runtimeAnimatorController.animationClips)
            {
                this.summonAnimClips.Add(clip.name);
            }
            this.summonAnimation.speed = 1.33f;
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        Animate();
    }

    protected virtual void OnEnable()
    {
        Animate();
    }

    protected virtual void Animate()
    {
        this.summonAnimation.Play(this.summonAnimClips[0]);
        this.summonAnimation.CrossFade(this.summonAnimClips[1], 3);
    }
}
