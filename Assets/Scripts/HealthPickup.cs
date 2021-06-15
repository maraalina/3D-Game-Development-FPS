using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int amountOfHealing = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealthSystem>().HealPlayer(amountOfHealing);
            AudioManager.instance.PlayerSFX(2);
            Destroy(gameObject);
        }
    }
}
