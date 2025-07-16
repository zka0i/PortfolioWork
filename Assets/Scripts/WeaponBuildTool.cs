using UnityEngine;
using System.Collections.Generic;

public class WeaponBuildTool : MonoBehaviour
{
    [Header("Build Settings")]
    public GameObject[] buildPrefabs;
    public float maxBuildDistance = 5f;
    public LayerMask groundLayer;

    [Header("Placement Audio")]
    public AudioClip[] placeSounds; // Match index with buildPrefabs
    public float placeVolume = 1f;
    private AudioSource audioSource;

    [Header("Hammer Hand")]
    public GameObject hammerHandPrefab;
    private GameObject hammerHandInstance;

    [Header("Weapon Manager")]
    public WeaponManager weaponManager;

    private int currentBuildIndex = 0;
    private GameObject currentPreview;
    private bool isBuilding = false;
    private Camera cam;

    private Dictionary<int, int> buildPlacements = new Dictionary<int, int>();

    void Start()
    {
        cam = Camera.main;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < buildPrefabs.Length; i++)
        {
            buildPlacements[i] = 0;
        }
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
            int previousIndex = currentBuildIndex;
            currentBuildIndex += scroll > 0 ? 1 : -1;
            currentBuildIndex = Mathf.Clamp(currentBuildIndex, 0, buildPrefabs.Length - 1);

            if (previousIndex != currentBuildIndex)
                SwitchPreview();
        }

        if (Input.GetMouseButtonDown(0) && buildPlacements[currentBuildIndex] > 0)
        {
            PlaceObject();
        }
    }

    void EnterBuildMode()
    {
        if (weaponManager != null && weaponManager.GetCurrentWeaponObject() != null)
        {
            weaponManager.GetCurrentWeaponObject().SetActive(false);
            weaponManager.DisableWeaponSwitching(true);
        }

        hammerHandInstance = Instantiate(hammerHandPrefab, weaponManager.weaponHolder);
        hammerHandInstance.transform.localPosition = Vector3.zero;
        hammerHandInstance.transform.localRotation = Quaternion.identity;

        if (buildPlacements[currentBuildIndex] > 0)
        {
            currentPreview = Instantiate(buildPrefabs[currentBuildIndex]);
            SetPreviewVisuals(true);
        }
    }

    void ExitBuildMode()
    {
        if (weaponManager != null && weaponManager.GetCurrentWeaponObject() != null)
        {
            weaponManager.GetCurrentWeaponObject().SetActive(true);
            weaponManager.GetCurrentWeapon().PlayEquipAnimation(); // ✅ NEW: Play equip anim on return!
            weaponManager.DisableWeaponSwitching(false);
        }

        if (hammerHandInstance != null)
            Destroy(hammerHandInstance);

        DestroyPreview();
    }

    void SwitchPreview()
    {
        DestroyPreview();

        if (buildPlacements[currentBuildIndex] > 0)
        {
            currentPreview = Instantiate(buildPrefabs[currentBuildIndex]);
            SetPreviewVisuals(true);
        }
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
        if (currentPreview == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxBuildDistance, groundLayer))
        {
            currentPreview.transform.position = hit.point;

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
        buildPlacements[currentBuildIndex]--;

        if (placeSounds != null && currentBuildIndex < placeSounds.Length && placeSounds[currentBuildIndex] != null)
        {
            audioSource.PlayOneShot(placeSounds[currentBuildIndex], placeVolume);
        }

        Debug.Log($"🔨 Placed: {buildPrefabs[currentBuildIndex].name}, Remaining: {buildPlacements[currentBuildIndex]}");

        if (buildPlacements[currentBuildIndex] <= 0)
        {
            DestroyPreview();
        }
    }

    public void AddPlacement(int prefabIndex, int amount)
    {
        if (prefabIndex < 0 || prefabIndex >= buildPrefabs.Length) return;
        buildPlacements[prefabIndex] += amount;
        Debug.Log($"✅ Picked up {amount}x {buildPrefabs[prefabIndex].name}");
    }

    public bool IsBuilding()
    {
        return isBuilding;
    }

    public GameObject GetCurrentWeaponObject()
    {
        return weaponManager != null ? weaponManager.GetCurrentWeaponObject() : null;
    }
}
