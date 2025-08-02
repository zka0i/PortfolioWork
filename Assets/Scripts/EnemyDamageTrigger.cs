using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageInterval = 1f;
    public float damagePerTick = 5f;

    private float lastDamageTime;
    private Generator generator;

    private void Start()
    {
        // Try to find Generator component in parent
        generator = GetComponentInParent<Generator>();
        if (generator == null)
        {
            Debug.LogWarning("⚠️ Generator script not found in parent!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"💥 Trigger entered by: {other.name} (Tag: {other.tag})");

        // Only apply damage if EnemyZone is inside the trigger
        if (other.CompareTag("EnemyZone"))
        {
            if (generator != null && Time.time - lastDamageTime >= damageInterval)
            {
                generator.TakeDamage(damagePerTick);
                lastDamageTime = Time.time;
                Debug.Log("⚙️ Enemy is damaging the generator!");
            }
        }
    }
}
