using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text recordText;

    // Update is called once per frame
    void Update()
    {
        this.scoreText.text = "Score: " + GameManager.instance.currentPlayerScore.ToString();
        this.recordText.text = "Personal Best: " + GameManager.personalBestScore.ToString();
    }
}
