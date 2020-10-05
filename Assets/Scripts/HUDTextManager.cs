using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDTextManager : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text recordText;

    // Update is called once per frame
    void Update()
    {
        this.scoreText.text = "$" + GameManager.instance.currentPlayerScore.ToString();
        this.recordText.text = "$" + GameManager.personalBestScore.ToString();
    }
}
