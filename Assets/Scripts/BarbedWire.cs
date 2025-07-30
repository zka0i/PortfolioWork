using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarbedWire : MonoBehaviour
{
    public float slowMultiplier = 0.5f;
    public float damagePerSecond = 5f;

    private Dictionary<NavMeshAgent, float> slowedAgents = new Dictionary<NavMeshAgent, float>();
    private Dictionary<Enemy, float> damageTimers = new Dictionary<Enemy, float>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("🧪 OnTriggerEnter hit: " + other.name);

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (!damageTimers.ContainsKey(enemy))
            {
                damageTimers.Add(enemy, 0f);
                Debug.Log("☠️ Started damaging enemy: " + enemy.name);
            }

            NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
            if (agent != null && !slowedAgents.ContainsKey(agent))
            {
                float originalSpeed = agent.speed;
                slowedAgents.Add(agent, originalSpeed);

                agent.speed = originalSpeed * slowMultiplier;
                Debug.Log($"🐌 Slowed {agent.name} from {originalSpeed} to {agent.speed}");
            }
        }
        else
        {
            Debug.Log("👻 Triggered object is not an Enemy: " + other.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && damageTimers.ContainsKey(enemy))
        {
            float lastTime = damageTimers[enemy];
            if (Time.time - lastTime >= 1f)
            {
                enemy.TakeDamage(damagePerSecond);
                damageTimers[enemy] = Time.time;
                Debug.Log($"🔥 Dealt {damagePerSecond} damage to: {enemy.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("🚪 OnTriggerExit hit: " + other.name);

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (damageTimers.ContainsKey(enemy))
            {
                damageTimers.Remove(enemy);
                Debug.Log("💨 Stopped damaging enemy: " + enemy.name);
            }

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
