using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    public static VoiceLineManager instance;

    private string directoryPath = "VoiceLines/";

    [SerializeField]
    private AudioSource voiceAudio;

    private AudioClip[] allWelcomeLines;
    private AudioClip[] allStartLines;
    private AudioClip[] allEndLines;

    private AudioClip tutorialLine;

    private void Awake()
    {
        instance = this;

        this.allWelcomeLines = Resources.LoadAll<AudioClip>(this.directoryPath + "Welcome");
        this.allStartLines = Resources.LoadAll<AudioClip>(this.directoryPath + "Start");
        this.allEndLines = Resources.LoadAll<AudioClip>(this.directoryPath + "End");
        this.tutorialLine = Resources.Load<AudioClip>(this.directoryPath + "js_round_reverb");

        DontDestroyOnLoad(this);
    }

    public void PlayRandomWelcomeLine()
    {
        int randomIndex = Random.Range(0, this.allWelcomeLines.Length);

        voiceAudio.clip = this.allWelcomeLines[randomIndex];
        voiceAudio.Play();
    }

    public void PlayRandomStartLine()
    {
        int randomIndex = Random.Range(0, this.allStartLines.Length);

        voiceAudio.clip = this.allStartLines[randomIndex];
        voiceAudio.Play();
    }

    public void PlayRandomEndLine()
    {
        int randomIndex = Random.Range(0, this.allEndLines.Length);

        voiceAudio.clip = this.allEndLines[randomIndex];
        voiceAudio.Play();
    }

    public void PlayTutorialLine()
    {
        voiceAudio.clip = this.tutorialLine;
        voiceAudio.Play();
    }
}
