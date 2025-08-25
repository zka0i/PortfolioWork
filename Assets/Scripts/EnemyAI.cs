using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Target References")]
    public Transform player;
    public Generator[] generators; // ✅ multiple generators

    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float fieldOfViewAngle = 90f;

    [Header("Debug")]
    public bool enableDebugLogs = false;

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

        if (generators == null || generators.Length == 0)
        {
            generators = FindObjectsOfType<Generator>();
        }
    }

    void Update()
    {
        if (agent == null || player == null) return;

        playerVisible = IsPlayerVisible();

        if (playerVisible)
        {
            if (enableDebugLogs)
                Debug.Log($"{gameObject.name} ➤ Chasing PLAYER");

            agent.SetDestination(player.position);

            if (enableDebugLogs)
                Debug.DrawLine(transform.position, player.position, Color.red);
        }
        else
        {
            Generator targetGen = GetNearestAliveGenerator();
            if (targetGen != null)
            {
                if (enableDebugLogs)
                    Debug.Log($"{gameObject.name} ➤ Chasing GENERATOR");

                agent.SetDestination(targetGen.transform.position);

                if (enableDebugLogs)
                    Debug.DrawLine(transform.position, targetGen.transform.position, Color.green);
            }
        }
    }

    Generator GetNearestAliveGenerator()
    {
        Generator nearest = null;
        float nearestDist = Mathf.Infinity;

        foreach (var gen in generators)
        {
            if (gen != null && !gen.IsDestroyed)
            {
                float dist = Vector3.Distance(transform.position, gen.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = gen;
                }
            }
        }

        return nearest;
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

        int layerMask = ~(1 << LayerMask.NameToLayer("PlayerTrigger"));

        Ray ray = new Ray(transform.position + Vector3.up, dirToPlayer.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius, layerMask, QueryTriggerInteraction.Collide))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
}
