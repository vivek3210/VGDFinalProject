using UnityEngine;

public class VisionGuard : GuardAI
{
    [Header("Vision Settings")]
    public float viewRadius = 20f;
    [Range(0, 360)] public float viewAngle = 120f;
    public LayerMask obstacleMask;
    public float eyeHeight = 1.2f;
    public float visibilityThreshold = 0.3f;

    protected override void DetectPlayer()
    {
        if (player == null) return;
        PlayerController pCtrl = player.GetComponent<PlayerController>();
        if (pCtrl == null) return;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 dirToPlayer = (player.position - eyePos).normalized;

        // Using -transform.forward if your model's Z axis is reversed
        Vector3 facing = -transform.forward;
        facing.y = 0f; facing.Normalize();

        Vector3 flatDir = new Vector3(dirToPlayer.x, 0f, dirToPlayer.z).normalized;
        float angle = Vector3.Angle(facing, flatDir);
        float distance = Vector3.Distance(eyePos, player.position);

        bool hasLineOfSight = !Physics.Raycast(eyePos, dirToPlayer, distance, obstacleMask);

        // Draw debug rays so you can visualize
        Debug.DrawRay(eyePos, dirToPlayer * viewRadius, Color.red);
        Debug.DrawRay(eyePos, facing * 2f, Color.blue);

        // If already chasing, keep verifying sight
        if (playerDetected)
        {
            if (!hasLineOfSight || distance > viewRadius)
            {
                detectTimer += Time.deltaTime;
                if (detectTimer >= detectionCooldown)
                {
                    playerDetected = false;
                    detectTimer = 0f;
                    Debug.Log($"{name} lost sight of player.");
                }
            }
            else
            {
                // still visible, reset timer
                detectTimer = 0f;
            }
            return; // stop here so we don’t instantly re‑trigger below
        }

        // Regular detection while patrolling
        if (distance <= viewRadius && angle < viewAngle / 2f && hasLineOfSight)
        {
            if (pCtrl.visibility >= visibilityThreshold)
            {
                playerDetected = true;
                detectTimer = 0f;
                Debug.Log($"{name} — FINAL DETECTION! Start chasing!");
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * -transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * -transform.forward;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * eyeHeight, left * viewRadius);
        Gizmos.DrawRay(transform.position + Vector3.up * eyeHeight, right * viewRadius);
    }
#endif
}