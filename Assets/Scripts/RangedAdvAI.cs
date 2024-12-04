using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedAdvAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    
    public GameObject FloatingTextPrefab;
    public float projectileSpeed;

    private Animator animation_controller;

    public GameObject smokeEffectPrefab;
    public Transform smokeSpawn;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public GameObject applePrefab;
    public Rigidbody playerRB;

    public GameObject stash;
    bool reloading;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        animation_controller = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }


    private void SearchWalkPoint()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * walkPointRange;

        NavMeshQueryFilter filter = new NavMeshQueryFilter();
        filter.agentTypeID = agent.agentTypeID;
        filter.areaMask = agent.areaMask;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, walkPointRange, filter))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distance = transform.position - walkPoint;

        if (distance.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void GoToStash() {
        reloading = true;
        agent.SetDestination(stash.transform.position);

        StartCoroutine(ReloadingRoutine());
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, attackRange, whatIsPlayer)) {
                if (hit.transform.CompareTag("Player"))
                {
                    ShowFloatingText();
                    alreadyAttacked = true;
                    animation_controller.SetBool("attack", true);

                    GameObject projectile = Instantiate(applePrefab, gameObject.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                    projectile.GetComponent<Projectile>().Initialize(playerRB, projectileSpeed);

                    GameObject smoke = Instantiate(smokeEffectPrefab, smokeSpawn.position, Quaternion.identity, transform);
                    ParticleSystem particle = smoke.GetComponent<ParticleSystem>();
                    if (particle != null)
                    {
                        Destroy(smoke, particle.main.duration + particle.main.startLifetime.constantMax);
                    }

                    Debug.Log("RANGED ATTACK");
                    Invoke(nameof(GoToStash), 2);
                }
                else {
                    Debug.Log("Player not in sight");
                }
            }
        }
    }

    private void ResetAttack()
    {
        animation_controller.SetBool("attack", false);
        alreadyAttacked = false;
    }

    private IEnumerator ReloadingRoutine() {
        while (Vector3.Distance(transform.position, stash.transform.position) > 2f) {
            yield return null;
        }

        animation_controller.SetBool("run", false);
        animation_controller.SetBool("idle", true);

        yield return new WaitForSeconds(2);

        reloading = false;
        ResetAttack();
        Debug.Log("Reloading complete.");
    }

    void ShowFloatingText()
    {
        Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (reloading) {
            animation_controller.SetBool("run", true);
            animation_controller.SetBool("idle", false);
            animation_controller.SetBool("attack", false);
            return;
        }

        if (alreadyAttacked)
        {
            animation_controller.SetBool("attack", false);
            return;
        }

        if (!playerInSightRange && !playerInAttackRange)
        {
            animation_controller.SetBool("run", true);
            animation_controller.SetBool("idle", false);
            animation_controller.SetBool("attack", false);
            Patroling();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            animation_controller.SetBool("run", true);
            animation_controller.SetBool("idle", false);
            animation_controller.SetBool("attack", false);
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange)
        {
            animation_controller.SetBool("run", false);
            animation_controller.SetBool("idle", false);

            AttackPlayer();
        }
    }
}
