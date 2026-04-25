using UnityEngine;

public class InvisibilityPowerup : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbilities abilities = other.GetComponent<PlayerAbilities>();

            if (abilities != null)
            {
                abilities.UnlockInvisibility();
            }

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f);
            }

            Destroy(gameObject);
        }
    }
}