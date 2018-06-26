using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HyperCard
{
    public class RotateOverTime : MonoBehaviour
    {
        public float RotationSpeed;

        void Start()
        {

        }

        void Update()
        {
            transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
        }
    }
}