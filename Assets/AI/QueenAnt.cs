using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAnt : MonoBehaviour
{
    [SerializeField] private GameObject enemy_ant_prefab;
    [SerializeField] private float enemy_spawn_interval;

    private void Start()
    {
        StartCoroutine(SpawnEnemyAnt(enemy_spawn_interval, enemy_ant_prefab));
    }

    private void Update()
    {
        
    }

    private IEnumerator SpawnEnemyAnt(float _interval, GameObject _enemy)
    {
        yield return new WaitForSeconds(_interval);
        GameObject new_enemy1 = Instantiate(_enemy, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)), Quaternion.identity);
        GameObject new_enemy2 = Instantiate(_enemy, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)), Quaternion.identity);
        GameObject new_enemy3 = Instantiate(_enemy, new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)), Quaternion.identity);
        StartCoroutine(SpawnEnemyAnt(_interval, _enemy));
    }
}
