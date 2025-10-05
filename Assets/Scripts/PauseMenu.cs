using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Optional UI")]
    public Toggle immersiveToggle;
    public Slider volumeSlider;

    [Header("UI to Hide in Immersive Mode")]
    public GameObject[] uiElementsToHide;

    private bool isPaused;
    private bool immersiveModeEnabled;

    void Start()
    {
        // Disable all panels on start
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);

        // Initialize Volume Slider
        if (volumeSlider)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // Setup Toggle
        if (immersiveToggle)
            immersiveToggle.onValueChanged.AddListener(OnImmersiveToggle);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 🔹 Handle Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (settingsPanel.activeSelf || controlsPanel.activeSelf)
                    OpenPauseMenu();
                else
                    ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // 🔹 Enforce hidden UI when immersive mode is ON
        if (immersiveModeEnabled)
        {
            foreach (var ui in uiElementsToHide)
            {
                if (ui != null && ui.activeSelf)
                    ui.SetActive(false);
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    private void OnImmersiveToggle(bool enabled)
    {
        immersiveModeEnabled = enabled;
        Debug.Log("Immersive mode: " + enabled);

        foreach (var ui in uiElementsToHide)
        {
            if (ui != null)
                ui.SetActive(!enabled);
        }
    }
}
