using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseIdle : MonoBehaviour
{
    public bool affectScale = true;
    public bool affectPosition = true;

    private float pulse;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    public float damping = 0.33f;
    public float frequency = 3f;

    // Use this for initialization
    void Start()
    {
        this.originalScale = transform.localScale;
        this.originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        pulse = Mathf.Sin(Time.time * frequency);
        if (affectScale)
        {
            transform.localScale = this.originalScale + Vector3.one * pulse * damping;
        }
        if (affectPosition)
        {
            transform.position = this.originalPosition + Vector3.up * pulse * damping;
        }
    }
}
