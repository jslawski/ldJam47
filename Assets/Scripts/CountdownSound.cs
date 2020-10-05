using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownSound : MonoBehaviour
{
    public AudioSource sizzleSound;
    public AudioSource countdownSound;

    public void PlayStartSound()
    {
        VoiceLineManager.instance.PlayRandomStartLine();
    }

    public void PlaySizzleSound()
    {
        this.sizzleSound.Play();
    }

    public void PlayCountdownSound()
    {
        this.countdownSound.Play();
    }

    public void DestroyCountdownObject()
    {
        Destroy(this.gameObject);
    }
}
