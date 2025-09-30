using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isDead = false;

    private Enemy enemy;

    [HideInInspector] public float originalSpeed;
    private float globalMultiplier = 1f; // wave/night scaling
    private float slowMultiplier = 1f;   // barbed wire / trap slowdown

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            originalSpeed = agent.speed; // IMPORTANT: store base speed in Awake so other scripts can read it early
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        // ensure starting speed uses base values
        ApplyFinalSpeed();
    }

    void Update()
    {
        if (isDead) return;

        if (enemy != null && enemy.IsDying())
        {
            isDead = true;
            if (agent != null) agent.isStopped = true;
            return;
        }

        // NOTE: enemy AI controls SetDestination; movement only controls speed/state
    }

    // ✅ Called by spawner / gamemanager to apply wave/global scaling (>1 allowed)
    public void ApplyGlobalSpeedMultiplier(float multiplier)
    {
        if (agent == null || isDead) return;
        globalMultiplier = Mathf.Max(0.01f, multiplier);
        ApplyFinalSpeed();
    }

    // ✅ Slowdown (barbed wire etc.) — multiplier should be <= 1
    public void ApplySpeedMultiplier(float multiplier)
    {
        if (agent == null || isDead) return;
        slowMultiplier = Mathf.Clamp(multiplier, 0.01f, 1f);
        ApplyFinalSpeed();
        // Debug.Log($"🐌 Slow applied: slowMultiplier={slowMultiplier} finalSpeed={agent.speed}");
    }

    // ✅ Reset only the slowdown (restore to the global-scaled speed)
    public void ResetSpeedMultiplier()
    {
        if (agent == null || isDead) return;
        slowMultiplier = 1f;
        ApplyFinalSpeed();
        // Debug.Log($"🏃 Reset slowdown. finalSpeed={agent.speed}");
    }

    private void ApplyFinalSpeed()
    {
        if (agent == null || isDead) return;
        agent.speed = originalSpeed * globalMultiplier * slowMultiplier;
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
