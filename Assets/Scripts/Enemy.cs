using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float damageAmount = 10f;
    public float damageInterval = 1f;

    [HideInInspector] public float lastDamageTime;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy took damage: " + amount + " | Current HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("💀 Enemy died.");
        Destroy(gameObject);
    }
}
