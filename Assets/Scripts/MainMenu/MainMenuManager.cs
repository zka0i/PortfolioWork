using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public enum CameraState { Main, Perks, Settings, Ready, Loading }
    private CameraState currentState = CameraState.Main;

    [Header("Camera Transforms")]
    public Transform mainCamera;
    public Transform mainCameraPosition;
    public Transform perksCameraPosition;
    public Transform settingsCameraPosition;

    [Header("Camera Motion")]
    public float cameraMoveSpeed = 2f;
    public float bobAmplitude = 0.15f;
    public float bobFrequency = 0.6f;

    [Header("UI")]
    public Button playButton;                   // replaces Ready button
    public Button perksButton;
    public Button settingsButton;
    public Button storyButton;                  // new: mission Story button
    public Button startButton;                  // new: mission Start button
    public GameObject missionsPanel;            // new: panel for missions
    public TextMeshProUGUI playButtonText;      // shows "Play" / "Back"
    public TextMeshProUGUI loadingText;

    [Header("Scene")]
    public string gameSceneName = "GameScene";
    public string cinematicSceneName = "Cinematic";

    [Header("Debug / Test")]
    public bool skipLoading = false;

    private Vector3 smoothedPosition;
    private Quaternion smoothedRotation;
    private Transform currentTarget;

    private bool isLoading = false;

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("[MainMenuManager] Main Camera is not assigned.");
            enabled = false;
            return;
        }

        if (mainCameraPosition == null)
        {
            Debug.LogError("[MainMenuManager] Main Camera Position is not assigned.");
            enabled = false;
            return;
        }

        smoothedPosition = mainCamera.position;
        smoothedRotation = mainCamera.rotation;
        currentTarget = mainCameraPosition;

        if (loadingText != null) loadingText.text = "";

        // Set default UI states
        if (playButtonText != null) playButtonText.text = "PLAY";
        if (missionsPanel != null) missionsPanel.SetActive(false);

        // Button events
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (perksButton != null) perksButton.onClick.AddListener(OnPerksClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
        if (storyButton != null) storyButton.onClick.AddListener(OnStoryClicked);
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
    }

    void Update()
    {
        if (mainCamera == null || currentTarget == null) return;

        smoothedPosition = Vector3.Lerp(smoothedPosition, currentTarget.position, Time.deltaTime * cameraMoveSpeed);
        smoothedRotation = Quaternion.Slerp(smoothedRotation, currentTarget.rotation, Time.deltaTime * cameraMoveSpeed);

        Vector3 bobOffset = new Vector3(
            Mathf.Sin(Time.time * bobFrequency) * bobAmplitude,
            Mathf.Cos(Time.time * bobFrequency * 0.9f) * bobAmplitude * 0.6f,
            Mathf.Sin(Time.time * bobFrequency * 0.5f) * bobAmplitude * 0.15f
        );

        mainCamera.position = smoothedPosition + bobOffset;
        mainCamera.rotation = smoothedRotation;
    }

    // === PLAY / MISSIONS logic ===
    void OnPlayClicked()
    {
        if (isLoading) return;

        if (missionsPanel != null)
        {
            bool showing = missionsPanel.activeSelf;
            missionsPanel.SetActive(!showing);

            if (playButtonText != null)
                playButtonText.text = showing ? "PLAY" : "BACK";
        }
    }

    void OnStoryClicked()
    {
        // Directly load the cinematic scene
        if (!isLoading)
        {
            StartCoroutine(StorySequence());
        }
    }

    IEnumerator StorySequence()
    {
        isLoading = true;
        currentState = CameraState.Loading;

        StartCoroutine(FakeLoading());

        // Wait until fake loading is complete
        while (isLoading)
        {
            yield return null;
        }

        // Load cinematic scene
        SceneManager.LoadScene(cinematicSceneName);
    }

    void OnStartClicked()
    {
        if (!isLoading)
        {
            StartCoroutine(ReadySequence()); // use the old ready logic
        }
    }

    IEnumerator ReadySequence()
    {
        isLoading = true;
        currentState = CameraState.Loading;
        if (missionsPanel != null) missionsPanel.SetActive(false);

        StartCoroutine(FakeLoading());
        yield break;
    }

    IEnumerator FakeLoading()
    {
        string[] steps = new string[]
        {
            "Loading map",
            "Loading assets",
            "Compiling shaders",
            "Optimizing textures",
            "Initializing systems",
            "Spawning world",
            "Finalizing setup"
        };

        if (loadingText != null) loadingText.text = "";

        foreach (string step in steps)
        {
            int percent = 0;
            while (percent < 100)
            {
                percent += Random.Range(3, 9);
                percent = Mathf.Min(percent, 100);

                if (loadingText != null)
                    loadingText.text = $"{step}... {percent}%";

                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        isLoading = false;

        if (skipLoading)
        {
            Debug.Log("[MainMenuManager] skipLoading enabled — staying in menu.");
            if (loadingText != null) loadingText.text = "";
        }
        else
        {
            // Only load game scene if Start button sequence
            if (currentState == CameraState.Loading && missionsPanel != null && !missionsPanel.activeSelf)
            {
                SceneManager.LoadScene(gameSceneName);
            }
        }
    }

    void OnPerksClicked()
    {
        currentState = CameraState.Perks;
        if (perksCameraPosition != null) currentTarget = perksCameraPosition;
    }

    void OnSettingsClicked()
    {
        currentState = CameraState.Settings;
        if (settingsCameraPosition != null) currentTarget = settingsCameraPosition;
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
        if (perksButton != null) perksButton.onClick.RemoveListener(OnPerksClicked);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
        if (storyButton != null) storyButton.onClick.RemoveListener(OnStoryClicked);
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
    }
}
