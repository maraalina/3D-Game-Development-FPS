using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(LevelExitDelay());
            Time.timeScale = 0.1f;
        }
    }

    IEnumerator LevelExitDelay()
    {
        yield return new WaitForSecondsRealtime(1f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1f;
    }
}
