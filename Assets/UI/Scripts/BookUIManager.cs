using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BookUIManager : MonoBehaviour
{
    [SerializeField] private GameObject parentObject;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Image[] pages; // Array of page images
    private int currentPageIndex = 0;

    private void Start()
    {
        UpdatePageVisibility();
        SetupPageButtons();
    }

    private void SetupPageButtons()
    {
        foreach (var page in pages)
        {
            // Find and setup Previous Button
            Button previousButton = page.transform.Find("Previous Button")?.GetComponent<Button>();
            if (previousButton != null)
            {
                previousButton.onClick.AddListener(GoToPreviousPage);
            }

            // Find and setup Next Button
            Button nextButton = page.transform.Find("Next Button")?.GetComponent<Button>();
            if (nextButton != null)
            {
                nextButton.onClick.AddListener(GoToNextPage);
            }

            // Find and setup Exit Button
            Button exitButton = page.transform.Find("ExitButton")?.GetComponent<Button>();
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(CloseUI);
            }
        }
    }

    private void GoToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility();
            UpdateButtonInteractability();
        }
    }

    private void GoToNextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
            UpdateButtonInteractability();
        }
    }

    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].gameObject.SetActive(i == currentPageIndex);
        }
    }

    private void UpdateButtonInteractability()
    {
        foreach (var page in pages)
        {
            Button previousButton = page.transform.Find("Previous Button")?.GetComponent<Button>();
            Button nextButton = page.transform.Find("Next Button")?.GetComponent<Button>();

            if (previousButton != null)
            {
                previousButton.interactable = currentPageIndex > 0;
            }

            if (nextButton != null)
            {
                nextButton.interactable = currentPageIndex < pages.Length - 1;
            }
        }
    }

    private void Update()
    {
        CheckAndUpdatePlayerMovement();
    }

    public void CloseUI()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }

        StartCoroutine(AnimateUI(false));
        // gameObject.SetActive(false);
        // Destroy(gameObject); // Destroy the GameObject that this script is attached to
    }

    public IEnumerator AnimateUI(bool enable)
    {
        if (enable)
        {
            parentObject.SetActive(true);
            LeanTween.scale(parentObject, Vector3.one, animationDuration).setEaseOutBounce();
        }
        else
        {
            LeanTween.scale(parentObject, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
            {
                parentObject.SetActive(false);
            });
            yield return new WaitForSeconds(animationDuration); // Wait for the animation to complete

            Destroy(gameObject); // Destroy after animation
        }
    }

    private void CheckAndUpdatePlayerMovement()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            if (IsAnyPageActive())
            {
                playerController.DisableMovement();
            }
            else
            {
                playerController.EnableMovement();
            }
        }
    }

    public bool IsAnyPageActive()
    {
        foreach (var page in pages)
        {
            if (page.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}