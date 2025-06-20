using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Damage Settings")]
    public float damageAmount = 10f;
    public float damageInterval = 1f;
    [HideInInspector] public float lastDamageTime;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip headshotSound;
    public AudioClip bodyHitSound;

    void Start()
    {
        currentHealth = maxHealth;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogWarning("❌ No AudioSource found on this enemy.");
            }
        }
    }

    public void TakeDamage(float amount, bool isHeadshot = false)
    {
        bool willDie = currentHealth - amount <= 0;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("☠️ Enemy took damage: " + amount + (isHeadshot ? " (HEAD)" : " (BODY)"));

        if (audioSource != null)
        {
            if (isHeadshot && willDie && headshotSound != null)
            {
                Debug.Log("🔊 Playing HEADSHOT DEATH sound");
                audioSource.PlayOneShot(headshotSound);
            }
            else if (bodyHitSound != null)
            {
                Debug.Log("🔊 Playing regular hit sound");
                audioSource.PlayOneShot(bodyHitSound);
            }
        }

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
