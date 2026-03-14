using UnityEngine;

public class VisionGuard : GuardAI
{
    [Header("Vision Settings")]
    public float viewRadius = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstacleMask;

    public float visibilityThreshold = 0.4f;

    protected override void DetectPlayer()
    {
        if (player == null) return;

        PlayerController pCtrl = player.GetComponent<PlayerController>();
        if (pCtrl == null) return;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewRadius)
        {
            float angle = Vector3.Angle(transform.forward, dirToPlayer);

            if (angle < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleMask))
                {
                    if (pCtrl.visibility >= visibilityThreshold)
                    {
                        playerDetected = true;
                        Debug.Log($"{name} (VisionGuard) saw the player!");
                    }
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