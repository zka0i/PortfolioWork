using UnityEngine;
using System.Collections.Generic;

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

    [Header("Debug")]
    public bool enableDebugLogs = false;

    private EnemyMovement enemyMovement;

    private void Awake()
    {
        EnemyHitbox[] hitboxes = GetComponentsInChildren<EnemyHitbox>();
        foreach (EnemyHitbox hb in hitboxes)
        {
            hb.enemy = this;
        }

        enemyMovement = GetComponent<EnemyMovement>();
        EnemyRegistry.Register(this);
    }

    private void OnDestroy()
    {
        EnemyRegistry.Unregister(this);
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && enableDebugLogs)
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

        if (enableDebugLogs)
            Debug.Log("☠️ Enemy took damage: " + amount + (isHeadshot ? " (HEAD)" : " (BODY)"));

        if (audioSource != null && !isDying)
        {
            if (isHeadshot && willDie && headshotSound != null)
            {
                if (enableDebugLogs) Debug.Log("🔊 Playing HEADSHOT DEATH sound");
                audioSource.PlayOneShot(headshotSound);
            }
            else if (!willDie && bodyHitSound != null)
            {
                if (enableDebugLogs) Debug.Log("🔊 Playing BODY hit sound");
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

        if (enableDebugLogs)
            Debug.Log("☠️ Trap damaged enemy silently: " + amount);

        if (currentHealth <= 0f)
        {
            Die(false);
        }
    }

    private void Die(bool isHeadshot)
    {
        isDying = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

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

    public bool IsDead()
    {
        return isDying;
    }

    public bool IsDying()
    {
        return isDying;
    }
}
