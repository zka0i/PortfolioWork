using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageInterval = 1f;
    public float damagePerTick = 5f;

    private float lastDamageTimeToPlayer;
    private float lastDamageTimeToGenerator;

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"💥 Trigger stay by: {other.name} (Tag: {other.tag})");

        // ✅ Damage the Player
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
            if (playerStats != null && Time.time - lastDamageTimeToPlayer >= damageInterval)
            {
                Debug.Log("✅ Damaging Player");
                playerStats.TakeDamage(damagePerTick);
                lastDamageTimeToPlayer = Time.time;
            }
            else if (playerStats == null)
            {
                Debug.LogWarning("❌ PlayerStats not found!");
            }
        }

        // ✅ Damage the Generator only when in its zone
        else if (other.CompareTag("GeneratorZone"))
        {
            Generator generator = other.GetComponentInParent<Generator>();
            if (generator != null && Time.time - lastDamageTimeToGenerator >= damageInterval)
            {
                Debug.Log("✅ Damaging Generator");
                generator.TakeDamage(damagePerTick);
                lastDamageTimeToGenerator = Time.time;
            }
            else if (generator == null)
            {
                Debug.LogWarning("❌ Generator not found in GeneratorZone parent!");
            }
        }
    }
}
