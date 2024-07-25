using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowImageUI : MonoBehaviour
{
    [SerializeField] private GameObject uiElement;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Vector3 smallScale = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector3 originalScale = new Vector3(1f, 1f, 1f);

    void Start()
    {
        // Set initial state
        uiElement.transform.localScale = smallScale;
        uiElement.SetActive(false);
    }

    public void EnableUI()
    {
        StartCoroutine(ScaleUI(uiElement, smallScale, originalScale, scaleDuration, true));
    }

    public void DisableUI()
    {
        StartCoroutine(ScaleUI(uiElement, originalScale, smallScale, scaleDuration, false));
    }

    private IEnumerator ScaleUI(GameObject ui, Vector3 fromScale, Vector3 toScale, float duration, bool setActive)
    {
        if (setActive)
        {
            ui.SetActive(true);
        }

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            ui.transform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ui.transform.localScale = toScale;

        if (!setActive)
        {
            ui.SetActive(false);
        }
    }
}
