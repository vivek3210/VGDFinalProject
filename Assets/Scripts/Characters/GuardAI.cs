using UnityEngine;
using UnityEngine.AI;

public abstract class GuardAI : MonoBehaviour
{
    [Header("Base Components")]
    protected NavMeshAgent agent;
    protected Transform player;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    protected int currentPatrolIndex = 0;
    public float waitTimeAtPoint = 2f;
    private float waitCounter = 0f;

    [Header("Detection State")]
    public bool playerDetected = false;
    public float detectionCooldown = 3f;
    private float detectTimer = 0f;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    protected virtual void Update()
    {
        if (playerDetected)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            DetectPlayer(); // defined in child class
        }
    }

    /*** Core Guard Behaviors ***/

    protected virtual void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTimeAtPoint)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.destination = patrolPoints[currentPatrolIndex].position;
                waitCounter = 0f;
            }
        }
    }

    protected virtual void ChasePlayer()
    {
        if (player == null) return;
        agent.destination = player.position;

        detectTimer += Time.deltaTime;
        if (detectTimer >= detectionCooldown)
        {
            playerDetected = false;
            detectTimer = 0f;
        }
    }

    /*** To be Implemented by Child Classes ***/
    protected abstract void DetectPlayer();
}