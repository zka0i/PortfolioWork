using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Immersive Option")]
    public Toggle immersiveToggle;
    public GameObject[] uiToHideInImmersive;

    [Header("Audio")]
    public AudioMixer masterMixer;

    private bool isPaused = false;

    void Start()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);

        if (immersiveToggle != null)
            immersiveToggle.onValueChanged.AddListener(OnImmersiveToggleChanged);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f; // freezes everything in the game
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        Time.timeScale = 1f; // unfreeze
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Called by buttons
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void CloseControls()
    {
        controlsPanel.SetActive(false);
    }

    public void MainMenu()
    {
        // You can load main menu scene here
        Debug.Log("Main Menu button clicked!");
    }

    void OnImmersiveToggleChanged(bool isOn)
    {
        foreach (GameObject uiElement in uiToHideInImmersive)
        {
            if (uiElement != null)
                uiElement.SetActive(!isOn);
        }
    }

    // Audio control example
    public void SetMasterVolume(float volume)
    {
        if (masterMixer != null)
            masterMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}
