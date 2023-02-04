using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAnt : MonoBehaviour
{
    [SerializeField] private GameObject enemy_ant_prefab;
    [SerializeField] private float enemy_spawn_interval;
    [SerializeField] private GameObject queen_booty;
    [SerializeField] private float queen_aggro_range;
    private bool triggered = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Vector3.Distance(this.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < queen_aggro_range 
            && triggered == false)
        {
            StartCoroutine(SpawnEnemyAnt(enemy_spawn_interval, enemy_ant_prefab));
            triggered = true;
        }
    }

    private IEnumerator SpawnEnemyAnt(float _interval, GameObject _enemy)
    {
        yield return new WaitForSeconds(_interval);
        GameObject new_enemy1 = Instantiate(_enemy, queen_booty.transform.position, Quaternion.identity);
        GameObject new_enemy2 = Instantiate(_enemy, queen_booty.transform.position, Quaternion.identity);
        GameObject new_enemy3 = Instantiate(_enemy, queen_booty.transform.position, Quaternion.identity);
        StartCoroutine(SpawnEnemyAnt(_interval, _enemy));
    }
}
