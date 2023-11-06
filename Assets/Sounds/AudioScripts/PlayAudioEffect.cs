using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Impact System/Play Audio Effect", fileName = "PlayAudioEffect")]
public class PlayAudioEffect : ScriptableObject
{
    public AudioClip shotClip;
    public float volume = 1f;

    public void PlayShotClip(AudioSource audioSource)
    {
        audioSource.PlayOneShot(shotClip, volume);
    }
}
