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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        this.currentPlayerScore = 0;
        gameFinished = false;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
