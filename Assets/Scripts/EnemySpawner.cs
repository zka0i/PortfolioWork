using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public float spawnInterval = 2f;

    [Header("Scaling")]
    [HideInInspector] public float spawnRateMultiplier = 1f;
    [HideInInspector] public float enemySpeedMultiplier = 1f;

    [Header("Timer Settings")]
    public float spawnDuration = 180f;
    public Text timerText;
    public Text remainingEnemiesText;

    [Header("Night Transition UI")]
    public GameObject nightTransitionUI;
    public Text nightText;
    public Image nightBackground;

    [HideInInspector] public float timer;
    [HideInInspector] public bool spawningStopped = true;

    private float spawnCooldown = 0f;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentSpawnIndex = 0;

    public bool AllEnemiesDefeated => spawningStopped && activeEnemies.Count == 0;

    void Start()
    {
        timer = spawnDuration;
        UpdateTimerUI();
        remainingEnemiesText?.gameObject.SetActive(false);
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
                timerText?.gameObject.SetActive(false);
                ShowRemainingEnemiesUI();
                return;
            }

            if (Time.time >= spawnCooldown)
            {
                SpawnEnemy();
                spawnCooldown = Time.time + (spawnInterval / Mathf.Max(0.1f, spawnRateMultiplier));
            }
        }

        activeEnemies.RemoveAll(enemy => enemy == null);

        if (spawningStopped)
        {
            if (AllEnemiesDefeated)
            {
                remainingEnemiesText.text = "All enemies defeated!";
            }
            else
            {
                UpdateRemainingUI();
            }
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefab == null) return;

        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Apply speed multiplier if enemy has EnemyMovement component
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.ApplySpeedMultiplier(enemySpeedMultiplier);
        }

        // Ensure audio
        AudioSource audio = enemy.GetComponent<AudioSource>();
        if (audio == null)
        {
            audio = enemy.AddComponent<AudioSource>();
            audio.playOnAwake = false;
        }

        activeEnemies.Add(enemy);
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
    }

    void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"Time Left: {minutes:00}:{seconds:00}";
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

    public bool TimerExpired()
    {
        return timer <= 0;
    }

    public void BeginSpawning()
    {
        timer = spawnDuration;
        spawningStopped = false;
        timerText?.gameObject.SetActive(true);
        remainingEnemiesText?.gameObject.SetActive(false);
    }

    public void ResetSpawner()
    {
        timer = spawnDuration;
        spawnCooldown = 0f;
        spawningStopped = true;

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();

        timerText?.gameObject.SetActive(false);
        remainingEnemiesText?.gameObject.SetActive(false);
    }

    public void StartNewNight(int nightNumber)
    {
        StartCoroutine(StartNightTransition(nightNumber));
    }

    IEnumerator StartNightTransition(int nightNumber)
    {
        if (nightTransitionUI != null && nightText != null && nightBackground != null)
        {
            nightText.text = $"Night {nightNumber}";
            nightTransitionUI.SetActive(true);
            nightBackground.color = new Color(0f, 0f, 0f, 0.7f); // semi-transparent black

            float duration = 2f;
            float t = 0f;
            Color initialColor = nightBackground.color;

            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(0.7f, 0f, t / duration);
                nightBackground.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }

            nightTransitionUI.SetActive(false);
        }

        BeginSpawning();
    }

    public void StopSpawning()
    {
        spawningStopped = true;
        timerText?.gameObject.SetActive(false);
    }
}
