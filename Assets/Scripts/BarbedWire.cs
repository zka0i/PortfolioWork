using UnityEngine;
using UnityEngine.AI;

public class BarbedWire : MonoBehaviour
{
    [Header("Slowdown Settings")]
    public float slowMultiplier = 0.1f;

    [Header("Damage Settings")]
    public float damagePerSecond = 5f;
    public float damageInterval = 1f;

    private void OnTriggerStay(Collider other)
    {
        // Try to find Enemy in parent
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null || enemy.IsDead()) return;

        // Get components from the same GameObject as the Enemy
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent == null) return;

        // Slow down only once using BarbedWireSlowMemory
        if (!enemy.TryGetComponent<BarbedWireSlowMemory>(out var memory))
        {
            memory = enemy.gameObject.AddComponent<BarbedWireSlowMemory>();
            memory.originalSpeed = agent.speed;
            agent.speed = memory.originalSpeed * slowMultiplier;
        }

        // Apply damage over time
        if (Time.time - memory.lastDamageTime >= damageInterval)
        {
            enemy.TakeDamage(damagePerSecond);
            memory.lastDamageTime = Time.time;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null) return;

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        BarbedWireSlowMemory memory = enemy.GetComponent<BarbedWireSlowMemory>();

        if (agent != null && memory != null)
        {
            agent.speed = memory.originalSpeed;
            Destroy(memory);
        }
    }
}

public class BarbedWireSlowMemory : MonoBehaviour
{
    public float originalSpeed;
    public float lastDamageTime;
}
