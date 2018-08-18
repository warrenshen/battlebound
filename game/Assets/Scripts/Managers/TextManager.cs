using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class TextManager : MonoBehaviour
{
    int index;

    [SerializeField]
    public static TextManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        this.index = 0;
    }

    public void ShowTextAtTarget(Transform target, string value, Color color, float duration = 1)
    {
        Transform chosen = transform.GetChild(this.index);
        TextMeshPro text_m = chosen.GetComponent<TextMeshPro>();

        chosen.position = target.transform.position;
        text_m.text = value;
        text_m.color = color;

        this.index = (this.index + 1) % transform.childCount;
        text_m.gameObject.SetActive(true);

        LeanTween.moveY(text_m.gameObject, chosen.transform.position.y + 0.2F, duration)
            .setOnComplete(() =>
            {
                text_m.gameObject.SetActive(false);
            });
    }
}
