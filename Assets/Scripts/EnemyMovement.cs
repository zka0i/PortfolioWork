using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isDead = false;
    private bool isSlowed = false;

    private Enemy enemy;

    [HideInInspector] public float originalSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
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

        // 🛑 DO NOT control SetDestination here — EnemyAI handles that!
    }

    // 🐌 Apply slowdown (barbed wire etc.)
    public void ApplySpeedMultiplier(float multiplier)
    {
        if (agent == null || isDead) return;

        multiplier = Mathf.Clamp(multiplier, 0.01f, 1f);
        if (!isSlowed)
        {
            agent.speed = originalSpeed * multiplier;
            isSlowed = true;
            Debug.Log("🐌 Enemy slowed! New speed: " + agent.speed);
        }
    }

    // 🔄 Reset to normal speed
    public void ResetSpeedMultiplier()
    {
        if (agent == null || isDead) return;

        if (isSlowed)
        {
            agent.speed = originalSpeed;
            isSlowed = false;
            Debug.Log("🏃‍♂️ Enemy speed reset to: " + agent.speed);
        }
    }

    public void StopMovement()
    {
        if (agent == null || isDead) return;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }

    public void ResumeMovement()
    {
        if (agent == null || isDead) return;
        agent.isStopped = false;
    }
}
