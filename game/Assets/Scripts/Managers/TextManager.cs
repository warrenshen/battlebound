using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class TextManager : MonoBehaviour
{
    [SerializeField]
    public Dictionary<string, int> typeIndices;
    public static TextManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.typeIndices = new Dictionary<string, int>();

        foreach (Transform source in this.transform)
        {
            typeIndices.Add(source.name, 0);
        }
    }

    public void ShowTextAtTarget(string name, Transform target, string value, float duration = 0.5F)
    {
        Transform root = transform.Find(name);
        if (root == null)
        {
            Debug.LogWarning("Could not play sound because no sound root found in pool.");
            return;
        }

        Transform chosen = root.transform.GetChild(typeIndices[name]);
        TextMeshPro textMesh = chosen.GetComponent<TextMeshPro>();
        chosen.position = target.transform.position;
        chosen.localScale = Vector3.zero;
        textMesh.text = value;

        typeIndices[name] = (typeIndices[name] + 1) % root.childCount;

        textMesh.gameObject.SetActive(true);
        LeanTween.scale(textMesh.gameObject, Vector3.one, ActionManager.TWEEN_DURATION)
             .setOnComplete(() =>
             {
                 LeanTween.moveY(textMesh.gameObject, chosen.transform.position.y + 0.5F, ActionManager.TWEEN_DURATION)
                      .setOnComplete(() =>
                      {
                          textMesh.gameObject.SetActive(false);
                      });
             });

    }
}
