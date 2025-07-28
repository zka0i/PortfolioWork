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
        EnemyMovement movement = GetEnemyMovementFromCollider(other);

        if (enemy != null)
        {
            float currentTime = Time.time;

            // Deal damage once per second
            if (currentTime - enemy.lastDamageTime >= 1f)
            {
                enemy.TakeDamage(damagePerSecond);
                enemy.lastDamageTime = currentTime;
            }
        }

        // Apply slow every frame
        if (movement != null)
        {
            movement.ApplySpeedMultiplier(slowMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyMovement movement = GetEnemyMovementFromCollider(other);
        if (movement != null)
        {
            // Reset speed to normal
            movement.ApplySpeedMultiplier(1f);
        }
    }

    private Enemy GetEnemyFromCollider(Collider col)
    {
        if (col.CompareTag("Enemy"))
            return col.GetComponent<Enemy>();

        EnemyHitbox hitbox = col.GetComponent<EnemyHitbox>();
        if (hitbox != null)
            return hitbox.enemy;

        Transform root = col.transform.root;
        if (root.CompareTag("Enemy"))
            return root.GetComponent<Enemy>();

        return null;
    }

    private EnemyMovement GetEnemyMovementFromCollider(Collider col)
    {
        if (col.CompareTag("Enemy"))
            return col.GetComponent<EnemyMovement>();

        Transform root = col.transform.root;
        if (root.CompareTag("Enemy"))
            return root.GetComponent<EnemyMovement>();

        return null;
    }
}
