using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float damageInterval = 1f; // Time between damage ticks

    public Transform player;
    public Generator generator;

    private NavMeshAgent agent;
    private Enemy enemyScript;
    private PlayerStats playerStats;

    private float lastDamageTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyScript = GetComponent<Enemy>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (generator == null)
            generator = FindObjectOfType<Generator>();

        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (player == null || generator == null) return;

        bool playerInSight = PlayerInSight();

        if (playerInSight)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(generator.transform.position);
        }

        HandleAttack();
    }

    void HandleAttack()
    {
        if (playerStats == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            if (Time.time >= lastDamageTime + damageInterval)
            {
                playerStats.TakeDamage(damage);
                lastDamageTime = Time.time;
            }
        }
    }

    bool PlayerInSight()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRadius) return false;

        Ray ray = new Ray(transform.position + Vector3.up, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
}
