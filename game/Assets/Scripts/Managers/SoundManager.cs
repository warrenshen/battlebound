using System.Collections;
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

    public void PlaySound(string name, Vector3 position)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
        {
            Debug.LogWarning("Could not play sound because no sound root found in pool.");
            return;
        }

        Transform chosen = root.transform.GetChild(soundIndices[name]);
        chosen.position = position;
        chosen.GetComponent<AudioSource>().Play();
        soundIndices[name] = (soundIndices[name] + 1) % root.transform.childCount;
    }
}
