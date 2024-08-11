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

        // Zoom factor (2x the size)
        float zoomFactor = 2f;
        Vector2 originalSize = rectTransform.sizeDelta;
        Vector2 targetSize = originalSize * zoomFactor;

        // Positions for corners
        Vector2[] cornerPositions = new Vector2[]
        {
            new Vector2(0.0f, 1.0f), // Top Left
            new Vector2(1.0f, 1.0f), // Top Right
            new Vector2(0.0f, 0.0f), // Bottom Left
            new Vector2(1.0f, 0.0f)  // Bottom Right
        };

        // Move to each corner and zoom
        for (int i = 0; i < cornerPositions.Length; i++)
        {
            rectTransform.pivot = cornerPositions[i];
            rectTransform.anchoredPosition = Vector2.zero; // Center it
            rectTransform.sizeDelta = targetSize; // Scale the image

            // Wait for 0.5 seconds at each corner
            yield return new WaitForSeconds(zoomDuration);

            // Return to original size and center
            rectTransform.sizeDelta = originalSize;
            rectTransform.pivot = new Vector2(0.5f, 0.5f); // Center pivot
            rectTransform.anchoredPosition = Vector2.zero;
        }

        // Optional: ensure the final state is centered
        rectTransform.sizeDelta = originalSize;
        rectTransform.pivot = new Vector2(0.5f, 0.5f); // Center pivot
        rectTransform.anchoredPosition = Vector2.zero;
    }
}

