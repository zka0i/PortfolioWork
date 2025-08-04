using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    public float damageInterval = 1f;
    public float damagePerTick = 5f;

    private float lastDamageTime;

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && Time.time - lastDamageTime >= damageInterval)
        {
            PlayerStats stats = GetComponentInParent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damagePerTick);
                lastDamageTime = Time.time;
                Debug.Log("☠️ Player took damage from enemy.");
            }
            else
            {
                Debug.LogWarning("❌ PlayerStats not found in parent!");
            }
        }
    }
}
