using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public List<Rigidbody> leaves = new List<Rigidbody>();
    private float attraction = 80f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Leaf"))
        {
            leaves.Add(other.transform.GetComponent<Rigidbody>());
        }

        foreach(var leaf in leaves)
        {
            leaf.isKinematic = false;
        }
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            if (leaves.Count > 0)
            {
                List<Rigidbody> leavesToRemove = new List<Rigidbody>();
                foreach (var leaf in leaves)
                {
                    leavesToRemove.Add(leaf);
                }

                foreach (var leaf in leavesToRemove)
                {
                    leaf.isKinematic = true;
                    leaves.Remove(leaf);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (leaves.Count > 0)
        {
            foreach (var leaf in leaves)
            {
                leaf.AddForce((transform.position - leaf.transform.position).normalized * attraction);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Leaf"))
        {
            if (leaves.Count > 0)
            {
                List<Rigidbody> leavesToRemove = new List<Rigidbody>();
                foreach (var leaf in leaves)
                {
                    leavesToRemove.Add(leaf);
                }

                foreach (var leaf in leavesToRemove)
                {
                    leaf.isKinematic = true;
                    leaves.Remove(leaf);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (leaves.Count > 0)
        {
            List<Rigidbody> leavesToRemove = new List<Rigidbody>();
            foreach (var leaf in leaves)
            {
                leavesToRemove.Add(leaf);
            }

            foreach (var leaf in leavesToRemove)
            {
                leaf.isKinematic = true;
                leaves.Remove(leaf);
            }
        }
    }
}
