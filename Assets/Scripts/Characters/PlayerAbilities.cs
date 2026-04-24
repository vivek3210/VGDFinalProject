using UnityEngine;
using System.Collections;

public class PlayerAbilities : MonoBehaviour
{
    public bool hasInvisibility = false;
    public bool isInvisible = false;

    public float invisibilityDuration = 5f;
    public bool hasFreeze = false;
    public float freezeDuration = 3f;
    public float freezeRange = 10f;

    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (hasInvisibility && !isInvisible && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(ActivateInvisibility());
        }
        if (hasFreeze && Input.GetKeyDown(KeyCode.F))
        {
            FreezeNearestGuard();
        }
    }

    public void UnlockInvisibility()
    {
        hasInvisibility = true;
        Debug.Log("Invisibility unlocked!");
    }

    IEnumerator ActivateInvisibility()
    {
        isInvisible = true;
        Debug.Log("Player is now invisible");

        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }

        yield return new WaitForSeconds(invisibilityDuration);

        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }

        isInvisible = false;
        Debug.Log("Player is visible again");
    }
    void FreezeNearestGuard()
    {
        GameObject[] guards = GameObject.FindGameObjectsWithTag("Guard");

        GameObject closestGuard = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject guard in guards)
        {
            float dist = Vector3.Distance(transform.position, guard.transform.position);

            if (dist < minDistance && dist <= freezeRange)
            {
                minDistance = dist;
                closestGuard = guard;
            }
        }

        if (closestGuard != null)
        {
            GuardFreeze freezeScript = closestGuard.GetComponent<GuardFreeze>();

            if (freezeScript != null)
            {
                freezeScript.Freeze(freezeDuration);
            }
        }
    }

    public bool IsInvisible()
    {
        return isInvisible;
    }
    public void UnlockFreeze()
    {
        hasFreeze = true;
        Debug.Log("Freeze ability unlocked!");
    }

}