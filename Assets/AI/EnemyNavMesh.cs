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
    [SerializeField] private Transform ai_target_roam;
    [SerializeField] private float ai_aggro_range = 0;
    [SerializeField] private float ai_roam_range = 0;
    [SerializeField] private State ai_state = State.NONE;

    private void Awake()
    {
        nav_mesh_agent = GetComponent<NavMeshAgent>();

        if (ai_aggro_range == 0)
        {
            Debug.LogWarning("AI Agent \"" + this.name + "\" has aggro range of Zero.");
        }
        if (ai_roam_range == 0)
        {
            Debug.LogWarning("AI Agent \"" + this.name + "\" has roam range of Zero.");
        }
    }

    private void Update()
    {
        if (CheckPlayerInRange())
            ai_state = State.ACTIVE;
        else
            ai_state = State.ROAM;

        Navigate();
    }

    private bool CheckPlayerInRange()
    {
        if (Vector3.Distance(this.transform.position, GameObject.FindGameObjectWithTag("Player").transform.position) < ai_aggro_range)
            return true;
        return false;
    }

    private bool UpdateRoamPosition(Vector3 centre, float range, out Vector3 result)
    {
        Vector3 random_point = centre + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(random_point, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
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
                if (nav_mesh_agent.remainingDistance <= nav_mesh_agent.stoppingDistance) // Reached target transform
                {
                    Vector3 pos;
                    if (UpdateRoamPosition(this.transform.position, ai_roam_range, out pos))
                    {
                        Debug.DrawRay(pos, Vector3.up, Color.yellow, 1.0f);
                        nav_mesh_agent.SetDestination(pos);
                    }
                }
                break;
            case State.ACTIVE:
                if (GameObject.FindGameObjectWithTag("Player")) 
                { 
                    ai_target_player = GameObject.FindGameObjectWithTag("Player").transform;
                    nav_mesh_agent.destination = ai_target_player.position;               
                }
                else
                {
                    Debug.LogError("There is no GameObject with Tag \"Player\".");
                }
                break;
        }
        
    }

    private void OnDrawGizmos()
    {
        // Draw Sphere to indicate agent aggro range
        Gizmos.DrawWireSphere(this.transform.position, ai_aggro_range);
    }
}
