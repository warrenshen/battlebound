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
    }

    public void ShowTextAtTarget(Transform target, string value, Color color, float duration = 1.5f)
    {
        Transform chosen = transform.GetChild(index);
        TextMeshPro text_m = chosen.GetComponent<TextMeshPro>();

        chosen.position = target.transform.position;
        text_m.text = value;
        text_m.color = color;
        text_m.alpha = 0;

        index = (index + 1) % transform.childCount;
        text_m.gameObject.SetActive(true);

        LeanTween.alphaVertex(text_m.gameObject, 1, duration).setEaseShake();
        LeanTween.moveY(text_m.gameObject, chosen.transform.position.y + 0.1F, duration)
          .setOnComplete(() =>
          {
              text_m.gameObject.SetActive(false);
          });
    }
}
