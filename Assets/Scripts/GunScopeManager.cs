using UnityEngine;

public class GunScopeManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform weaponParent; // Your GunHolder — where weapons are cloned

    [Header("Scope Settings")]
    public float scopeFOV = 30f;
    public float normalFOV = 60f;
    public float transitionSpeed = 10f;

    private Transform currentWeapon;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private Vector3 scopedPos;
    private Vector3 scopedRot;

    void Update()
    {
        if (currentWeapon == null)
        {
            Debug.LogWarning("❌ No current weapon assigned.");
            return;
        }

        if (Input.GetMouseButton(1)) // Hold right-click
        {
            Debug.Log("🎯 Scoping in...");
            currentWeapon.localPosition = Vector3.Lerp(currentWeapon.localPosition, scopedPos, Time.deltaTime * transitionSpeed);
            currentWeapon.localRotation = Quaternion.Lerp(currentWeapon.localRotation, Quaternion.Euler(scopedRot), Time.deltaTime * transitionSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, scopeFOV, Time.deltaTime * transitionSpeed);
        }
        else
        {
            currentWeapon.localPosition = Vector3.Lerp(currentWeapon.localPosition, originalPos, Time.deltaTime * transitionSpeed);
            currentWeapon.localRotation = Quaternion.Lerp(currentWeapon.localRotation, originalRot, Time.deltaTime * transitionSpeed);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * transitionSpeed);
        }
    }

    public void UpdateCurrentWeapon()
    {
        if (weaponParent.childCount == 0)
        {
            Debug.LogWarning("🔶 GunScopeManager: No weapon found in weaponParent.");
            currentWeapon = null;
            return;
        }

        currentWeapon = weaponParent.GetChild(0);
        Debug.Log("✅ New weapon assigned: " + currentWeapon.name);

        originalPos = currentWeapon.localPosition;
        originalRot = currentWeapon.localRotation;

        WeaponScopedSettings settings = currentWeapon.GetComponent<WeaponScopedSettings>();
        if (settings != null)
        {
            scopedPos = settings.scopedPosition;
            scopedRot = settings.scopedRotation;
            Debug.Log("🔧 Scoped position: " + scopedPos + " | Scoped rotation: " + scopedRot);
        }
        else
        {
            Debug.LogWarning("⚠️ WeaponScopedSettings not found on weapon.");
        }
    }
}
