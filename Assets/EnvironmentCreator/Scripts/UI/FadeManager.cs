using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public Image fadeImage; // UI Image to cover the screen for fade effect

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeOut(Color fadeColor, float duration = 1f)
    {
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        fadeImage.gameObject.SetActive(true);

        for (float t = 0.01f; t < duration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, Mathf.Min(1, t / duration));
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration = 1f)
    {
        for (float t = 0.01f; t < duration; t += Time.deltaTime)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, Mathf.Max(0, 1 - (t / duration)));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
