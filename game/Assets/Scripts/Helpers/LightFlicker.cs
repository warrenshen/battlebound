using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {
    public float duration = 1.0F;
    public Light lt;
    public bool followCamPositionY = true;

    void Start()
    {
        lt = GetComponent<Light>();
    }
    void Update()
    {
        float phi = Time.time / duration * 2 * Mathf.PI;
        float amplitude = 10.0F + Mathf.Cos(phi) * 2.0F;
        lt.intensity = amplitude;
        if (followCamPositionY)
            transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
    }
}
