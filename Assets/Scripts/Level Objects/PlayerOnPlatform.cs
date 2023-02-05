using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnPlatform : MonoBehaviour
{
    private GameObject player;
    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    player = other.gameObject;
        //    player.transform.SetParent(this.transform);
        //}

    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag("Player"))
        //{
        //    player.transform.SetParent(null);
        //    player = null;
        //}
    }
}
