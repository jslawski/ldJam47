using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private FadePanelManager fadePanel;

    [SerializeField]
    private GameObject tutorialBoi;

    [SerializeField]
    private GameObject pullTutorialImage;

    private Coroutine transitioningCoroutine;

    private void Awake()
    {
        this.fadePanel.FadeFromBlack();
        LassoTip.OnLatched += this.DisplayPullTutorialImage;
    }

    private void BeginTransitionToGame()
    {
       this.transitioningCoroutine = StartCoroutine(this.TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        yield return new WaitForSeconds(3.0f);
        this.fadePanel.OnFadeSequenceComplete += this.TransitionScenes;
        this.fadePanel.FadeToBlack();
    }

    private void TransitionScenes()
    {
        this.fadePanel.OnFadeSequenceComplete -= this.TransitionScenes;
        SceneManager.LoadScene(2);
    }

    private void Update()
    {
        if (this.tutorialBoi == null && this.transitioningCoroutine == null)
        {
            this.BeginTransitionToGame();
        }
    }

    private void DisplayPullTutorialImage(GameObject target)
    {
        LassoTip.OnLatched -= this.DisplayPullTutorialImage;
        this.pullTutorialImage.SetActive(true);
    }
}
