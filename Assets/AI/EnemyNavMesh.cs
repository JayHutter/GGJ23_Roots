using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMesh : MonoBehaviour
{

    private enum State
    {
        ROAM,
        ACTIVE,
        NONE
    }


    [SerializeField] private UnityEngine.AI.NavMeshAgent nav_mesh_agent;
    [SerializeField] private Transform ai_target_player;
    [SerializeField] private float ai_aggro_range = 0;
    [SerializeField] private State ai_state = State.NONE;

    private void Awake()
    {
        nav_mesh_agent = GetComponent<NavMeshAgent>();
        if (ai_aggro_range == 0)
        {
            Debug.LogWarning("AI Agent \"" + this.name + "\" has aggro range of Zero.");
        }
    }

    private void Update()
    {
        Debug.Log(CheckPlayerInRange());
        if (CheckPlayerInRange())
        {
            ai_state = State.ACTIVE;
        }
        else
        {
            ai_state = State.ROAM;
        }

        Navigate();
    }

    private bool CheckPlayerInRange()
    {
        if (Vector3.Distance(this.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < ai_aggro_range)
            return true;
        return false;
    }

    private void Navigate()
    {
        switch(ai_state)
        {
            case State.NONE:
                nav_mesh_agent.destination = this.transform.position;
                break;
            case State.ROAM:
                nav_mesh_agent.destination = this.transform.position;
                break;
            case State.ACTIVE:
                ai_target_player = GameObject.FindGameObjectWithTag("Player").transform;
                nav_mesh_agent.destination = ai_target_player.position;
                break;
        }
        
    }

    private void OnDrawGizmos()
    {
        // Draw Sphere to indicate agent aggro range
        Gizmos.DrawWireSphere(this.transform.position, ai_aggro_range);
    }
}
