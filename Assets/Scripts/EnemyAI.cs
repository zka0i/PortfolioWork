using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;                // Assign in inspector or via spawner
    public Generator generator;             // Assign in inspector or via spawner

    private NavMeshAgent agent;

    [Header("Detection Settings")]
    public float detectionRadius = 20f;
    public float fieldOfView = 110f;

    private bool seesPlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Assign player if not already set
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"{gameObject.name} auto-assigned player: {player.name}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} COULD NOT FIND PLAYER");
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} already had player assigned: {player.name}");
        }

        // Assign generator if not already set
        if (generator == null)
        {
            generator = FindObjectOfType<Generator>();
            if (generator != null)
                Debug.Log($"{gameObject.name} auto-assigned generator: {generator.name}");
            else
                Debug.LogWarning($"{gameObject.name} COULD NOT FIND GENERATOR");
        }
        else
        {
            Debug.Log($"{gameObject.name} already had generator assigned: {generator.name}");
        }
    }

    void Update()
    {
        if (player == null || generator == null)
            return;

        float playerDist = Vector3.Distance(transform.position, player.position);
        float generatorDist = Vector3.Distance(transform.position, generator.transform.position);

        // Check line of sight to player
        seesPlayer = false;
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (playerDist <= detectionRadius && angle < fieldOfView * 0.5f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, out RaycastHit hit, detectionRadius))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    seesPlayer = true;
                }
            }
        }

        // Debug info
        Debug.Log($"{gameObject.name} ➤ PlayerDist: {playerDist:F1}, GenDist: {generatorDist:F1}, SeePlayer: {seesPlayer}");

        // Targeting logic
        if (seesPlayer || (playerDist < generatorDist && playerDist < detectionRadius))
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(generator.transform.position);
        }
    }
}
