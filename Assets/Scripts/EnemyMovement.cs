using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private bool isDead = false;

    private Enemy enemy;

    [HideInInspector] public float baseSpeed;
    [HideInInspector] public float originalSpeed;
    [HideInInspector] public bool isSlowed = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;
        originalSpeed = baseSpeed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;

        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (isDead) return;

        if (enemy != null && enemy.IsDying())
        {
            isDead = true;
            agent.isStopped = true;
            return;
        }

        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    public void ApplySpeedMultiplier(float multiplier)
    {
        if (agent == null || isDead) return;

        multiplier = Mathf.Clamp(multiplier, 0.01f, 1f); // Prevent zero or negative speed
        agent.speed = baseSpeed * multiplier;
        isSlowed = true;
    }

    public void ResetSpeedMultiplier()
    {
        if (agent == null || isDead) return;

        agent.speed = baseSpeed;
        isSlowed = false;
    }
}
