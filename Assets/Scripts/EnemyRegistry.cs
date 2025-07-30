using System.Collections.Generic;
using UnityEngine;

public class EnemyRegistry : MonoBehaviour
{
    public static List<Enemy> allEnemies = new List<Enemy>();

    public static void Register(Enemy enemy)
    {
        if (!allEnemies.Contains(enemy))
        {
            allEnemies.Add(enemy);
        }
    }

    public static void Unregister(Enemy enemy)
    {
        if (allEnemies.Contains(enemy))
        {
            allEnemies.Remove(enemy);
        }
    }
}
