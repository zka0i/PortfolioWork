using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public GameObject winScreen;
    public GameObject loseScreen;
    public Image backgroundFadeImage;
    public TextMeshProUGUI nightText;
    public TextMeshProUGUI intermissionText;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    public float displayTime = 2f;

    [Header("Intermission Settings")]
    public float intermissionDuration = 10f;

    [Header("Audio")]
    public AudioSource nightStartAudioSource;
    public AudioClip nightStartClip;

    [Header("References")]
    public EnemySpawner enemySpawner;
    public Generator[] generators; // ✅ now supports multiple generators
    public PlayerMovement playerMovement;
    public Weapon weapon;
    public MonoBehaviour cameraLookScript;

    [Header("Difficulty Scaling")]
    public float baseSpawnRateMultiplier = 1f;
    public float spawnRateGrowthPerNight = 0.1f;
    public float baseEnemySpeedMultiplier = 1f;
    public float speedGrowthPerNight = 0.1f;

    [Header("Final Wave Helicopter")]
    public GameObject helicopterPrefab;
    public Transform helicopterSpawnPoint;
    public Transform helicopterLandingPoint;
    public Transform helicopterHoverPoint; // ✅ new hover point
    public float helicopterSpeed = 5f;
    public float finalWaveSpawnRateMultiplier = 3f;

    [Header("Debug Testing")]
    public bool startAtFinalWave = false;

    private bool gameEnded = false;
    private bool fading = false;
    private float fadeTimer = 0f;

    private enum FadeState { None, FadeIn, Display, FadeOut }
    private FadeState fadeState = FadeState.None;

    private int currentNight = 1;
    private const int maxNights = 12;

    private bool isIntermission = false;
    private float intermissionTimer = 0f;

    private GameObject activeHelicopter;
    private bool finalWaveTriggered = false;
    private bool reachedHover = false; // ✅ track helicopter progress

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        if (intermissionText != null) intermissionText.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (startAtFinalWave)
            currentNight = maxNights;

        StartNight(currentNight);
    }

    void Update()
    {
        if (gameEnded) return;

        // ✅ Check all generators
        bool allDestroyed = true;
        foreach (var gen in generators)
        {
            if (gen != null && !gen.IsDestroyed)
            {
                allDestroyed = false;
                break;
            }
        }

        if (allDestroyed)
        {
            ShowLoseScreen();
            return;
        }

        if (isIntermission)
        {
            intermissionTimer -= Time.deltaTime;

            if (intermissionText != null)
            {
                intermissionText.gameObject.SetActive(true);
                intermissionText.text = $"Intermission: {Mathf.CeilToInt(intermissionTimer)}s";
            }

            if (intermissionTimer <= 0f)
            {
                isIntermission = false;
                if (intermissionText != null) intermissionText.gameObject.SetActive(false);

                if (currentNight < maxNights)
                {
                    currentNight++;
                    StartNight(currentNight);
                }
                else
                {
                    ShowWinScreen(); // Normally never reached due to helicopter win condition
                }
            }

            return;
        }

        if (currentNight < maxNights && enemySpawner.TimerExpired() && enemySpawner.AllEnemiesDefeated)
        {
            isIntermission = true;
            intermissionTimer = intermissionDuration;

            if (intermissionText != null)
            {
                intermissionText.gameObject.SetActive(true);
                intermissionText.text = $"Intermission: {Mathf.CeilToInt(intermissionTimer)}s";
            }

            enemySpawner.StopSpawning();
            Debug.Log("Intermission started.");
            return;
        }

        HandleNightFade();
        HandleHelicopterMovement();
    }

    void StartNight(int nightNumber)
    {
        ApplyDifficultyScaling(nightNumber);
        enemySpawner.StartNewNight(nightNumber);

        TogglePlayerControl(false);

        if (nightStartAudioSource != null && nightStartClip != null)
        {
            nightStartAudioSource.clip = nightStartClip;
            nightStartAudioSource.Play();
        }

        if (backgroundFadeImage != null && nightText != null)
        {
            backgroundFadeImage.gameObject.SetActive(true);
            nightText.gameObject.SetActive(true);

            SetAlpha(backgroundFadeImage, 0.6f);
            SetAlpha(nightText, 0f);

            nightText.text = $"Night {nightNumber}";
            fadeTimer = 0f;
            fadeState = FadeState.FadeIn;
            fading = true;
        }

        if (nightNumber == maxNights && !finalWaveTriggered)
        {
            TriggerFinalWave();
        }
    }

    void TriggerFinalWave()
    {
        finalWaveTriggered = true;

        if (enemySpawner != null)
        {
            enemySpawner.spawnRateMultiplier *= finalWaveSpawnRateMultiplier;
            Debug.Log("🔥 Final wave triggered: Massive zombie spawn!");
        }

        if (helicopterPrefab != null && helicopterSpawnPoint != null)
        {
            activeHelicopter = Instantiate(helicopterPrefab, helicopterSpawnPoint.position, helicopterSpawnPoint.rotation);
            Debug.Log("🚁 Helicopter spawned and en route.");
        }

        if (intermissionText != null)
        {
            intermissionText.gameObject.SetActive(true);
            intermissionText.text = "Get to the helicopter for extraction!";
        }
    }

    void HandleHelicopterMovement()
    {
        if (activeHelicopter == null) return;

        float step = helicopterSpeed * Time.deltaTime;

        // ✅ First move to hover point
        if (!reachedHover && helicopterHoverPoint != null)
        {
            activeHelicopter.transform.position = Vector3.MoveTowards(
                activeHelicopter.transform.position,
                helicopterHoverPoint.position,
                step
            );

            if (Vector3.Distance(activeHelicopter.transform.position, helicopterHoverPoint.position) < 0.5f)
                reachedHover = true;
        }
        else if (helicopterLandingPoint != null)
        {
            // ✅ Then descend slowly to landing
            activeHelicopter.transform.position = Vector3.MoveTowards(
                activeHelicopter.transform.position,
                helicopterLandingPoint.position,
                step * 0.5f // slower descent
            );

            // ✅ Add subtle helicopter bob
            activeHelicopter.transform.position += new Vector3(
                Mathf.Sin(Time.time * 2f) * 0.05f, // left-right sway
                Mathf.Sin(Time.time * 3f) * 0.02f, // up-down bounce
                0
            );
        }
    }

    void HandleNightFade()
    {
        if (!fading) return;

        fadeTimer += Time.deltaTime;

        switch (fadeState)
        {
            case FadeState.FadeIn:
                float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                SetAlpha(backgroundFadeImage, Mathf.Lerp(0.6f, 0.9f, t));
                SetAlpha(nightText, Mathf.Lerp(0f, 1f, t));
                if (t >= 1f)
                {
                    fadeTimer = 0f;
                    fadeState = FadeState.Display;
                }
                break;
            case FadeState.Display:
                if (fadeTimer >= displayTime)
                {
                    fadeTimer = 0f;
                    fadeState = FadeState.FadeOut;
                }
                break;
            case FadeState.FadeOut:
                float tOut = Mathf.Clamp01(fadeTimer / fadeDuration);
                SetAlpha(backgroundFadeImage, Mathf.Lerp(0.6f, 0f, tOut));
                SetAlpha(nightText, Mathf.Lerp(1f, 0f, tOut));
                if (tOut >= 1f)
                {
                    backgroundFadeImage.gameObject.SetActive(false);
                    nightText.gameObject.SetActive(false);
                    fading = false;
                    fadeState = FadeState.None;

                    TogglePlayerControl(true);
                }
                break;
        }
    }

    void SetAlpha(Graphic graphic, float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    public void ShowWinScreen()
    {
        gameEnded = true;
        winScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ShowLoseScreen()
    {
        gameEnded = true;
        loseScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void TogglePlayerControl(bool enableMovement)
    {
        if (playerMovement != null)
            playerMovement.enabled = enableMovement;

        if (weapon != null)
            weapon.enabled = enableMovement;

        if (cameraLookScript != null)
            cameraLookScript.enabled = true;
    }

    void ApplyDifficultyScaling(int night)
    {
        float spawnRate = baseSpawnRateMultiplier + (night - 1) * spawnRateGrowthPerNight;
        float speedMult = baseEnemySpeedMultiplier + (night - 1) * speedGrowthPerNight;
        float damageMult = 1f + (night - 1) * 0.1f; // ✅ new damage scaling (10% per night)

        if (enemySpawner != null)
        {
            enemySpawner.spawnRateMultiplier = Mathf.Max(0.2f, spawnRate);
            enemySpawner.enemySpeedMultiplier = speedMult;
            enemySpawner.enemyDamageMultiplier = damageMult; // ✅ apply to spawner
        }

        Debug.Log($"[DIFFICULTY] Night {night} - Spawn Rate Mult: {spawnRate}, Enemy Speed Mult: {speedMult}, Enemy Damage Mult: {damageMult}");
    }
}
