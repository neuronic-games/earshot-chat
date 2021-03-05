using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float   degreesPerSecond;
    public Vector3 axes;


    void Update()
    {
        transform.Rotate(axes * (degreesPerSecond * Time.deltaTime));
    }
}