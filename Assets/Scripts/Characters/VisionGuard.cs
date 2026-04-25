using UnityEngine;

public class VisionGuard : GuardAI
{
    [Header("Vision Settings")]
    public float viewRadius = 1.5f;
    [Range(0, 360)] public float viewAngle = 20f;
    public LayerMask obstacleMask;
    public float eyeHeight = 1.2f;

    [Header("Detection Tuning")]
    public float centerVisionAngle = 10f;
    public float visibilityThreshold = 0.4f;

    protected override void DetectPlayer()
    {
        if (player == null)
        {
            playerDetected = false;
            return;
        }

        PlayerController pCtrl = player.GetComponent<PlayerController>();
        if (pCtrl == null)
        {
            playerDetected = false;
            return;
        }
        PlayerAbilities abilities = player.GetComponent<PlayerAbilities>();

        if (abilities != null && abilities.isInvisible)
        {
            playerDetected = false;

            if (agent != null)
            {
                agent.ResetPath();
            }

            return;
        }

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 playerTarget = player.position + Vector3.up * 1.0f;
        Vector3 toPlayer = playerTarget - eyePos;

        float distance = toPlayer.magnitude;
        if (distance > viewRadius)
        {
            playerDetected = false;
            return;
        }

        Vector3 dirToPlayer = toPlayer.normalized;

        // If your guard model faces forward normally, use transform.forward instead.
        Vector3 facing = transform.forward;
        facing.y = 0f;
        facing.Normalize();

        Vector3 flatDir = new Vector3(dirToPlayer.x, 0f, dirToPlayer.z).normalized;
        float angle = Vector3.Angle(facing, flatDir);

        if (angle > viewAngle * 0.5f)
        {
            playerDetected = false;
            return;
        }

        bool hasLineOfSight = !Physics.Raycast(eyePos, dirToPlayer, distance, obstacleMask);

        Debug.DrawRay(eyePos, dirToPlayer * distance, hasLineOfSight ? Color.green : Color.red);
        Debug.DrawRay(eyePos, facing * 2f, Color.blue);

        if (!hasLineOfSight)
        {
            playerDetected = false;
            return;
        }

        float requiredVisibility = visibilityThreshold;

        // Harder to detect on the outer edges of the cone
        if (angle > centerVisionAngle * 0.5f)
        {
            requiredVisibility += 0.2f;
        }

        if (pCtrl.visibility >= requiredVisibility)
        {
            if (!playerDetected)
            {
                Debug.Log($"{name} spotted the player!");
            }

            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * eyeHeight;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * -transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * -transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, left * viewRadius);
        Gizmos.DrawRay(origin, right * viewRadius);

        Vector3 leftCenter = Quaternion.Euler(0, -centerVisionAngle / 2f, 0) * -transform.forward;
        Vector3 rightCenter = Quaternion.Euler(0, centerVisionAngle / 2f, 0) * -transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(origin, leftCenter * viewRadius);
        Gizmos.DrawRay(origin, rightCenter * viewRadius);
    }
#endif
}