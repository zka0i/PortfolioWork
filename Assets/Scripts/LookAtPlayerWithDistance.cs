using UnityEngine;

public class LookAtPlayerWithDistance : MonoBehaviour
{
    [Header("Settings")]
    public float showDistance = 5f;

    private Transform player;
    private Canvas canvas;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("No Canvas found on this GameObject.");
            return;
        }

        canvas.enabled = false; // start hidden
    }

    void LateUpdate()
    {
        if (player == null || canvas == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldShow = distance <= showDistance;

        // Only update if visibility changes
        if (canvas.enabled != shouldShow)
            canvas.enabled = shouldShow;

        if (shouldShow)
        {
            // Look at the player while keeping upright
            Vector3 targetPos = player.position;
            targetPos.y = transform.position.y; // keep UI upright
            transform.LookAt(targetPos);

            // Flip 180 so text faces the player correctly
            transform.Rotate(0, 180, 0);
        }
    }
}
