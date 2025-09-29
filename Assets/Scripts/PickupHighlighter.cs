using UnityEngine;

public class PickupHighlighter : MonoBehaviour
{
    [Header("Highlight Settings")]
    public float highlightRange = 3f;
    public Material highlightMaterial;   // 👈 Assign your glow material here
    private Material[] originalMaterials;

    private Transform player;
    private Renderer[] renderers;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Cache renderers + original materials
        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].material;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= highlightRange;

        foreach (Renderer rend in renderers)
        {
            if (inRange && highlightMaterial != null)
            {
                rend.material = highlightMaterial; // 👈 Swap to glow
            }
            else
            {
                int index = System.Array.IndexOf(renderers, rend);
                if (index >= 0 && index < originalMaterials.Length)
                    rend.material = originalMaterials[index]; // 👈 Restore original
            }
        }
    }
}
