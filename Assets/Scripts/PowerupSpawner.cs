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

    public SpawnPoint[] spawnPoints;

    [Header("Powerup Prefabs")]
    public GameObject medkitPrefab;
    public GameObject bandagePrefab;
    public GameObject energyDrinkPrefab;
    public GameObject ammoBoxPrefab; // ✅ New ammo box prefab

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

        // Ensure one medkit
        int medkitIndex = allIndexes[Random.Range(0, allIndexes.Count)];
        SpawnPowerupAt(medkitIndex, medkitPrefab);
        allIndexes.Remove(medkitIndex);

        // Calculate how many spawns must remain empty
        int minEmpty = Mathf.Max(2, spawnPoints.Length / 5); // 20% minimum, at least 2

        int maxPowerups = spawnPoints.Length - minEmpty - 1; // already placed 1 medkit
        int powerupsToSpawn = Mathf.Min(maxPowerups, allIndexes.Count);

        // Shuffle list
        Shuffle(allIndexes);

        for (int i = 0; i < powerupsToSpawn; i++)
        {
            int index = allIndexes[i];
            GameObject prefabToSpawn = GetRandomPowerup();
            SpawnPowerupAt(index, prefabToSpawn);
        }

        // Remaining slots will be empty
    }

    void SpawnPowerupAt(int index, GameObject prefab)
    {
        if (index < 0 || index >= spawnPoints.Length || prefab == null) return;

        Transform spawnLoc = spawnPoints[index].location;
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
        float roll = Random.value;

        if (roll < 0.3f) return bandagePrefab;
        else if (roll < 0.6f) return energyDrinkPrefab;
        else if (roll < 0.9f) return ammoBoxPrefab; // ✅ 30% chance for ammo box
        else return medkitPrefab; // ✅ 10% chance for extra medkit (not the guaranteed one)
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
