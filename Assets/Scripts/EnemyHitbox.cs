using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [Tooltip("Check this if this collider is part of the enemy's head.")]
    public bool isHead = false;

    [Tooltip("Reference to the Enemy script. Will auto-assign from parent if not set.")]
    public Enemy enemy;

    void Start()
    {
        // Auto-assign enemy from parent if not set manually
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
            if (enemy == null)
            {
                Debug.LogWarning("⚠️ Enemy reference not found in parent for " + gameObject.name);
            }
        }
    }

    public void ApplyDamage(float amount)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(amount, isHead);
        }
        else
        {
            Debug.LogWarning("❌ Enemy reference missing on " + gameObject.name + ". Damage not applied.");
        }
    }
}
