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

    [SerializeField]
    private Text easyPinkieCount;

    [SerializeField]
    private Text beefcakeCount;

    [SerializeField]
    private Text goldenBoiCount;

    // Update is called once per frame
    void Awake()
    {
        this.scoreText.text = GameManager.instance.currentPlayerScore.ToString();
        this.recordText.text = GameManager.personalBestScore.ToString();
        this.easyPinkieCount.text = GameManager.instance.easyPinkieCount.ToString();
        this.beefcakeCount.text = GameManager.instance.beefcakeCount.ToString();
        this.goldenBoiCount.text = GameManager.instance.goldenBoiCount.ToString();
    }
}
