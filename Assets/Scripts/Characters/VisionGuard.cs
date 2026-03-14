using UnityEngine;

public class VisionGuard : GuardAI
{
    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public float eyeHeight = 0.8f; // Lower if player is short

    public float visibilityThreshold = 0.4f;

    protected override void DetectPlayer()
    {
        if (player == null) return;
        PlayerController pCtrl = player.GetComponent<PlayerController>();
        if (pCtrl == null) return;

        // Adjust eyes based on your model
        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;  // eyeHeight = adjustable variable

        // Target the player's collider center if available
        Vector3 targetPoint = player.position;
        Collider playerCol = player.GetComponent<Collider>();
        if (playerCol != null)
            targetPoint = playerCol.bounds.center;

        Vector3 dirToPlayer = (targetPoint - eyePos).normalized;
        float distance = Vector3.Distance(eyePos, targetPoint);
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        Debug.DrawRay(eyePos, dirToPlayer * viewRadius, Color.red);

        if (distance <= viewRadius && angle < viewAngle / 2f)
        {
            if (!Physics.Raycast(eyePos, dirToPlayer, distance, obstacleMask))
            {
                if (pCtrl.visibility >= visibilityThreshold)
                {
                    playerDetected = true;
                    Debug.Log($"{name} — FINAL DETECTION! Start chasing!");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}