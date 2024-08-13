using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueImageChanger : MonoBehaviour
{
    [SerializeField] private CanvasGroup firstCanvasGroup;
    [SerializeField] private CanvasGroup secondCanvasGroup;
    [SerializeField] private Image secondImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float zoomDuration = 0.5f; // Duration for each corner zoom

    private Coroutine fadeCoroutine;
    private Coroutine zoomCoroutine;

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

    public void StartZoomAndMoveToCorners()
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }
        zoomCoroutine = StartCoroutine(ZoomAndMoveToCornersCoroutine());
    }

    private IEnumerator ZoomAndMoveToCornersCoroutine()
    {
        yield return new WaitForSeconds(1f);
        RectTransform rectTransform = secondImage.rectTransform;

        // Scale factor for zoom
        float zoomFactor = 2f;

        // Save the original size
        Vector2 originalSize = rectTransform.sizeDelta;

        // Positions for corners with pivot and anchor points
        Vector2[] cornerAnchors = new Vector2[]
        {
        new Vector2(0f, 1f), // Top Left
        new Vector2(1f, 1f), // Top Right
        new Vector2(0f, 0f), // Bottom Left
        new Vector2(1f, 0f)  // Bottom Right
        };

        for (int i = 0; i < cornerAnchors.Length; i++)
        {
            // Set the pivot to the corner
            rectTransform.pivot = cornerAnchors[i];

            // Set the anchor to the corner
            rectTransform.anchorMin = cornerAnchors[i];
            rectTransform.anchorMax = cornerAnchors[i];

            // Position the image at the corner
            rectTransform.anchoredPosition = Vector2.zero;

            // Scale the image to fill the screen
            rectTransform.sizeDelta = new Vector2(Screen.width * zoomFactor, Screen.height * zoomFactor);

            // Wait for the zoom duration before moving to the next corner
            yield return new WaitForSeconds(zoomDuration);
        }

        // Optional: ensure the final state is centered and reset the size
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = originalSize;
    }
}

