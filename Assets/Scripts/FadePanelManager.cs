using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanelManager : MonoBehaviour
{
    [SerializeField]
    private Image fadePanel;
    private float fadeSpeed = 3f;

    public delegate void FadeComplete();
    public event FadeComplete OnFadeSequenceComplete;

    public void FadeToBlack()
    {
        StopAllCoroutines();
        StartCoroutine(this.FadeToBlackCoroutine());
    }

    public void FadeFromBlack()
    {
        StopAllCoroutines();
        StartCoroutine(this.FadeFromBlackCoroutine());
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        while (fadePanel.color.a < 1 || Time.deltaTime == 0.0f)
        {
            Color updatedColor = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, this.fadePanel.color.a + (this.fadeSpeed * Time.fixedDeltaTime));
            fadePanel.color = updatedColor;
            //Debug.LogError("Adding this much alpha: " + (this.fadeSpeed * Time.deltaTime) + " for total of " + this.fadePanel.color.a);
            yield return null;
        }

        this.fadePanel.color = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, 1.0f);

        if (this.OnFadeSequenceComplete != null)
        {
            this.OnFadeSequenceComplete();
        }
    }

    private IEnumerator FadeFromBlackCoroutine()
    {
        while (fadePanel.color.a > 0 || Time.deltaTime == 0.0f)
        {
            Color updatedColor = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, this.fadePanel.color.a - (this.fadeSpeed * Time.fixedDeltaTime));
            fadePanel.color = updatedColor;
            //Debug.LogError("Removing this much alpha: " + (this.fadeSpeed * Time.fixedDeltaTime) + " for total of " + this.fadePanel.color.a);
            yield return null;
        }

        this.fadePanel.color = new Color(this.fadePanel.color.r, this.fadePanel.color.g, this.fadePanel.color.b, 0.0f);

        if (this.OnFadeSequenceComplete != null)
        {
            this.OnFadeSequenceComplete();
        }
    }
}
