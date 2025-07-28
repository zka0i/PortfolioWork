using UnityEngine;
using System.Collections.Generic;

public class BarbedWire : MonoBehaviour
{
    [Header("Trap Settings")]
    public float damagePerSecond = 10f;
    [Range(0f, 1f)] public float speedMultiplier = 0.2f;

    private List<Enemy> enemiesInWire = new List<Enemy>();

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !enemiesInWire.Contains(enemy))
        {
            enemiesInWire.Add(enemy);
            enemy.ApplySpeedMultiplier(speedMultiplier);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemiesInWire.Contains(enemy))
        {
            enemiesInWire.Remove(enemy);
            enemy.ResetSpeedMultiplier();
        }
    }

    private void Update()
    {
        if (enemiesInWire.Count == 0) return;

        float damage = damagePerSecond * Time.deltaTime;
        for (int i = enemiesInWire.Count - 1; i >= 0; i--)
        {
            Enemy enemy = enemiesInWire[i];

            if (enemy == null || enemy.IsDead())
            {
                enemiesInWire.RemoveAt(i);
                continue;
            }

            enemy.TakeDamage(damage);
        }
    }
}
