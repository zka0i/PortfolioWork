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

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar) healthBar.maxValue = maxHealth;
        if (staminaBar) staminaBar.maxValue = maxStamina;

        movement = GetComponent<PlayerMovement>();
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
        if (isUsingEnergyDrink) return;

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
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
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
        if (isUsingEnergyDrink) return;
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
