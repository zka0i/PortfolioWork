using UnityEngine;

public class BarbedWirePickup : MonoBehaviour
{
    public int prefabIndex = 0;
    public int amount = 2;
    public GameObject highlightObject;
    public WeaponBuildTool buildTool;
    public float interactRange = 3f;

    [Header("Audio")]
    public AudioClip pickupSound;
    public float pickupVolume = 1f;

    private Camera cam;
    private Transform player;
    private AudioSource playerAudio;

    void Start()
    {
        cam = Camera.main;

        if (highlightObject != null)
            highlightObject.SetActive(false);

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
            playerAudio = player.GetComponent<AudioSource>();

        if (playerAudio == null && player != null)
            playerAudio = player.gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

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

                    // Play pickup sound from player's AudioSource
                    if (pickupSound != null && playerAudio != null)
                        playerAudio.PlayOneShot(pickupSound, pickupVolume);

                    Destroy(gameObject);
                }

                return;
            }
        }

        if (highlightObject != null)
            highlightObject.SetActive(false);
    }
}
