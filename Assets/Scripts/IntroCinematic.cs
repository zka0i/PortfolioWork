using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCinematic : MonoBehaviour
{
    [Header("Cinematic Settings")]
    public Animator cinematicCameraAnimator; // 🎥 The camera’s Animator with the intro animation
    public string animationTriggerName = "Play"; // Trigger name in Animator to start cinematic
    public float cinematicDuration = 50f; // ⏱️ Duration of the cinematic (in seconds)
    public string gameSceneName = "GameScene"; // 🎮 The name of your actual game scene to load

    [Header("Optional")]
    public AudioSource cinematicAudio; // Optional background audio or voice narration

    private bool hasStarted = false;

    void Start()
    {
        // Ensure everything else is disabled while cinematic plays
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start cinematic once
        if (cinematicCameraAnimator != null && !hasStarted)
        {
            hasStarted = true;
            cinematicCameraAnimator.SetTrigger(animationTriggerName);
            if (cinematicAudio != null) cinematicAudio.Play();
            Invoke(nameof(LoadGameScene), cinematicDuration);
        }
        else
        {
            Debug.LogWarning("⚠️ Missing Animator or already started cinematic!");
            Invoke(nameof(LoadGameScene), cinematicDuration);
        }
    }

    void LoadGameScene()
    {
        // Load your actual playable game scene
        SceneManager.LoadScene(gameSceneName);
    }
}
