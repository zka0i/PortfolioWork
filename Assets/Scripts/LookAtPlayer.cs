using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void LateUpdate()
    {
        if (player == null)
            return;

        // Rotate toward the player
        transform.LookAt(player);

        // Flip 180° so text or UI faces correctly
        transform.Rotate(0, 180, 0);

        // Optional: keep upright (no tilting)
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
