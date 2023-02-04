using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider))]
public class Puddle : MonoBehaviour
{
    [SerializeField] VisualEffect vfx;
    private float amount = 10;
    private float maxAmount = 10;

    [SerializeField] float radius = 0.5f;
    private SphereCollider sphereCollider;

    private void Start()
    {
        amount = maxAmount;
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius  = radius;
        vfx.SetFloat("Radius", radius);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            amount -= Time.deltaTime;
            vfx.SetFloat("Scale", Mathf.InverseLerp(0, maxAmount, amount));

            if (amount <= 0)
                Destroy(gameObject);
        }
    }
}
