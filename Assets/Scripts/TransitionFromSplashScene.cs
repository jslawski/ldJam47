using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionFromSplashScene : MonoBehaviour
{
    [SerializeField]
    private float secondsToWaitBeforeTransition = 3.0f;

    [SerializeField]
    private FadePanelManager fader;

    private void Awake()
    {
        this.fader.OnFadeSequenceComplete += this.DelayScreen;

        this.fader.FadeFromBlack();
    }

    private void DelayScreen()
    {
        this.fader.OnFadeSequenceComplete -= this.DelayScreen;
        StartCoroutine(this.TransitionAfterWait());
    }

    private IEnumerator TransitionAfterWait()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeTransition);
        this.fader.OnFadeSequenceComplete += this.LoadNextScene;
        this.fader.FadeToBlack();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}
