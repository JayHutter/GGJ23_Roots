using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Rigidbody currentLeaf;
    private float attraction = 80f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Leaf"))
        {
            currentLeaf = other.GetComponent<Rigidbody>();
            currentLeaf.isKinematic = false;
        }
    }

    private void FixedUpdate()
    {
        if (currentLeaf != null)
            currentLeaf.AddForce((transform.position - currentLeaf.transform.position).normalized * attraction);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Leaf"))
        {
            currentLeaf.isKinematic = true;
            currentLeaf = null;
        }
    }

    private void OnDisable()
    {
        if (currentLeaf != null)
        {
            currentLeaf.isKinematic = true;
            currentLeaf = null;
        }
    }
}
