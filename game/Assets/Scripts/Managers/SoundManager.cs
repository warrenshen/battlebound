﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    public Dictionary<string, int> soundIndices;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.soundIndices = new Dictionary<string, int>();

        foreach (Transform source in this.transform)
        {
            soundIndices.Add(source.name, 0);
        }
    }

    public void PlaySound(string name, Vector3 position, float pitchVariance = 0F, float pitchBias = 0F, float delay = 0F)
    {
        Transform root = transform.Find(name);
        if (root == null)
        {
            Debug.LogWarning(String.Format("Could not play sound `{0}` because no sound root found in pool.", name));
            return;
        }

        Transform chosen = root.GetChild(soundIndices[name]);
        chosen.position = position;

        AudioSource audioSource = chosen.GetComponent<AudioSource>();
        audioSource.pitch = 1 + pitchBias;
        soundIndices[name] = (soundIndices[name] + 1) % root.transform.childCount;

        if (pitchVariance > 0)
        {
            float previousPitch = audioSource.pitch;
            audioSource.pitch += UnityEngine.Random.Range(-pitchVariance, pitchVariance);
            audioSource.PlayDelayed(delay);
        }
        else
        {
            audioSource.PlayDelayed(delay);
        }
    }
}
