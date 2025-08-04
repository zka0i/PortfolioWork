using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Target References")]
    public Transform player;
    public Generator generator;

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float fieldOfViewAngle = 90f;

    private NavMeshAgent agent;
    private bool playerVisible;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (generator == null)
        {
            generator = FindObjectOfType<Generator>();
        }
    }

    void Update()
    {
        if (agent == null || player == null || generator == null) return;

        playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            Debug.Log($"{gameObject.name} ➤ Chasing PLAYER");
            agent.SetDestination(player.position);
            Debug.DrawLine(transform.position, player.position, Color.red);
        }
        else
        {
            Debug.Log($"{gameObject.name} ➤ Chasing GENERATOR");
            agent.SetDestination(generator.transform.position);
            Debug.DrawLine(transform.position, generator.transform.position, Color.green);
        }
    }

    bool IsPlayerVisible()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;

        if (dist > detectionRadius)
            return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer.normalized);
        if (angle > fieldOfViewAngle * 0.5f)
            return false;

        // ✅ Ignore "PlayerTrigger" layer in this raycast
        int layerMask = ~(1 << LayerMask.NameToLayer("PlayerTrigger"));

        Ray ray = new Ray(transform.position + Vector3.up, dirToPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius, layerMask, QueryTriggerInteraction.Collide))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
}
