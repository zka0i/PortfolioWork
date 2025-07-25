using UnityEngine;

public class BarbedWire : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 10f;
    [Header("Slowdown Settings")]
    public float slowMultiplier = 0.5f;

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = GetEnemyFromCollider(other);
        if (enemy != null)
        {
            float currentTime = Time.time;
            if (currentTime - enemy.lastDamageTime >= 1f)
            {
                enemy.TakeDamage(damagePerSecond);
                enemy.lastDamageTime = currentTime;
            }

            // Apply slowdown only if not already slowed
            enemy.ApplySpeedMultiplier(slowMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = GetEnemyFromCollider(other);
        if (enemy != null)
        {
            // Reset speed back to normal
            enemy.ApplySpeedMultiplier(1f);
        }
    }

    private Enemy GetEnemyFromCollider(Collider col)
    {
        // Check direct reference
        if (col.CompareTag("Enemy"))
        {
            return col.GetComponent<Enemy>();
        }

        // If it's a child collider like EnemyHitbox
        EnemyHitbox hitbox = col.GetComponent<EnemyHitbox>();
        if (hitbox != null && hitbox.enemy != null)
        {
            return hitbox.enemy;
        }

        // Try root fallback (in case both above fail)
        Transform root = col.transform.root;
        if (root.CompareTag("Enemy"))
        {
            return root.GetComponent<Enemy>();
        }

        return null;
    }
}
