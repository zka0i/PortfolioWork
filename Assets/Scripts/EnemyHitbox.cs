using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public bool isHead; // true for head, false for body
    public Enemy enemy;

    public void ApplyDamage(float amount)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(amount, isHead);
        }
    }
}
