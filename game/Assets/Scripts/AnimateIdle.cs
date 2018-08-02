using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateIdle : MonoBehaviour
{
    private dynamic summonAnimation;
    private List<string> summonAnimClips;

    void Awake()
    {
        this.summonAnimClips = new List<string>();
        this.summonAnimation = this.GetComponent<Animation>();
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
            this.summonAnimation = this.GetComponent<Animator>();
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
    void Start()
    {
        this.summonAnimation.Play(this.summonAnimClips[0]);
        this.summonAnimation.CrossFade(this.summonAnimClips[1], 1F);
    }
}
