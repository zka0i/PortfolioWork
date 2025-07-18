using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Damage Settings")]
    public float damageInterval = 1f;
    public float damagePerTick = 5f;

    [Header("Replacement")]
    public GameObject destroyedPrefab;

    private float lastDamageTime;

    // ✅ Public read-only access for other scripts
    public float CurrentHealth => currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (destroyedPrefab != null)
        {
            Instantiate(destroyedPrefab, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
