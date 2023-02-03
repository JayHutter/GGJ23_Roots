using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{

    [SerializeField] private Transform target_location;
    [SerializeField] private UnityEngine.AI.NavMeshAgent nav_mesh_agent;

    private void Awake()
    {
        nav_mesh_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        nav_mesh_agent.destination = target_location.position;
    }
}
