using UnityEngine;

public class GeneratorDamageZone : MonoBehaviour
{
    public float damagePerTick = 5f;
    public float damageInterval = 1f;

    private float lastDamageTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && Time.time - lastDamageTime >= damageInterval)
        {
            Generator generator = GetComponentInParent<Generator>();
            if (generator != null)
            {
                generator.TakeDamage(damagePerTick);
                Debug.Log("⚙️ Generator damaged by enemy.");
                lastDamageTime = Time.time;
            }
            else
            {
                Debug.LogWarning("❌ Generator not found in parent.");
            }
        }
    }
}
