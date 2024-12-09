using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class AdversarialAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public Rigidbody playerBody;

    public Ragdoll ragdoll;

    public GameObject FloatingTextPrefab;

    private Animator animation_controller;

    public GameObject smokeEffectPrefab;
    public Transform smokeSpawn;

    public AudioClip hitEffect;
    public AudioSource AudioSource;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        animation_controller = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        ragdoll = GameObject.FindWithTag("Player").GetComponent<Ragdoll>();
        AudioSource = GetComponent<AudioSource>();
    }

    //private void SearchWalkPoint()
    //{
    //    float randomZ = Random.Range(-walkPointRange, walkPointRange);
    //    float randomX = Random.Range(-walkPointRange, walkPointRange);

    //    walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

    //    if (Physics.Raycast(walkPoint, Vector3.down, whatIsGround))
    //    {
    //        walkPointSet = true;
    //    }
    //}

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


        //else
        //{
        //    walkPoint = Vector3.zero;
        //    walkPointSet = false;
        //}
    }


    private void Patroling() {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) {
            agent.SetDestination(walkPoint);
        }

        Vector3 distance = transform.position - walkPoint;

        if (distance.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void ChasePlayer() {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer() {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ShowFloatingText();
            AudioSource.PlayOneShot(hitEffect);
            alreadyAttacked = true;
            animation_controller.SetBool("attack", true);
            Vector3 attackDirection = (player.position - transform.position).normalized;
            ragdoll.RagDollModeOn();
            playerBody.AddForce(attackDirection * 50f, ForceMode.Impulse);

            GameObject smoke = Instantiate(smokeEffectPrefab, smokeSpawn.position, Quaternion.identity, transform);
            ParticleSystem particle = smoke.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                Destroy(smoke, particle.main.duration + particle.main.startLifetime.constantMax);
            }

            Debug.Log("ATTACK");
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        animation_controller.SetBool("attack", false);
        alreadyAttacked = false;
    }

    void ShowFloatingText() {
        Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (alreadyAttacked) {
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
