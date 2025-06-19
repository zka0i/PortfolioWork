using UnityEngine;

public class GunScope : MonoBehaviour
{
    public Transform gunHolder;
    public Camera playerCamera;

    public float scopeFOV = 30f;
    public float normalFOV = 60f;
    public float scopeSpeed = 10f;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private WeaponScopedSettings currentWeaponSettings;

    void Start()
    {
        originalPos = gunHolder.localPosition;
        originalRot = gunHolder.localRotation;

        UpdateCurrentWeaponSettings();
    }

    void Update()
    {
        bool isScoping = Input.GetMouseButton(1); // hold right-click

        if (gunHolder.childCount == 0 || currentWeaponSettings == null)
            return;

        if (isScoping)
        {
            gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, currentWeaponSettings.scopedPosition, Time.deltaTime * scopeSpeed);
            gunHolder.localRotation = Quaternion.Lerp(gunHolder.localRotation, Quaternion.Euler(currentWeaponSettings.scopedRotation), Time.deltaTime * scopeSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, scopeFOV, Time.deltaTime * scopeSpeed);
        }
        else
        {
            gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, originalPos, Time.deltaTime * scopeSpeed);
            gunHolder.localRotation = Quaternion.Lerp(gunHolder.localRotation, originalRot, Time.deltaTime * scopeSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * scopeSpeed);
        }
    }

    public void UpdateCurrentWeaponSettings()
    {
        if (gunHolder.childCount > 0)
        {
            Transform activeWeapon = gunHolder.GetChild(0);
            currentWeaponSettings = activeWeapon.GetComponent<WeaponScopedSettings>();

            if (currentWeaponSettings == null)
            {
                Debug.LogWarning("❌ Missing WeaponScopedSettings on weapon: " + activeWeapon.name);
            }
        }
    }
}
