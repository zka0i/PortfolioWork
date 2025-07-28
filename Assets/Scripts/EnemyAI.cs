using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 15f;
    public float attackRange = 2f;

    [Header("Attack Settings")]
    public float damage = 10f;
    public float damageInterval = 1f;

    [Header("References")]
    public Transform player;
    public Generator generator;
    public GameObject destroyedGeneratorPrefab;

    private NavMeshAgent agent;
    private Enemy enemyScript;
    private PlayerStats playerStats;

    private float lastDamageTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyScript = GetComponent<Enemy>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (generator == null)
            generator = FindObjectOfType<Generator>();

        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (enemyScript != null && enemyScript.IsDying()) return;
        if (player == null || generator == null) return;

        bool canSeePlayer = PlayerInSight();

        if (canSeePlayer)
        {
            agent.stoppingDistance = attackRange;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            float distToGen = Vector3.Distance(transform.position, generator.transform.position);
            agent.stoppingDistance = attackRange;

            if (distToGen <= attackRange + 0.5f)
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

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distToGen = Vector3.Distance(transform.position, generator.transform.position);

        if (playerStats != null && distToPlayer <= attackRange && PlayerInSight())
        {
            playerStats.TakeDamage(damage);
            lastDamageTime = Time.time;
            Debug.Log("⚔️ Enemy attacked PLAYER for " + damage);
        }
        else if (distToGen <= attackRange && !PlayerInSight())
        {
            generator.TakeDamage(damage);
            lastDamageTime = Time.time;
            Debug.Log("⚙️ Enemy attacked GENERATOR for " + damage);

            if (generator.CurrentHealth <= 0 && destroyedGeneratorPrefab != null)
            {
                Instantiate(destroyedGeneratorPrefab, generator.transform.position, generator.transform.rotation);
                Destroy(generator.gameObject);
                Debug.Log("💥 Generator destroyed and replaced with broken prefab.");
            }
        }
    }

    bool PlayerInSight()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRadius) return false;

        Ray ray = new Ray(transform.position + Vector3.up, (player.position - transform.position).normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, detectionRadius))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
}
