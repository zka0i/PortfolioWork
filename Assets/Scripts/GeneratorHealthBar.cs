using UnityEngine;
using UnityEngine.UI;

public class GeneratorHealthBar : MonoBehaviour
{
    public Generator generator;
    public Image healthFill;

    // How transparent the bar should get when HP is zero
    [Range(0, 1)]
    public float minAlpha = 0.5f;

    void Update()
    {
        if (generator == null || healthFill == null) return;

        // Update bar fill
        float fillAmount = Mathf.Clamp01(generator.CurrentHealth / generator.maxHealth);
        healthFill.fillAmount = fillAmount;

        // Fade effect: alpha decreases as HP goes down
        Color color = healthFill.color;
        color.a = Mathf.Lerp(minAlpha, 1f, fillAmount); // full alpha at full HP, minAlpha at 0 HP

        // Optional: change color based on health
        if (fillAmount > 0.5f) color = Color.green;
        else if (fillAmount > 0.25f) color = Color.yellow;
        else color = Color.red;

        color.a = Mathf.Lerp(minAlpha, 1f, fillAmount); // preserve alpha
        healthFill.color = color;
    }
}
