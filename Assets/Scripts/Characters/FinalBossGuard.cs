using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class FinalBossGuard : GuardAI
{
    [Header("Boss Movement")]
    public float bossMoveSpeed = 1.2f;
    public float bossAcceleration = 3f;

    [Header("Hearing Settings")]
    public float baseHearingRadius = 2.5f;
    public float minimumMovementToHear = 0.2f;

    [Header("Vision Settings")]
    public float viewRadius = 4.5f;
    [Range(0, 360)] public float viewAngle = 20f;
    public float centerVisionAngle = 8f;
    public LayerMask obstacleMask;
    public float eyeHeight = 1.2f;
    public float visibilityThreshold = 0.75f;

    [Header("Prediction Settings")]
    public float predictionRadius = 3.5f;
    public float interceptDistance = 0.8f;
    public float memoryInterval = 0.6f;
    public int maxStoredPositions = 4;
    public float minimumMovementForPrediction = 1.2f;
    public float minimumMoveAmount = 0.2f;
    public bool ignoreCrouchingForPrediction = true;

    [Header("Patrol Boundary")]
    public float maxDistanceFromPatrolPoint = 3f;

    [Header("Debug")]
    public bool debugBossGuard = false;

    private List<Vector3> playerPositionMemory = new List<Vector3>();
    private float memoryTimer = 0f;
    private Vector3 predictedPosition;
    private bool hasPrediction = false;

    protected override void Start()
    {
        base.Start();

        if (agent != null)
        {
            agent.speed = bossMoveSpeed;
            agent.acceleration = bossAcceleration;
        }
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

        bool heard = CheckHearing(pCtrl);
        bool seen = CheckVision(pCtrl);
        bool predicted = CheckPrediction(pCtrl);

        playerDetected = heard || seen || predicted;

        if (!predicted)
        {
            hasPrediction = false;
        }
    }

    bool CheckHearing(PlayerController pCtrl)
    {
        if (pCtrl.MoveAmount < minimumMovementToHear)
            return false;

        float dist = Vector3.Distance(transform.position, player.position);
        float effectiveRadius = baseHearingRadius * Mathf.Lerp(0.5f, 1.0f, pCtrl.noiseLevel);

        return dist <= effectiveRadius;
    }

    bool CheckVision(PlayerController pCtrl)
    {
        Vector3 eye = transform.position + Vector3.up * eyeHeight;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 dir = target - eye;
        float dist = dir.magnitude;

        if (dist > viewRadius)
            return false;

        Vector3 facing = -transform.forward;
        facing.y = 0f;
        facing.Normalize();

        Vector3 flatDir = new Vector3(dir.x, 0f, dir.z).normalized;
        float angle = Vector3.Angle(facing, flatDir);

        if (angle > centerVisionAngle * 0.5f)
            return false;

        bool blocked = Physics.Raycast(eye, dir.normalized, dist, obstacleMask);
        if (blocked)
            return false;

        return pCtrl.visibility >= visibilityThreshold;
    }

    bool CheckPrediction(PlayerController pCtrl)
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > predictionRadius)
            return false;

        if (pCtrl.MoveAmount < minimumMoveAmount)
            return false;

        if (ignoreCrouchingForPrediction && pCtrl.isCrouching)
            return false;

        RecordPosition();

        if (playerPositionMemory.Count < 2)
            return false;

        Vector3 start = playerPositionMemory[0];
        Vector3 end = playerPositionMemory[playerPositionMemory.Count - 1];
        Vector3 movement = end - start;

        if (movement.magnitude < minimumMovementForPrediction)
            return false;

        Vector3 dir = movement.normalized;
        Vector3 candidate = player.position + dir * interceptDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, 2f, NavMesh.AllAreas))
        {
            predictedPosition = hit.position;
            hasPrediction = true;
            return true;
        }

        return false;
    }

    void RecordPosition()
    {
        memoryTimer += Time.deltaTime;

        if (memoryTimer >= memoryInterval)
        {
            playerPositionMemory.Add(player.position);

            if (playerPositionMemory.Count > maxStoredPositions)
                playerPositionMemory.RemoveAt(0);

            memoryTimer = 0f;
        }
    }

    bool IsNearPatrolPoints(Vector3 point)
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return true;

        foreach (Transform p in patrolPoints)
        {
            if (p == null) continue;

            if (Vector3.Distance(point, p.position) <= maxDistanceFromPatrolPoint)
                return true;
        }

        return false;
    }

    Vector3 GetClosestPatrolPoint(Vector3 fromPosition)
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return transform.position;

        Transform closest = null;
        float bestDistance = float.MaxValue;

        foreach (Transform p in patrolPoints)
        {
            if (p == null) continue;

            float d = Vector3.Distance(fromPosition, p.position);
            if (d < bestDistance)
            {
                bestDistance = d;
                closest = p;
            }
        }

        return closest != null ? closest.position : transform.position;
    }

    protected override void ChasePlayer()
    {
        if (agent == null || !agent.isOnNavMesh || player == null)
            return;

        Vector3 target = hasPrediction ? predictedPosition : player.position;

        // If target is outside the boss zone, move to closest patrol point instead
        if (!IsNearPatrolPoints(target))
        {
            Vector3 fallback = GetClosestPatrolPoint(target);
            agent.SetDestination(fallback);

            if (debugBossGuard)
            {
                Debug.Log($"{name} target outside patrol zone, moving to nearest patrol point.");
            }

            return;
        }

        agent.SetDestination(target);
    }

    protected override void Patrol()
    {
        hasPrediction = false;
        base.Patrol();
    }
}