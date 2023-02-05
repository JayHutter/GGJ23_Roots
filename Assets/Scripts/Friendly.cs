using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendly : MonoBehaviour
{
    [SerializeField] private GameObject reward;

    bool isSafe = true;
    Transform playerTransform;
    [SerializeField] private Transform body;

    private void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        if (reward)
            reward.SetActive(false);
    }

    private void Update()
    {
        CheckSafety();
        LookAtPlayer();
    }

    private void CheckSafety()
    {
        //Check if enemies are near 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSafe)
            return;

        if (other.tag == "Player")
        {
            if (reward)
            {
                reward.SetActive(true);
            }
                //Instantiate(reward, body.position + body.up, Quaternion.identity);

            Debug.Log("You saved NPC!");

            Destroy(this);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 dir = playerTransform.position - body.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);

        body.rotation = rot;
    }
}
