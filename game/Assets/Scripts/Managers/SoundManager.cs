using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public Dictionary<string, int> soundIndices;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.soundIndices = new Dictionary<string, int>();
    }

    // Use this for initialization
    void Start()
    {
        foreach (Transform source in transform)
        {
            soundIndices.Add(source.name, 0);
        }
    }

    public void PlaySound(string name, Vector3 pos)
    {
        GameObject parent = GameObject.Find(name);
        Transform chosen = parent.transform.GetChild(soundIndices[name]);
        chosen.position = pos;
        chosen.GetComponent<AudioSource>().Play();
        //chosen.gameObject.SetActive(true);
        soundIndices[name] = (soundIndices[name] + 1) % parent.transform.childCount;
    }
}
