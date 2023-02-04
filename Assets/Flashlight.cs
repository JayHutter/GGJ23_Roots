using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Rigidbody prevLeaf;
    public Rigidbody currentLeaf;
    private float attraction = 80f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Leaf"))
        {
            if(prevLeaf != currentLeaf)
                prevLeaf = other.GetComponent<Rigidbody>();

            currentLeaf = other.GetComponent<Rigidbody>();
            currentLeaf.isKinematic = false;
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            if (currentLeaf != null)
            {
                currentLeaf.isKinematic = true;
                currentLeaf = null;
            }

            if (prevLeaf != null)
                prevLeaf.isKinematic = true;
        }

        if (prevLeaf == currentLeaf)
        {
            if (prevLeaf != null)
                prevLeaf.isKinematic = true;
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
            if(currentLeaf != null)
            {
                currentLeaf.isKinematic = true;
                currentLeaf = null;
            }
        }
    }

    private void OnDisable()
    {
        if (currentLeaf != null)
        {
            currentLeaf.isKinematic = true;
            currentLeaf = null;
        }

        if (prevLeaf != null)
            prevLeaf.isKinematic = true;
    }
}
