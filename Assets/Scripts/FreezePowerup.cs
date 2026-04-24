using UnityEngine;

public class FreezePowerup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAbilities abilities = other.GetComponent<PlayerAbilities>();

            if (abilities != null)
            {
                abilities.UnlockFreeze();
            }

            Destroy(gameObject);
        }
    }
}