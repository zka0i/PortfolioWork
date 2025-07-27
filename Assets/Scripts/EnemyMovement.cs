using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private float baseSpeed;
    private float speedResetTimer;
    private bool isSlowed = false;
    private bool isDead = false;

    private Enemy enemy;

    [Header("Slowdown Settings")]
    public float slowResetTime = 2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        baseSpeed = agent.speed;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

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

        // Restore speed after timer
        if (isSlowed)
        {
            speedResetTimer -= Time.deltaTime;
            if (speedResetTimer <= 0f)
            {
                agent.speed = baseSpeed;
                isSlowed = false;
            }
        }
    }

    // Called from BarbedWire
    public void ApplySpeedMultiplier(float multiplier)
    {
        if (agent == null) return;
        if (isDead) return;

        agent.speed = baseSpeed * multiplier;
        isSlowed = true;
        speedResetTimer = slowResetTime;
    }
}
