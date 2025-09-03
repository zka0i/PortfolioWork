using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarbedWire : MonoBehaviour
{
    [Header("Effect Settings")]
    public float slowMultiplier = 0.5f;
    public float damagePerSecond = 5f;

    [Header("Health Settings")]
    public float maxHealth = 50f;
    private float currentHealth;
    public bool IsDestroyed { get; private set; } = false;

    private Dictionary<NavMeshAgent, float> slowedAgents = new Dictionary<NavMeshAgent, float>();
    private Dictionary<Enemy, float> damageTimers = new Dictionary<Enemy, float>();

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // 🩸 Called when BarbedWire itself takes damage
    public void TakeDamage(float amount)
    {
        if (IsDestroyed) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"🛠️ BarbedWire took {amount} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDestroyed) return;

        IsDestroyed = true;
        Debug.Log("💥 BarbedWire destroyed!");

        // ✅ Restore all slowed enemies before destroying
        foreach (var pair in slowedAgents)
        {
            if (pair.Key != null)
            {
                pair.Key.speed = pair.Value;
                Debug.Log($"🏃 Restored {pair.Key.name} speed to {pair.Value} (wire destroyed)");
            }
        }
        slowedAgents.Clear();
        damageTimers.Clear();

        // Disable its collider and visuals
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;

        Destroy(gameObject, 2f); // optional delay for cleanup
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsDestroyed) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (!damageTimers.ContainsKey(enemy))
                damageTimers.Add(enemy, 0f);

            NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
            if (agent != null && !slowedAgents.ContainsKey(agent))
            {
                float originalSpeed = agent.speed;
                slowedAgents.Add(agent, originalSpeed);

                agent.speed = originalSpeed * slowMultiplier;
                Debug.Log($"🐌 Slowed {agent.name} from {originalSpeed} to {agent.speed}");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsDestroyed) return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && damageTimers.ContainsKey(enemy))
        {
            float lastTime = damageTimers[enemy];
            if (Time.time - lastTime >= 1f)
            {
                // Enemy takes damage
                enemy.TakeDamage(damagePerSecond);

                // BarbedWire ALSO takes damage from enemy pushing through
                TakeDamage(1f); // ⚡ each tick damages the wire too (tune this value)

                damageTimers[enemy] = Time.time;
                Debug.Log($"🔥 BarbedWire dealt {damagePerSecond} damage to: {enemy.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (damageTimers.ContainsKey(enemy))
                damageTimers.Remove(enemy);

            NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
            if (agent != null && slowedAgents.ContainsKey(agent))
            {
                float originalSpeed = slowedAgents[agent];
                agent.speed = originalSpeed;
                slowedAgents.Remove(agent);
                Debug.Log($"🏃 Restored {agent.name} speed to {originalSpeed}");
            }
        }
    }
}
