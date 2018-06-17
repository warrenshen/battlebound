using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {
    public float duration = 1.0F;
    private Light lt;
    public bool followCamPositionY = true;

    private float initialIntensity;

    void Start()
    {
        lt = GetComponent<Light>();
        initialIntensity = lt.intensity;

    }
    void Update()
    {
        float phi = Time.time / duration * 2 * Mathf.PI;
        float amplitude = initialIntensity + Mathf.Cos(phi) * initialIntensity/4.0f;
        lt.intensity = amplitude;
        if (followCamPositionY)
            transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
    }
}
