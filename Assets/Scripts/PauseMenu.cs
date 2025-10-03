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

    private bool isPaused;

    void Start()
    {
        // Disable all panels on start
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (controlsPanel) controlsPanel.SetActive(false);

        if (immersiveToggle)
            immersiveToggle.onValueChanged.AddListener(OnImmersiveToggle);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
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
        SceneManager.LoadScene("MainMenu"); // replace with your main menu scene name
    }

    private void OnImmersiveToggle(bool enabled)
    {
        Debug.Log("Immersive mode: " + enabled);
        // hide/show extra UI elements here if desired
    }
}
