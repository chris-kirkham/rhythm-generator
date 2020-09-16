using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioFromRhythm : MonoBehaviour
{
    private AudioSource audioSource;

    public enum PlayMode { Monophonic, Polyphonic };
    public PlayMode playMode = PlayMode.Polyphonic;

    public void Init()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        if (playMode == PlayMode.Polyphonic)
        {
            audioSource.PlayOneShot(audioSource.clip); //PlayOneShot doesn't cut off currently-playing clip audio when playing a new clip
        }
        else
        {
            audioSource.Play(); //this cuts off last clip when playing a new one
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }


}
