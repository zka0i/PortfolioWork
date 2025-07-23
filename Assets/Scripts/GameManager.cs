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
    public Generator generator;
    public PlayerMovement playerMovement;
    public Weapon weapon;

    private bool gameEnded = false;
    private bool fading = false;
    private float fadeTimer = 0f;
    private enum FadeState { None, FadeIn, Display, FadeOut }
    private FadeState fadeState = FadeState.None;

    private int currentNight = 1;
    private const int maxNights = 12;

    private bool isIntermission = false;
    private float intermissionTimer = 0f;

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

        StartNight(currentNight);
    }

    void Update()
    {
        if (gameEnded) return;

        if (generator == null || generator.IsDestroyed)
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
                    ShowWinScreen();
                }
            }

            return;
        }

        if (enemySpawner.TimerExpired() && enemySpawner.AllEnemiesDefeated)
        {
            isIntermission = true;
            intermissionTimer = intermissionDuration;

            if (intermissionText != null)
            {
                intermissionText.gameObject.SetActive(true);
                intermissionText.text = $"Intermission: {Mathf.CeilToInt(intermissionTimer)}s";
            }

            Debug.Log("Intermission started.");
        }

        HandleNightFade();
    }

    void StartNight(int nightNumber)
    {
        enemySpawner.StartNewNight(nightNumber);

        TogglePlayerControl(false); // disable movement & weapon, allow look around

        // Play night start sound
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
    }

    void HandleNightFade()
    {
        if (!fading) return;

        fadeTimer += Time.deltaTime;

        switch (fadeState)
        {
            case FadeState.FadeIn:
                {
                    float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                    SetAlpha(backgroundFadeImage, Mathf.Lerp(0.6f, 0.9f, t));
                    SetAlpha(nightText, Mathf.Lerp(0f, 1f, t));

                    if (t >= 1f)
                    {
                        fadeTimer = 0f;
                        fadeState = FadeState.Display;
                    }
                    break;
                }
            case FadeState.Display:
                {
                    if (fadeTimer >= displayTime)
                    {
                        fadeTimer = 0f;
                        fadeState = FadeState.FadeOut;
                    }
                    break;
                }
            case FadeState.FadeOut:
                {
                    float t = Mathf.Clamp01(fadeTimer / fadeDuration);
                    SetAlpha(backgroundFadeImage, Mathf.Lerp(0.6f, 0f, t));
                    SetAlpha(nightText, Mathf.Lerp(1f, 0f, t));

                    if (t >= 1f)
                    {
                        backgroundFadeImage.gameObject.SetActive(false);
                        nightText.gameObject.SetActive(false);
                        fading = false;
                        fadeState = FadeState.None;

                        TogglePlayerControl(true); // re-enable movement and weapon
                    }
                    break;
                }
        }
    }

    void SetAlpha(Graphic graphic, float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    void ShowWinScreen()
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

        // Don't touch camera control – player can always look around
    }
}
