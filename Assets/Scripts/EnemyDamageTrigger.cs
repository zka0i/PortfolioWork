using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    public Enemy parentEnemy;

    private void OnTriggerStay(Collider other)
    {
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
    }
}
