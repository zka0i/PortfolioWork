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
    private bool isDestroyed = false;

    // ✅ Public read-only access for other scripts
    public float CurrentHealth => currentHealth;

    // ✅ Public property to check if the generator is destroyed
    public bool IsDestroyed => isDestroyed;

    void Start()
    {
        currentHealth = maxHealth;
        isDestroyed = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        if (destroyedPrefab != null)
        {
            Instantiate(destroyedPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
