using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Audio source for background music
    [SerializeField]
    private AudioSource bgAudioSource;

    // Audio source for audio effects
    [SerializeField]
    private AudioSource fxAudioSource;

    // Short sound effect
    public void PlayClip(AudioClip audioClip)
    {
        fxAudioSource.clip = audioClip;
        fxAudioSource.Play();
    }

    // Looped background music for menus and levels
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
