using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnInterval = 2f;

    [Header("Timer Settings")]
    public float spawnDuration = 180f; // 3 mins
    public Text timerText;
    public Text remainingEnemiesText;

    private float timer;
    private bool spawningStopped = false;
    private float spawnCooldown = 0f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentSpawnIndex = 0;

    void Start()
    {
        timer = spawnDuration;
        UpdateTimerUI();

        if (remainingEnemiesText != null)
            remainingEnemiesText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!spawningStopped)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0)
            {
                spawningStopped = true;
                ShowRemainingEnemiesUI();
                return;
            }

            // Spawning logic
            if (Time.time >= spawnCooldown)
            {
                SpawnEnemy();
                spawnCooldown = Time.time + spawnInterval;
            }
        }

        // Cleanup dead enemies
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (spawningStopped && activeEnemies.Count == 0)
        {
            if (remainingEnemiesText != null)
                remainingEnemiesText.text = "All enemies defeated!";
            Debug.Log("All enemies defeated!");
        }
        else if (spawningStopped)
        {
            UpdateRemainingUI();
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefab == null) return;

        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Ensure AudioSource is enabled
        AudioSource audio = enemy.GetComponent<AudioSource>();
        if (audio != null && !audio.enabled)
            audio.enabled = true;

        activeEnemies.Add(enemy);

        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = string.Format("Time Left: {0:00}:{1:00}", minutes, seconds);
    }

    void ShowRemainingEnemiesUI()
    {
        if (remainingEnemiesText != null)
        {
            remainingEnemiesText.gameObject.SetActive(true);
            UpdateRemainingUI();
        }
    }

    void UpdateRemainingUI()
    {
        if (remainingEnemiesText != null)
        {
            remainingEnemiesText.text = $"Kill the remaining:\n{activeEnemies.Count}";
        }
    }
}
