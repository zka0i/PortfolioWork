using UnityEngine;

public class GunScopeManager : MonoBehaviour
{
    [Header("References")]
    public Transform weaponParent; // Assign your GunHolder here
    public Camera playerCamera;
    public float scopedFOV = 40f;
    private float defaultFOV;

    private GunScope currentScope;

    void Start()
    {
        if (playerCamera != null)
            defaultFOV = playerCamera.fieldOfView;
    }

    void Update()
    {
        UpdateCurrentWeapon();

        if (currentScope != null)
        {
            bool holdingRightClick = Input.GetMouseButton(1);
            currentScope.isAiming = holdingRightClick;

            if (playerCamera != null)
            {
                playerCamera.fieldOfView = Mathf.Lerp(
                    playerCamera.fieldOfView,
                    holdingRightClick ? scopedFOV : defaultFOV,
                    Time.deltaTime * currentScope.aimSmoothing
                );
            }
        }
    }

    void UpdateCurrentWeapon()
    {
        if (weaponParent.childCount == 0)
        {
            currentScope = null;
            return;
        }

        Transform weapon = weaponParent.GetChild(0); // assume only one weapon at a time
        if (currentScope == null || currentScope.gameObject != weapon.gameObject)
        {
            currentScope = weapon.GetComponent<GunScope>();

            if (currentScope == null)
            {
                Debug.LogWarning("GunScopeManager: Gun has no GunScope component.");
            }
        }
    }
}
