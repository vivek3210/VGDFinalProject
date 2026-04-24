using UnityEngine;
using System.Collections;

public class GuardFreeze : MonoBehaviour
{
    public MonoBehaviour[] scriptsToDisable;
    private bool isFrozen = false;

    public void Freeze(float duration)
    {
        if (!isFrozen)
        {
            StartCoroutine(FreezeRoutine(duration));
        }
    }

    IEnumerator FreezeRoutine(float duration)
    {
        GuardAI guardBase = GetComponent<GuardAI>();
        if (guardBase != null)
        {
            guardBase.isFrozen = true;
        }

        isFrozen = true;
        Debug.Log(gameObject.name + " is frozen!");

        // Disable all assigned scripts
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        // Re-enable scripts
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }

        if (guardBase != null)
        {
            guardBase.isFrozen = false;
            guardBase.ForceReturnToPatrol();
        }

        Debug.Log(gameObject.name + " unfrozen!");
        isFrozen = false;
    }
}