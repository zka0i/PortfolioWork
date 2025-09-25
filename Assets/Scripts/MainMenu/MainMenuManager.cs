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
    public Transform mainCamera;                // the camera transform in scene
    public Transform mainCameraPosition;        // default menu view transform
    public Transform perksCameraPosition;       // perks view transform
    public Transform settingsCameraPosition;    // settings view transform

    [Header("Camera Motion")]
    public float cameraMoveSpeed = 2f;    // how fast underlying position/rotation lerps
    public float bobAmplitude = 0.15f;    // small so not dizzying
    public float bobFrequency = 0.6f;

    [Header("UI")]
    public Button readyButton;
    public Button perksButton;
    public Button settingsButton;
    public TextMeshProUGUI readyButtonText;   // shows "Ready" / "Unready"
    public TextMeshProUGUI loadingText;       // shows fake loading lines

    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Debug / Test")]
    public bool skipLoading = false; // if true, won't actually load scene (useful for testing)

    // Internal smoothing containers (prevents bob drift)
    private Vector3 smoothedPosition;
    private Quaternion smoothedRotation;

    // current target transform the camera should move toward
    private Transform currentTarget;

    private bool isReady = false;
    private bool isLoading = false;

    void Start()
    {
        // sanity checks
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

        // initialize smoothed states from actual camera transform
        smoothedPosition = mainCamera.position;
        smoothedRotation = mainCamera.rotation;

        // start target at main
        currentTarget = mainCameraPosition;

        // setup UI
        if (loadingText != null) loadingText.text = "";
        if (readyButtonText != null) readyButtonText.text = "Ready";

        if (readyButton != null) readyButton.onClick.AddListener(OnReadyClicked);
        if (perksButton != null) perksButton.onClick.AddListener(OnPerksClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    void Update()
    {
        if (mainCamera == null || currentTarget == null) return;

        // smoothly update underlying camera position & rotation (no bob applied here)
        smoothedPosition = Vector3.Lerp(smoothedPosition, currentTarget.position, Time.deltaTime * cameraMoveSpeed);
        smoothedRotation = Quaternion.Slerp(smoothedRotation, currentTarget.rotation, Time.deltaTime * cameraMoveSpeed);

        // compute bob offset ALWAYS (active at all times)
        Vector3 bobOffset = new Vector3(
            Mathf.Sin(Time.time * bobFrequency) * bobAmplitude,
            Mathf.Cos(Time.time * bobFrequency * 0.9f) * bobAmplitude * 0.6f,
            Mathf.Sin(Time.time * bobFrequency * 0.5f) * bobAmplitude * 0.15f
        );

        // apply: final camera = smoothed base + bob
        mainCamera.position = smoothedPosition + bobOffset;
        mainCamera.rotation = smoothedRotation;
    }

    // === READY / UNREADY logic ===
    void OnReadyClicked()
    {
        if (isLoading)
        {
            // Unready: cancel loading and return to main state
            CancelLoading();
            return;
        }

        if (!isReady)
        {
            StartCoroutine(ReadySequence());
        }
    }

    IEnumerator ReadySequence()
    {
        isReady = true;
        currentState = CameraState.Ready;
        currentTarget = mainCameraPosition;

        if (readyButtonText != null)
            readyButtonText.text = "Unready";

        // Wait until the camera underlying position has nearly reached the target so it feels deliberate
        yield return StartCoroutine(WaitForCameraToReachTarget(0.05f, 2.0f));

        // Start fake loading (non-blocking)
        isLoading = true;
        currentState = CameraState.Loading;
        StartCoroutine(FakeLoading());
    }

    // cancels coroutines related to loading and returns menu to main idle state
    void CancelLoading()
    {
        StopCoroutine(FakeLoading()); // best-effort stop
        StopAllCoroutines();          // safe clear of camera waiting coroutine(s)
        isLoading = false;
        isReady = false;
        currentState = CameraState.Main;
        currentTarget = mainCameraPosition;

        if (loadingText != null) loadingText.text = "";
        if (readyButtonText != null) readyButtonText.text = "Ready";

        // restore smoothedPosition/rotation base to current camera transform (prevents jumps)
        smoothedPosition = mainCamera.position;
        smoothedRotation = mainCamera.rotation;
    }

    // Wait until camera's smoothed position is close enough to current target or timeout
    IEnumerator WaitForCameraToReachTarget(float threshold = 0.05f, float timeoutSeconds = 3f)
    {
        float timer = 0f;
        while (Vector3.Distance(smoothedPosition, currentTarget.position) > threshold && timer < timeoutSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    // Fake loading that updates per-frame (no freeze), can be canceled by setting isLoading=false
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

        // step-by-step progress, increases over frames
        foreach (string step in steps)
        {
            int percent = 0;
            while (percent < 100)
            {
                if (!isLoading) yield break; // canceled by Unready

                // increment smoothly per frame
                percent += Random.Range(3, 9); // small jumps
                percent = Mathf.Min(percent, 100);

                if (loadingText != null)
                    loadingText.text = $"{step}... {percent}%";

                // wait a frame so the main thread stays responsive
                yield return null;
            }

            // ensure final 100% display for a short frame
            if (loadingText != null)
                loadingText.text = $"{step}... 100%";

            // brief pause to let user see 100%
            float smallPause = 0.2f;
            float t = 0f;
            while (t < smallPause)
            {
                if (!isLoading) yield break;
                t += Time.deltaTime;
                yield return null;
            }
        }

        // full loading finished
        if (isLoading)
        {
            if (skipLoading)
            {
                Debug.Log("[MainMenuManager] skipLoading is enabled — not actually loading scene.");
                // reset states so user can continue testing in editor
                isLoading = false;
                isReady = false;
                currentState = CameraState.Main;
                currentTarget = mainCameraPosition;
                if (loadingText != null) loadingText.text = "";
                if (readyButtonText != null) readyButtonText.text = "Ready";
            }
            else
            {
                // optional: fade screen here if desired (not implemented).
                SceneManager.LoadScene(gameSceneName);
            }
        }
    }

    // === PERKS and SETTINGS buttons ===
    void OnPerksClicked()
    {
        currentState = CameraState.Perks;
        if (perksCameraPosition != null) currentTarget = perksCameraPosition;
        // do not cancel loading/ready automatically — keep user control
    }

    void OnSettingsClicked()
    {
        currentState = CameraState.Settings;
        if (settingsCameraPosition != null) currentTarget = settingsCameraPosition;
    }

    // Clean up event listeners if needed
    private void OnDestroy()
    {
        if (readyButton != null) readyButton.onClick.RemoveListener(OnReadyClicked);
        if (perksButton != null) perksButton.onClick.RemoveListener(OnPerksClicked);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsClicked);
    }
}
