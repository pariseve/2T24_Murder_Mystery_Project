using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueImageChanger : MonoBehaviour
{
    [SerializeField] private CanvasGroup firstCanvasGroup;
    [SerializeField] private CanvasGroup secondCanvasGroup;
    [SerializeField] private Image secondImage;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (firstCanvasGroup == null || secondCanvasGroup == null || secondImage == null)
        {
            Debug.LogError("Please assign all CanvasGroups and Images in the Inspector.");
        }
    }

    public void FadeInFirstThenSecondWithSprite(Sprite newSprite)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInFirstThenSecondCoroutine(newSprite));
    }

    private IEnumerator FadeInFirstThenSecondCoroutine(Sprite newSprite)
    {
        yield return StartCoroutine(FadeCanvasGroup(firstCanvasGroup, 0, 1));
        secondImage.sprite = newSprite;
        yield return StartCoroutine(FadeCanvasGroup(secondCanvasGroup, 0, 1));
    }

    public void FadeOutSecondThenFirst()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutSecondThenFirstCoroutine());
    }

    private IEnumerator FadeOutSecondThenFirstCoroutine()
    {
        yield return StartCoroutine(FadeCanvasGroup(secondCanvasGroup, 1, 0));
        yield return StartCoroutine(FadeCanvasGroup(firstCanvasGroup, 1, 0));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}

