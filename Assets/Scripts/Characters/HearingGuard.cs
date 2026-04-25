using UnityEngine;

public class HearingGuard : GuardAI
{
    [Header("Hearing Settings")]
    public float baseHearingRadius = 6f;
    public float minimumMovementToHear = 0.05f;
    public bool crouchingMakesPlayerUndetectable = true;
    public bool stoppingMovementCancelsChaseImmediately = true;

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

        // If player stops moving, hearing guard immediately gives up and patrols again
        if (pCtrl.MoveAmount < minimumMovementToHear)
        {
            playerDetected = false;

            if (stoppingMovementCancelsChaseImmediately)
            {
                ForceReturnToPatrol();
            }

            return;
        }

        // If crouching should fully avoid hearing detection
        if (crouchingMakesPlayerUndetectable && pCtrl.isCrouching)
        {
            playerDetected = false;
            ForceReturnToPatrol();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        float effectiveRadius = baseHearingRadius * Mathf.Lerp(0.5f, 1.5f, pCtrl.noiseLevel);

        if (distance <= effectiveRadius)
        {
            if (!playerDetected)
            {
                Debug.Log($"{name} (HearingGuard) heard the player!");
            }

            playerDetected = true;
        }
        else
        {
            playerDetected = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseHearingRadius);
    }
}