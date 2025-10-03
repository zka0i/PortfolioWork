using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyVariant
    {
        public GameObject prefab;
        [Range(0f, 100f)] public float baseWeight = 10f;
        [Range(0f, 5f)] public float weightIncreasePerWave = 1f;
    }

    [Header("Spawn Settings")]
    public List<EnemyVariant> enemyVariants = new List<EnemyVariant>();
    public List<Transform> spawnPoints = new List<Transform>();

    [Tooltip("Base interval between each spawn tick (will be divided by spawnRateMultiplier)")]
    public float groupSpawnInterval = 2f;

    [Tooltip("How many enemies spawn each tick (group)")]
    public int groupSize = 3;

    [Header("Scaling")]
    [HideInInspector] public float spawnRateMultiplier = 1f;
    [HideInInspector] public float enemySpeedMultiplier = 1f;
    [HideInInspector] public float enemyDamageMultiplier = 1f;

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

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int currentSpawnIndex = 0;
    private int currentWave = 1;

    public bool AllEnemiesDefeated => spawningStopped && activeEnemies.Count == 0;

    private float cleanupTimer = 0f;
    private const float cleanupInterval = 2f;

    private float nextSpawnTime = 0f;

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

            // ✅ Spawn enemies in groups at intervals, respecting spawnRateMultiplier
            if (Time.time >= nextSpawnTime)
            {
                SpawnEnemyGroup();
                nextSpawnTime = Time.time + (groupSpawnInterval / Mathf.Max(0.1f, spawnRateMultiplier));
            }
        }

        cleanupTimer += Time.deltaTime;
        if (cleanupTimer >= cleanupInterval)
        {
            cleanupTimer = 0f;
            CleanupNullEnemies();
        }

        if (spawningStopped)
        {
            if (AllEnemiesDefeated)
            {
                if (remainingEnemiesText != null)
                    remainingEnemiesText.text = "All enemies defeated!";
            }
            else
            {
                UpdateRemainingUI();
            }
        }
    }

    void SpawnEnemyGroup()
    {
        if (spawnPoints.Count == 0 || enemyVariants.Count == 0) return;

        for (int i = 0; i < groupSize; i++)
        {
            Transform spawnPoint = spawnPoints[currentSpawnIndex];
            GameObject prefabToSpawn = GetWeightedEnemyPrefab();
            if (prefabToSpawn == null) return;

            GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

            // ✅ Scale NavMeshAgent speed using EnemyMovement's originalSpeed
            NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (agent != null && movement != null)
            {
                agent.speed = movement.originalSpeed * enemySpeedMultiplier;
            }

            // ✅ Scale enemy damage
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.damageAmount *= enemyDamageMultiplier;
            }

            // ✅ Ensure audio exists
            if (!enemy.TryGetComponent(out AudioSource audio))
            {
                audio = enemy.AddComponent<AudioSource>();
                audio.playOnAwake = false;
            }

            activeEnemies.Add(enemy);
            currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Count;
        }
    }

    GameObject GetWeightedEnemyPrefab()
    {
        float totalWeight = 0f;
        foreach (var variant in enemyVariants)
        {
            totalWeight += variant.baseWeight + (variant.weightIncreasePerWave * (currentWave - 1));
        }

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var variant in enemyVariants)
        {
            float effectiveWeight = variant.baseWeight + (variant.weightIncreasePerWave * (currentWave - 1));
            cumulative += effectiveWeight;

            if (roll <= cumulative)
                return variant.prefab;
        }

        return enemyVariants[0].prefab;
    }

    void CleanupNullEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
                activeEnemies.RemoveAt(i);
        }
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
        nextSpawnTime = Time.time; // ✅ allow immediate spawn when wave starts
    }

    public void ResetSpawner()
    {
        timer = spawnDuration;
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
        currentWave = nightNumber;
        StartCoroutine(StartNightTransition(nightNumber));
    }

    IEnumerator StartNightTransition(int nightNumber)
    {
        if (nightTransitionUI != null && nightText != null && nightBackground != null)
        {
            nightText.text = $"Night {nightNumber}";
            nightTransitionUI.SetActive(true);
            nightBackground.color = new Color(0f, 0f, 0f, 0.7f);

            float duration = 2f;
            float t = 0f;

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
