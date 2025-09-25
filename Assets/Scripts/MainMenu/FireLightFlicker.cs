using UnityEngine;

[RequireComponent(typeof(Light))]
public class FireLightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minIntensity = 0.85f;
    public float maxIntensity = 1.35f;
    public float flickerSpeed = 5f; // how quickly it reacts to changes

    private Light fireLight;
    private float targetIntensity;

    void Start()
    {
        fireLight = GetComponent<Light>();
        targetIntensity = Random.Range(minIntensity, maxIntensity);
    }

    void Update()
    {
        // Smoothly move towards target intensity
        fireLight.intensity = Mathf.Lerp(fireLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);

        // If close enough to target, pick a new random intensity
        if (Mathf.Abs(fireLight.intensity - targetIntensity) < 0.05f)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);
        }
    }
}
