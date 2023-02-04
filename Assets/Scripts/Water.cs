using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private float power = 0.1f;
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        if (other.layer == LayerMask.NameToLayer("Growable"))
        {
            other.transform.root.GetComponent<PlantPot>().watered += power;
        }
    }
}
