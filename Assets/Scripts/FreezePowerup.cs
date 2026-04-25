using UnityEngine;

public class FreezePowerup : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbilities abilities = other.GetComponent<PlayerAbilities>();

            if (abilities != null)
            {
                abilities.UnlockFreeze();
            }

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f);
            }

            Destroy(gameObject);
        }
    }
}