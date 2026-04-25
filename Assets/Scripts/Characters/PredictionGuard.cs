using UnityEngine;
using System.Collections.Generic;

public class PredictionGuard : GuardAI
{
    public float predictionRadius = 10f;
    public float interceptDistance = 4f;
    public float memoryInterval = .5f;
    public int maxStoredPositions = 6;
    public float repeatedPathRadius = 2f;
    public int repeatedPathThreshold = 3;
    public float losePlayerRadius = 15f;

    private List<Vector3> playerPositionMemory = new List<Vector3>();
    private float memoryTimer = 0f;
    private Vector3 predictedPosition;
    private bool hasPrediction = false;

    protected override void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        //keep recording player movement if close enough
        if (distance <= predictionRadius)
        {
            RecordPlayerPosition();
            CheckForPattern();
        }
        else
        {
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
                playerPositionMemory.RemoveAt(0);

            memoryTimer = 0f;
        }
    }

    void CheckForPattern()
    {
        if (playerPositionMemory.Count < 2) return;

        //estimate movement direction from oldest to newest remembered position
        Vector3 start = playerPositionMemory[0];
        Vector3 end = playerPositionMemory[playerPositionMemory.Count - 1];
        Vector3 rawDir = end - start;
        if (rawDir.magnitude < .1f) return;

        Vector3 moveDir = rawDir.normalized;

        predictedPosition = player.position + moveDir * interceptDistance;
        hasPrediction = true;

        int repeatCount = 0;
        Vector3 currentPos = player.position;

        foreach (Vector3 oldPos in playerPositionMemory)
        {
            if (Vector3.Distance(currentPos, oldPos) <= repeatedPathRadius)
                repeatCount++;
        }
        if (repeatCount >= repeatedPathThreshold)
        {
            playerDetected = true;
            detectTimer = 0f;

            //move toward predicted interception point instead of exact player location
            agent.destination = predictedPosition;
        }
    }

    protected override void ChasePlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (hasPrediction)
            agent.destination = predictedPosition;
        else
            agent.destination = player.position;

        if (distance > losePlayerRadius)
        {
            detectTimer += Time.deltaTime;
            if (detectTimer >= detectionCooldown)
            {
                playerDetected = false;
                detectTimer = 0f;
                hasPrediction = false;
                ReturnToPatrol();
            }
        }
        else
        {
            detectTimer = 0f;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, predictionRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, losePlayerRadius);
        if (hasPrediction)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(predictedPosition, .4f);
        }
    }
#endif
}

