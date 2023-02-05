using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RopeNode : MonoBehaviour
{
    Transform playerTransform;
    private float disableDistance = 50;
    private float disableDistanceSqr;

    private Collider col;

    private void Start()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player)
            playerTransform = player.transform;

        disableDistanceSqr = disableDistance * disableDistance;
        col = GetComponent<Collider>();
    }
}
