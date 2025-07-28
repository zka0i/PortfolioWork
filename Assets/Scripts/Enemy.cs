using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDying = false;

    [Header("Damage Settings")]
    public float damageAmount = 10f;
    public float damageInterval = 1f;
    [HideInInspector] public float lastDamageTime;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip headshotSound;
    public AudioClip bodyHitSound;

    private EnemyMovement enemyMovement;

    private void Awake()
    {
        // Assign this Enemy to all child hitboxes
        EnemyHitbox[] hitboxes = GetComponentsInChildren<EnemyHitbox>();
        foreach (EnemyHitbox hb in hitboxes)
        {
            hb.enemy = this;
        }

        // Get movement reference
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Start()
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

    // 🔫 Normal damage from bullets (plays sound)
    public void TakeDamage(float amount, bool isHeadshot = false)
    {
        if (isDying) return;

        bool willDie = currentHealth - amount <= 0;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("☠️ Enemy took damage: " + amount + (isHeadshot ? " (HEAD)" : " (BODY)"));

        if (audioSource != null && !isDying)
        {
            if (isHeadshot && willDie && headshotSound != null)
            {
                Debug.Log("🔊 Playing HEADSHOT DEATH sound");
                audioSource.PlayOneShot(headshotSound);
            }
            else if (!willDie && bodyHitSound != null)
            {
                Debug.Log("🔊 Playing BODY hit sound");
                audioSource.PlayOneShot(bodyHitSound);
            }
        }

        if (currentHealth <= 0f)
        {
            Die(isHeadshot);
        }
    }

    // ⚠️ Silent damage (e.g., from barbed wire) — NO audio
    public void TakeDamage(float amount)
    {
        if (isDying) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("☠️ Trap damaged enemy silently: " + amount);

        if (currentHealth <= 0f)
        {
            Die(false);
        }
    }

    // 🧠 Common death logic
    private void Die(bool isHeadshot)
    {
        isDying = true;

        // Disable collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Hide all renderers
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;

        float destroyDelay = 0f;

        if (isHeadshot && headshotSound != null)
        {
            destroyDelay = headshotSound.length;
        }
        else if (bodyHitSound != null)
        {
            destroyDelay = bodyHitSound.length;
        }

        Destroy(gameObject, destroyDelay);
    }

    // 🐌 Barbed wire slows the enemy
    public void ApplySpeedMultiplier(float multiplier)
    {
        if (enemyMovement != null)
        {
            enemyMovement.ApplySpeedMultiplier(multiplier);
        }
    }

    public void ResetSpeedMultiplier()
    {
        if (enemyMovement != null)
        {
            enemyMovement.ResetSpeedMultiplier();
        }
    }

    // ✅ Public method so other scripts (like movement) can know if enemy is dead
    public bool IsDead()
    {
        return isDying;
    }

    // ✅ Needed by EnemyMovement and others to check if enemy is dying
    public bool IsDying()
    {
        return isDying;
    }
}
