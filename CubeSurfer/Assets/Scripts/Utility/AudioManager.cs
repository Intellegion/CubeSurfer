using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource bgAudioSource;

    [SerializeField]
    private AudioSource fxAudioSource;

    private void Start()
    {
        
    }

    public void PlayClip(AudioClip audioClip)
    {
        fxAudioSource.clip = audioClip;
        fxAudioSource.Play();
    }

    public void PlayMusic(AudioClip audioClip)
    {
        bgAudioSource.clip = audioClip;
        bgAudioSource.Play();
    }

    public void StopMusic()
    {
        bgAudioSource.Stop();
    }

    public void StopSoundFX()
    {
        fxAudioSource.Stop();
    }
    
}
