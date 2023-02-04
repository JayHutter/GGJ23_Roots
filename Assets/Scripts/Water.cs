using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        if (other.layer == LayerMask.NameToLayer("Growable"))
        {
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y + 1, other.transform.position.z);
        }
    }
}
