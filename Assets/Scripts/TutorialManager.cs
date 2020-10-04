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

    private Coroutine transitioningCoroutine;

    private void Awake()
    {
        this.fadePanel.FadeFromBlack();
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
        SceneManager.LoadScene(1);
    }

    private void Update()
    {
        if (this.tutorialBoi == null && this.transitioningCoroutine == null)
        {
            this.BeginTransitionToGame();
        }
        
        /*if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.fadePanel.FadeFromBlack();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.fadePanel.FadeToBlack();
        }*/
    }
}
