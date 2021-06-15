using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    UICanvasController healthBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        healthBar = FindObjectOfType<UICanvasController>();
        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int amountOfDamage)
    {
        currentHealth = -amountOfDamage;
        AudioManager.instance.PlayerSFX(5);
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            //the player can no longer move , the game stops
            gameObject.SetActive(false);
            AudioManager.instance.StopBackgroundMusic();
            AudioManager.instance.PlayerSFX(4);
            FindObjectOfType<GameManager>().PlayerRespawn();
        }
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }
}
