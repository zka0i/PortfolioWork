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

    void Awake()
    {
        // Auto-assign self to all child hitboxes
        EnemyHitbox[] hitboxes = GetComponentsInChildren<EnemyHitbox>();
        foreach (EnemyHitbox hb in hitboxes)
        {
            hb.enemy = this;
        }
    }

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
        if (isDying) return;

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
            else if (!willDie && bodyHitSound != null)
            {
                Debug.Log("🔊 Playing BODY hit sound");
                audioSource.PlayOneShot(bodyHitSound);
            }
        }

        if (currentHealth <= 0f)
        {
            isDying = true;

            // Disable collider
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Disable all renderers
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
                r.enabled = false;

            // Optional: disable movement or AI scripts here if needed

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
    }
}
