using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource soundEffect;

    public void PlayButtonSound()
    {
        this.soundEffect.Play();
    }
}
