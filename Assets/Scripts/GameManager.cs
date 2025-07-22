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

    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    public float displayTime = 2f;

    [Header("References")]
    public EnemySpawner enemySpawner;
    public Generator generator;

    private bool gameEnded = false;
    private bool fading = false;
    private float fadeTimer = 0f;
    private enum FadeState { None, FadeIn, Display, FadeOut }
    private FadeState fadeState = FadeState.None;

    private int currentNight = 1;
    private const int maxNights = 12;

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

        if (enemySpawner.TimerExpired() && enemySpawner.AllEnemiesDefeated)
        {
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

        HandleNightFade();
    }

    void StartNight(int nightNumber)
    {
        // Reset and spawn new wave
        enemySpawner.StartNewNight(nightNumber);

        // Show fade background and text
        if (backgroundFadeImage != null && nightText != null)
        {
            backgroundFadeImage.gameObject.SetActive(true);
            nightText.gameObject.SetActive(true);

            SetAlpha(backgroundFadeImage, 0f);
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
                    SetAlpha(backgroundFadeImage, Mathf.Lerp(0f, 0.9f, t));
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
                    SetAlpha(backgroundFadeImage, Mathf.Lerp(0.9f, 0f, t));
                    SetAlpha(nightText, Mathf.Lerp(1f, 0f, t));

                    if (t >= 1f)
                    {
                        backgroundFadeImage.gameObject.SetActive(false);
                        nightText.gameObject.SetActive(false);
                        fading = false;
                        fadeState = FadeState.None;
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
}
