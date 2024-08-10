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

    [SerializeField] private bool loadedInputFields = false;

    private void Start()
    {
        UpdatePageVisibility();
        SetupPageButtons();
        LoadInputFields();
    }

    private void Update()
    {
        if (!loadedInputFields)
        {
            loadedInputFields = true;
            LoadInputFields();
        }
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

    public void CloseUI()
    {
        SaveInputFields();
        loadedInputFields = false;

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }

        StartCoroutine(AnimateUI(false));
        // gameObject.SetActive(false);
        // Destroy(gameObject); // Destroy the GameObject that this script is attached to
    }

    private void SaveInputFields()
    {
        foreach (var page in pages)
        {
            // Debug.Log($"Starting to save input fields for page: {page.name}");
            SaveInputFieldsInPage(page.transform);
        }
        PlayerPrefs.Save();
        // Debug.Log("Input fields saved.");
    }

    private void SaveInputFieldsInPage(Transform parent)
    {
        // Debug.Log($"Checking parent: {parent.name}");
        foreach (Transform child in parent)
        {
            // Debug.Log($"Checking child: {child.name}");

            TMP_InputField tmpInputField = child.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                string key = $"{tmpInputField.gameObject.name}_Text";
                PlayerPrefs.SetString(key, tmpInputField.text);
                // Debug.Log($"Saved: {key} = {tmpInputField.text}");
            }
            else
            {
                // Debug.Log($"No TMP_InputField found on: {child.name}");
            }

            // Recursively check all children
            SaveInputFieldsInPage(child);
        }
    }

    private void LoadInputFields()
    {
        // Debug.Log("Called load input fields");
        foreach (var page in pages)
        {
            // Debug.Log($"Starting to load input fields for page: {page.name}");
            LoadInputFieldsInPage(page.transform);
        }
    }

    private void LoadInputFieldsInPage(Transform parent)
    {
        // Debug.Log("called load input fields");
        foreach (Transform child in parent)
        {
            TMP_InputField tmpInputField = child.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                string key = $"{tmpInputField.gameObject.name}_Text";
                if (PlayerPrefs.HasKey(key))
                {
                    tmpInputField.text = PlayerPrefs.GetString(key);
                    // Debug.Log($"Loaded: {key} = {tmpInputField.text}");
                }
                else
                {
                    // Debug.Log($"Key not found: {key}");
                }
            }
            else
            {
                // Debug.Log($"No TMP_InputField found on: {child.name}");
            }

            // Recursively search in child objects
            LoadInputFieldsInPage(child);
        }
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