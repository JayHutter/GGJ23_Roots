using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuddleSpawner : MonoBehaviour
{
    [SerializeField] private CollectablePuddle puddlePrefab;
    CollectablePuddle spawnedPuddle;
    private bool isSpawning = false;

    private void Start()
    {
        StartCoroutine(SpawnPuddleAfter(0));
    }

    private void Update()
    {
        if (spawnedPuddle.GetAmount() <= 0 && !isSpawning)
            SpawnPuddleAfter(5);
    }

    IEnumerator SpawnPuddleAfter(float time)
    {
        isSpawning = true;
        spawnedPuddle = null;
        yield return new WaitForSeconds(time);
        spawnedPuddle = Instantiate(puddlePrefab, transform.position, transform.rotation, transform);
        isSpawning = false;
    }
}
