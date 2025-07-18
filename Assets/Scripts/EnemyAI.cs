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
    public GameObject destroyedGeneratorPrefab; // Prefab to spawn after destruction

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
            agent.stoppingDistance = attackRange;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.stoppingDistance = attackRange;

            float generatorDist = Vector3.Distance(transform.position, generator.transform.position);

            // Stop moving if close enough to prevent clipping
            if (generatorDist <= attackRange + 0.5f)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(generator.transform.position);
            }
        }

        HandleAttack();
    }

    void HandleAttack()
    {
        if (Time.time < lastDamageTime + damageInterval) return;

        float playerDist = Vector3.Distance(transform.position, player.position);
        float generatorDist = Vector3.Distance(transform.position, generator.transform.position);

        if (playerStats != null && playerDist <= attackRange && PlayerInSight())
        {
            playerStats.TakeDamage(damage);
            lastDamageTime = Time.time;
        }
        else if (generatorDist <= attackRange && !PlayerInSight())
        {
            generator.TakeDamage(damage);
            lastDamageTime = Time.time;

            if (generator.CurrentHealth <= 0 && destroyedGeneratorPrefab != null)
            {
                Instantiate(destroyedGeneratorPrefab, generator.transform.position, generator.transform.rotation);
                Destroy(generator.gameObject);
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
