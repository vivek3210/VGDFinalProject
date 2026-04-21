using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class PredictionGuard : GuardAI
{
    [Header("Prediction Settings")]
    public float predictionRadius = 8f;
    public float interceptDistance = 2.5f;
    public float memoryInterval = 0.4f;
    public int maxStoredPositions = 6;

    [Header("Pattern Settings")]
    public float minimumMovementForPrediction = 0.6f;
    public float minimumMoveAmount = 0.1f;
    public bool ignoreCrouchingPlayer = true;

    [Header("Patrol Boundary")]
    public bool stayNearPatrolZone = true;
    public float maxDistanceFromHome = 10f;

    [Header("Debug")]
    public bool debugPrediction = true;

    private List<Vector3> playerPositionMemory = new List<Vector3>();
    private float memoryTimer = 0f;
    private Vector3 predictedPosition;
    private bool hasPrediction = false;
    private Vector3 homePosition;

    protected override void Start()
    {
        base.Start();
        homePosition = transform.position;
    }

    protected override void DetectPlayer()
    {
        if (player == null)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        PlayerController pCtrl = player.GetComponent<PlayerController>();
        if (pCtrl == null)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only track nearby player
        if (distanceToPlayer > predictionRadius)
        {
            playerDetected = false;
            hasPrediction = false;
            playerPositionMemory.Clear();
            return;
        }

        // Ignore crouching if desired
        if (ignoreCrouchingPlayer && pCtrl.isCrouching)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        // Ignore if player is barely moving
        if (pCtrl.MoveAmount < minimumMoveAmount)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        RecordPlayerPosition();

        if (playerPositionMemory.Count < 2)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        Vector3 oldest = playerPositionMemory[0];
        Vector3 newest = playerPositionMemory[playerPositionMemory.Count - 1];
        Vector3 movement = newest - oldest;

        if (movement.magnitude < minimumMovementForPrediction)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        Vector3 moveDir = movement.normalized;
        Vector3 candidatePrediction = player.position + moveDir * interceptDistance;

        // Keep prediction near home zone
        if (stayNearPatrolZone && Vector3.Distance(homePosition, candidatePrediction) > maxDistanceFromHome)
        {
            playerDetected = false;
            hasPrediction = false;
            return;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidatePrediction, out hit, 2f, NavMesh.AllAreas))
        {
            predictedPosition = hit.position;
            hasPrediction = true;
            playerDetected = true;

            if (debugPrediction)
            {
                Debug.Log($"{name} is predicting player movement.");
            }
        }
        else
        {
            playerDetected = false;
            hasPrediction = false;
        }
    }

    void RecordPlayerPosition()
    {
        memoryTimer += Time.deltaTime;

        if (memoryTimer >= memoryInterval)
        {
            playerPositionMemory.Add(player.position);

            if (playerPositionMemory.Count > maxStoredPositions)
            {
                playerPositionMemory.RemoveAt(0);
            }

            memoryTimer = 0f;
        }
    }

    protected override void ChasePlayer()
    {
        if (agent == null || !agent.isOnNavMesh)
            return;

        if (hasPrediction)
        {
            agent.SetDestination(predictedPosition);
        }
        else
        {
            base.ChasePlayer();
        }
    }

    protected override void Patrol()
    {
        hasPrediction = false;
        base.Patrol();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, predictionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(homePosition == Vector3.zero ? transform.position : homePosition, maxDistanceFromHome);

        if (hasPrediction)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(predictedPosition, 0.35f);
            Gizmos.DrawLine(transform.position, predictedPosition);
        }
    }
#endif
}