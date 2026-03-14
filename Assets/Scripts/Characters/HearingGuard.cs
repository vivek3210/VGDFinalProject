using UnityEngine;

public class HearingGuard : GuardAI
{
    [Header("Hearing Settings")]
    public float baseHearingRadius = 8f;

    protected override void DetectPlayer()
    {
        if (player == null) return;

        PlayerController pCtrl = player.GetComponent<PlayerController>();
        if (pCtrl == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        float effectiveRadius = baseHearingRadius * Mathf.Lerp(0.5f, 1.5f, pCtrl.noiseLevel);

        if (distance < effectiveRadius)
        {
            playerDetected = true;
            Debug.Log($"{name} (HearingGuard) heard the player!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseHearingRadius);
    }
}