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
    [Header("Freeze State")]
    public bool isFrozen = false;
    public float detectionCooldown = 3f;
    protected float detectTimer = 0f;
    protected bool hasDetectedPlayer = false;
    [Header("Catch Settings")]
    public float catchDistance = 1.2f;
    private bool hasCaughtPlayer = false;

    [Header("Debug")]
    public bool debugPathStatus = false;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent == null)
        {
            Debug.LogError($"{name}: NavMeshAgent component is missing.");
            enabled = false;
            return;
        }

        if (player == null)
        {
            Debug.LogWarning($"{name}: Could not find player with tag 'Player'.");
        }

        if (patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[0] != null)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    protected virtual void Update()
    {
        if (isFrozen)
        {
            if (agent != null)
                agent.ResetPath();

            return;
        }
        PlayerAbilities abilities = player != null ? player.GetComponent<PlayerAbilities>() : null;

        if (abilities != null && abilities.isInvisible)
        {
            if (agent != null)
                agent.ResetPath();

            ForceReturnToPatrol();
            return;
        }
        DetectPlayer();

        if (playerDetected)
        {
            hasDetectedPlayer = true;
            detectTimer = 0f;
            ChasePlayer();
            TryCatchPlayer();

        }
        else if (hasDetectedPlayer)
        {
            detectTimer += Time.deltaTime;

            if (detectTimer < detectionCooldown)
            {
                ChasePlayer();
            }
            else
            {
                hasDetectedPlayer = false;
                detectTimer = 0f;
                ReturnToPatrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    protected virtual void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (patrolPoints[currentPatrolIndex] == null)
            return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            waitCounter += Time.deltaTime;

            if (waitCounter >= waitTimeAtPoint)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

                if (patrolPoints[currentPatrolIndex] != null)
                {
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }

                waitCounter = 0f;
            }
        }
    }

    protected virtual void ChasePlayer()
    {
        if (player == null)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        agent.SetDestination(player.position);
    }

    protected void ReturnToPatrol()
    {
        playerDetected = false;
        detectTimer = 0f;
        waitCounter = 0f;
        hasCaughtPlayer = false;

        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        if (patrolPoints[currentPatrolIndex] != null && agent != null && agent.isOnNavMesh)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public void ForceReturnToPatrol()
    {
        hasDetectedPlayer = false;
        ReturnToPatrol();
    }
    protected void TryCatchPlayer()
    {
        if (player == null || hasCaughtPlayer)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= catchDistance)
        {
            hasCaughtPlayer = true;
            GameManager.Instance.PlayerCaught();
        }
    }
    protected abstract void DetectPlayer();
}