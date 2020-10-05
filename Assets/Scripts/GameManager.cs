using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Scoring
    public int currentPlayerScore = 0;
    public static int personalBestScore = 0;

    //Timer
    public Timer gameTimer;
    public float timePerRoundInSeconds = 10f;

    public GameObject gameOverPanel;

    public static bool gameFinished = false;

    public GameObject countdown;
    public bool preGameState = true;

    public FadePanelManager fadePanel;

    public AudioSource backgroundMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.currentPlayerScore = 0;
        gameFinished = false;

        this.fadePanel.FadeFromBlack();

        this.backgroundMusic = GameObject.Find("BGM").GetComponent<AudioSource>();
    }

    private void Start()
    {
        this.countdown = GameObject.Find("Countdown");
        StartCoroutine(this.WaitForCountdown());
    }

    private IEnumerator WaitForCountdown()
    {
        while (this.countdown != null)
        {
            yield return null;
        }

        gameTimer.StartTimer(timePerRoundInSeconds);
        gameTimer.OnTimerEnded -= this.GameCompleted;
        gameTimer.OnTimerEnded += this.GameCompleted;
        this.preGameState = false;
    }

    private void GameCompleted()
    {
        gameFinished = true;
        this.gameOverPanel.SetActive(true);
        gameTimer.OnTimerEnded -= this.GameCompleted;
    }

    public void IncrementScore(int scoreAmount)
    {
        this.currentPlayerScore += scoreAmount;
        if (this.currentPlayerScore > personalBestScore)
        {
            personalBestScore = currentPlayerScore;
        }
    }

    public void RestartGame()
    {
        this.fadePanel.OnFadeSequenceComplete += this.ReloadScene;
        this.fadePanel.FadeToBlack();
    }

    private void ReloadScene()
    {
        this.fadePanel.OnFadeSequenceComplete -= this.ReloadScene;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        this.fadePanel.OnFadeSequenceComplete += this.ReturnToTitle;
        this.fadePanel.FadeToBlack();
    }

    private void ReturnToTitle()
    {
        gameFinished = false;
        this.fadePanel.OnFadeSequenceComplete -= this.ReturnToTitle;
        this.backgroundMusic.Stop();
        Destroy(this.backgroundMusic.gameObject);
        SceneManager.LoadScene(0);
    }

    public float GetRandomSoundPitch()
    {
        return Random.Range(0.9f, 1.1f);
    }
}
