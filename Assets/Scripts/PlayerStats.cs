using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 15f;
    public float sprintStaminaCost = 20f;
    public Slider staminaBar;

    [Header("Powerup Effects")]
    public float energyDrinkDuration = 10f;
    public float speedBoostMultiplier = 1.5f;

    [Header("Low HP Feedback")]
    public Image bloodOverlay;                 // Assign a semi-transparent red UI image in Canvas
    public float fadeSpeed = 2f;
    public float lowHPThreshold = 30f;         // HP where effects kick in
    public AudioSource heartbeatSource;        // Heartbeat audio source
    public AudioClip heartbeatClip;
    public AudioLowPassFilter lowPassFilter;   // For muffled hearing effect
    public float muffledCutoff = 800f;         // Low-pass cutoff freq when low HP
    public float normalCutoff = 22000f;        // Normal hearing cutoff

    private PlayerMovement movement;
    private bool isUsingEnergyDrink = false;
    private float originalSprintSpeed;
    private bool isDead = false;

    private float bloodTargetAlpha = 0f;       // UI fade target

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar) healthBar.maxValue = maxHealth;
        if (staminaBar) staminaBar.maxValue = maxStamina;

        movement = GetComponent<PlayerMovement>();
        if (movement != null)
            originalSprintSpeed = movement.sprintSpeed;

        if (bloodOverlay != null)
        {
            Color c = bloodOverlay.color;
            c.a = 0f;
            bloodOverlay.color = c;
        }

        if (heartbeatSource != null)
        {
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.loop = true;
            heartbeatSource.volume = 0f; // start silent
            heartbeatSource.playOnAwake = false;
        }
    }

    void Update()
    {
        UpdateUI();
        HandleStamina();
        HandleLowHPEffects();
    }

    void UpdateUI()
    {
        if (healthBar) healthBar.value = currentHealth;
        if (staminaBar) staminaBar.value = currentStamina;
    }

    void HandleStamina()
    {
        if (isUsingEnergyDrink || movement == null) return;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && movement.IsMoving();

        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= sprintStaminaCost * Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"☠️ Player took damage: {amount}. Current HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("💀 Player has died.");

        if (movement != null)
            movement.enabled = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SendMessage("ShowLoseScreen");
        }
        else
        {
            Debug.LogWarning("❌ GameManager instance not found!");
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"❤️ Player healed: {amount}. Current HP: {currentHealth}");
    }

    public void UseMedkit() => Heal(maxHealth);
    public void UseBandage() => Heal(30f);

    public void UseEnergyDrink()
    {
        if (isUsingEnergyDrink || movement == null) return;
        StartCoroutine(EnergyDrinkRoutine());
    }

    private IEnumerator EnergyDrinkRoutine()
    {
        isUsingEnergyDrink = true;
        movement.sprintSpeed *= speedBoostMultiplier;
        yield return new WaitForSeconds(energyDrinkDuration);
        movement.sprintSpeed = originalSprintSpeed;
        isUsingEnergyDrink = false;
    }

    // === LOW HP EFFECTS ===
    void HandleLowHPEffects()
    {
        bool lowHP = currentHealth > 0 && currentHealth <= lowHPThreshold;

        if (lowHP)
        {
            bloodTargetAlpha = Mathf.InverseLerp(maxHealth, 0, currentHealth); // more blood as HP lowers
            if (heartbeatSource != null && !heartbeatSource.isPlaying)
                heartbeatSource.Play();

            if (lowPassFilter != null)
                lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, muffledCutoff, Time.deltaTime * fadeSpeed);
        }
        else
        {
            bloodTargetAlpha = 0f;

            if (lowPassFilter != null)
                lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, normalCutoff, Time.deltaTime * fadeSpeed);

            if (heartbeatSource != null && heartbeatSource.isPlaying && heartbeatSource.volume <= 0.01f)
                heartbeatSource.Stop();
        }

        // Fade UI blood overlay
        if (bloodOverlay != null)
        {
            Color c = bloodOverlay.color;
            c.a = Mathf.Lerp(c.a, bloodTargetAlpha, Time.deltaTime * fadeSpeed);
            bloodOverlay.color = c;
        }

        // Fade heartbeat volume
        if (heartbeatSource != null)
        {
            float targetVol = lowHP ? 1f : 0f;
            heartbeatSource.volume = Mathf.Lerp(heartbeatSource.volume, targetVol, Time.deltaTime * fadeSpeed);
        }
    }
}
