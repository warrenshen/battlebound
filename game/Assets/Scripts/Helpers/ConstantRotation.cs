using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public bool x = false;
    public bool y = true;
    public bool z = false;

    public float rotateSpeed = 10; // set it in the  inspector

    void Update()
    {
        Rotate();
    }


    void Rotate()
    {
        if (x)
        {
            transform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime, Space.World);
        }
        if (y)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        }
        if (z)
        {
            transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.World);
        }
    }
}
