using UnityEngine;

public class InvisibilityPowerup : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbilities abilities = other.GetComponent<PlayerAbilities>();

            if (abilities != null)
            {
                abilities.UnlockInvisibility();
            }

            // 🔊 Play sound
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // ❗ Delay destroy so sound can play
            Destroy(gameObject, 0.5f);
        }
    }
}