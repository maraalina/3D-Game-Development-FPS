using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float timeUntilPlayerDies = 3f;

    public void PlayerRespawn()
    {
        StartCoroutine(PlayerRespawnCo());
    }

    IEnumerator PlayerRespawnCo()
    {
        yield return new WaitForSeconds(timeUntilPlayerDies);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
