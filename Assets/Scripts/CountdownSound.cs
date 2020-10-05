using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownSound : MonoBehaviour
{
    public void PlayStartSound()
    {
        VoiceLineManager.instance.PlayRandomStartLine();
    }


    public void DestroyCountdownObject()
    {
        Destroy(this.gameObject);
    }
}
