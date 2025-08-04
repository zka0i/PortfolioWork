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

    private PlayerMovement movement;
    private bool isUsingEnergyDrink = false;
    private float originalSprintSpeed;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar) healthBar.maxValue = maxHealth;
        if (staminaBar) staminaBar.maxValue = maxStamina;

        movement = GetComponent<PlayerMovement>();
        if (movement != null)
            originalSprintSpeed = movement.sprintSpeed;
    }

    void Update()
    {
        UpdateUI();
        HandleStamina();
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

        // Add custom game over / respawn logic here
        // For example: disable movement, show game over screen, etc.
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"❤️ Player healed: {amount}. Current HP: {currentHealth}");
    }

    public void UseMedkit()
    {
        Heal(maxHealth);
    }

    public void UseBandage()
    {
        Heal(30f);
    }

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
}
