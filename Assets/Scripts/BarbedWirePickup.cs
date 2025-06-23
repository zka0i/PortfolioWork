using UnityEngine;

public class BarbedWirePickup : MonoBehaviour
{
    public int prefabIndex = 0;
    public int amount = 2;
    public GameObject highlightObject;
    public WeaponBuildTool buildTool;
    public float interactRange = 3f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (highlightObject != null)
            highlightObject.SetActive(false);
    }

    void Update()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (highlightObject != null)
                    highlightObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    buildTool.AddPlacement(prefabIndex, amount);
                    Destroy(gameObject);
                }

                return;
            }
        }

        if (highlightObject != null)
            highlightObject.SetActive(false);
    }
}
