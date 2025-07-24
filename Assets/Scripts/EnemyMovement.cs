using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private float baseSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    public void ApplySpeedMultiplier(float multiplier)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        agent.speed = baseSpeed * multiplier;
    }
}
