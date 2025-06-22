using UnityEngine;

public class WeaponBuildTool : MonoBehaviour
{
    [Header("Build Settings")]
    public GameObject[] buildPrefabs;
    public float maxBuildDistance = 5f;
    public LayerMask groundLayer;

    [Header("Hammer Hand")]
    public GameObject hammerHandPrefab; // Assign your hammer model
    private GameObject hammerHandInstance;

    [Header("Weapon Manager")]
    public WeaponManager weaponManager; // Assign reference in Inspector

    private int currentBuildIndex = 0;
    private GameObject currentPreview;
    private bool isBuilding = false;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isBuilding = !isBuilding;

            if (isBuilding)
                EnterBuildMode();
            else
                ExitBuildMode();
        }

        if (!isBuilding) return;

        UpdatePreviewPosition();

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentBuildIndex += scroll > 0 ? 1 : -1;
            currentBuildIndex = Mathf.Clamp(currentBuildIndex, 0, buildPrefabs.Length - 1);
            SwitchPreview();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }
    }

    void EnterBuildMode()
    {
        if (buildPrefabs.Length == 0) return;

        // Hide weapon
        if (weaponManager != null && weaponManager.GetCurrentWeaponObject() != null)
            weaponManager.GetCurrentWeaponObject().SetActive(false);

        // Spawn hammer hand
        hammerHandInstance = Instantiate(hammerHandPrefab, weaponManager.weaponHolder);
        hammerHandInstance.transform.localPosition = Vector3.zero;
        hammerHandInstance.transform.localRotation = Quaternion.identity;

        // Spawn first preview
        currentPreview = Instantiate(buildPrefabs[currentBuildIndex]);
        SetPreviewVisuals(true);
    }

    void ExitBuildMode()
    {
        // Restore weapon
        if (weaponManager != null && weaponManager.GetCurrentWeaponObject() != null)
            weaponManager.GetCurrentWeaponObject().SetActive(true);

        // Remove hammer hand
        if (hammerHandInstance != null)
            Destroy(hammerHandInstance);

        // Remove preview
        DestroyPreview();
    }

    void SwitchPreview()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(buildPrefabs[currentBuildIndex]);
        SetPreviewVisuals(true);
    }

    void DestroyPreview()
    {
        if (currentPreview != null)
            Destroy(currentPreview);
    }

    void SetPreviewVisuals(bool isGhost)
    {
        if (currentPreview == null) return;

        foreach (var r in currentPreview.GetComponentsInChildren<Renderer>())
        {
            Material mat = new Material(r.material);
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;
            r.material = mat;
        }

        Collider col = currentPreview.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    void UpdatePreviewPosition()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, groundLayer))
        {
            currentPreview.transform.position = hit.point;

            // Rotate to face the same direction as camera (ignoring pitch)
            Vector3 forward = cam.transform.forward;
            forward.y = 0f;
            if (forward != Vector3.zero)
                currentPreview.transform.rotation = Quaternion.LookRotation(forward);
        }
    }

    void PlaceObject()
    {
        if (currentPreview == null) return;

        Instantiate(buildPrefabs[currentBuildIndex], currentPreview.transform.position, currentPreview.transform.rotation);
        Debug.Log("🔨 Placed: " + buildPrefabs[currentBuildIndex].name);
    }
}
