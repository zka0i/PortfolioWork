using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Generator Stats")]
    public float maxHealth = 200f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("? Generator took damage: " + amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("?? Generator destroyed!");
        // Optional: Trigger fail state or shutdown
    }
}
