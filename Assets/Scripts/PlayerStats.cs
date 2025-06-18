using UnityEngine;
using UnityEngine.UI;

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

    private PlayerMovement movement;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthBar) healthBar.maxValue = maxHealth;
        if (staminaBar) staminaBar.maxValue = maxStamina;

        movement = GetComponent<PlayerMovement>();
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
}
