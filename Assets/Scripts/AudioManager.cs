using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource backgroundMusic;

    public AudioSource[] SFXs;

    //it stops a bit before Start()
    private void Awake()
    {
        instance = this;

    }

    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }

    public void PlayerSFX(int sfxNumber)
    {
        SFXs[sfxNumber].Stop();
        SFXs[sfxNumber].Play();
    }
}
