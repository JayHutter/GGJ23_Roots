using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public bool leftRotation = true;

    void Update()
    {
        if (leftRotation)
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * rotationSpeed, 0));
        }

        else
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * -rotationSpeed, 0));
        }
    }
}
