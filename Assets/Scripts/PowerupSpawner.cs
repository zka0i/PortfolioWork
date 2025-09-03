using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerupSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform location;
        [HideInInspector] public GameObject currentPowerup;
    }

    [System.Serializable]
    public class PowerupEntry
    {
        public GameObject prefab;   // Prefab of powerup (can be null = empty)
        [Range(0f, 100f)] public float weight = 10f; // Rarity percentage weight
    }

    public SpawnPoint[] spawnPoints;

    [Header("Powerup Table")]
    public List<PowerupEntry> powerupTable = new List<PowerupEntry>();

    [Header("Spawn Settings")]
    public float respawnDelay = 20f;

    private void Start()
    {
        InitializeSpawns();
    }

    void InitializeSpawns()
    {
        List<int> allIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
            allIndexes.Add(i);

        // Shuffle spawn point list
        Shuffle(allIndexes);

        foreach (int index in allIndexes)
        {
            GameObject prefabToSpawn = GetRandomPowerup();
            if (prefabToSpawn != null)
            {
                SpawnPowerupAt(index, prefabToSpawn);
            }
        }
    }

    void SpawnPowerupAt(int index, GameObject prefab)
    {
        if (index < 0 || index >= spawnPoints.Length) return;

        Transform spawnLoc = spawnPoints[index].location;

        if (prefab == null)
        {
            spawnPoints[index].currentPowerup = null;
            return;
        }

        GameObject spawned = Instantiate(prefab, spawnLoc.position, spawnLoc.rotation);
        spawnPoints[index].currentPowerup = spawned;

        // Hook into OnPickedUp callback
        PowerupPickup pickup = spawned.GetComponent<PowerupPickup>();
        if (pickup != null)
        {
            pickup.OnPickedUp += () =>
            {
                spawnPoints[index].currentPowerup = null;
                StartCoroutine(RespawnAfterDelay(index));
            };
        }
    }

    IEnumerator RespawnAfterDelay(int index)
    {
        yield return new WaitForSeconds(respawnDelay);
        if (spawnPoints[index].currentPowerup == null)
        {
            GameObject prefab = GetRandomPowerup();
            SpawnPowerupAt(index, prefab);
        }
    }

    GameObject GetRandomPowerup()
    {
        if (powerupTable.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var entry in powerupTable)
            totalWeight += entry.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in powerupTable)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.prefab; // prefab can be null = empty spawn
        }

        return null;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
