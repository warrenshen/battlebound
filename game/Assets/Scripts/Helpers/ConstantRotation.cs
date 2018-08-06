using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public bool x = false;
    public bool y = true;
    public bool z = false;

    public float rotateSpeed = 10; // set it in the  inspector

    public bool random;
    private Vector3 randomDirection;

    private void Awake()
    {
        if (random)
        {
            randomDirection = Random.insideUnitSphere;
        }
    }

    private void Update()
    {
        if (randomDirection == null)
        {
            Rotate();
        }
        else
        {
            RotateRandom();
        }
    }


    private void Rotate()
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

    private void RotateRandom()
    {
        transform.Rotate(randomDirection, rotateSpeed * Time.deltaTime, Space.World);
    }
}
