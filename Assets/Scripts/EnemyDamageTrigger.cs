using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    public Enemy parentEnemy;

    private void OnTriggerStay(Collider other)
    {
        // Damage Player
        if (other.CompareTag("Player"))
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null && Time.time - parentEnemy.lastDamageTime >= parentEnemy.damageInterval)
            {
                player.TakeDamage(parentEnemy.damageAmount);
                parentEnemy.lastDamageTime = Time.time;

                Debug.Log("💢 Enemy Trigger: Damaged player for " + parentEnemy.damageAmount);
            }
        }

        // Damage Generator
        Generator generator = other.GetComponent<Generator>();
        if (generator != null && Time.time - parentEnemy.lastDamageTime >= parentEnemy.damageInterval)
        {
            generator.TakeDamage(parentEnemy.damageAmount);
            parentEnemy.lastDamageTime = Time.time;

            Debug.Log("⚠️ Enemy Trigger: Damaged generator for " + parentEnemy.damageAmount);
        }
    }
}
