using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCard
{
    public class DissolveOverTime : MonoBehaviour
    {
        public float FadeTime;

        private void Start()
        {
            StartCoroutine(FadeInOut());
        }

        IEnumerator FadeInOut()
        {
            if (FadeTime != 0)
            {
                var t = 0f;

                while (GetComponent<Card>().BurningAmount < 1)
                {
                    t += Time.deltaTime / FadeTime;

                    GetComponent<Card>().BurningAmount = Mathf.Lerp(0, 1, Mathf.SmoothStep(0.0f, 1.0f, t));

                    yield return new WaitForEndOfFrame();
                }

                t = 0f;

                while (GetComponent<Card>().BurningAmount > 0)
                {
                    t += Time.deltaTime / FadeTime;

                    GetComponent<Card>().BurningAmount = Mathf.Lerp(1, 0, Mathf.SmoothStep(0.0f, 1.0f, t));

                    yield return new WaitForEndOfFrame();
                }
            }

            yield return FadeInOut();
        }
    }
}