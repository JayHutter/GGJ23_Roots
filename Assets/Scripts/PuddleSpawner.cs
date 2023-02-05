using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleSpawner : MonoBehaviour
{
    public float range = 50;
    public GameObject puddlePrefab;

    private void Start()
    {
        StartCoroutine(SpawnPuddle());
    }

    IEnumerator SpawnPuddle()
    {
        while(true)
        {
            float xPos = Random.RandomRange(-50, 50);
            float yPos = Random.RandomRange(-50, 50);

            var newPuddle = Instantiate(puddlePrefab, transform.position + new Vector3(xPos, 0, yPos), Quaternion.identity);
            yield return new WaitForSeconds(10);
        }
    }
}
