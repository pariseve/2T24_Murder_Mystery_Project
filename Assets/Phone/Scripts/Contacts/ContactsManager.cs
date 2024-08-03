using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ContactsManager : MonoBehaviour
{
    public static ContactsManager Instance { get; private set; }

    [SerializeField] private GameObject contactsUI;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RawImage[] contactImages; // Array of contact images
    [SerializeField] private GameObject[] contactDescriptions; // Array of contact descriptions

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AddBackImageListener();
        AddContactImageListeners();
        AddDescriptionBackButtonListeners();
        AddScrollbarListener(scrollRect);

        if (scrollRect != null)
        {
            StartCoroutine(SetScrollViewToTop());
        }
    }

    private IEnumerator SetScrollViewToTop()
    {
        yield return new WaitForEndOfFrame();

        // Set the verticalNormalizedPosition to 1 (top)
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DeactivateAllDescriptions();
        }
    }

    private void DeactivateAllDescriptions()
    {
        foreach (var contactDescriptions in contactDescriptions)
        {
            contactDescriptions.SetActive(false);
        }
    }

    private void AddBackImageListener()
    {
        Image backImage = contactsUI.GetComponentInChildren<Image>(true); // Search in all children
        if (backImage != null && backImage.name == "Back Button")
        {
            // Add an EventTrigger component if not already present
            EventTrigger trigger = backImage.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = backImage.gameObject.AddComponent<EventTrigger>();
            }

            // Create a new entry for Pointer Click event
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) =>
            {
                // Zoom out effect before disabling contactsUI
                StartCoroutine(ZoomOutAndDisableContactsUI());
            });

            // Add the entry to the EventTrigger events list
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Image not found in the chat instance prefab.");
        }
    }

    private void AddDescriptionBackButtonListeners()
    {
        for (int i = 0; i < contactDescriptions.Length; i++)
        {
            int index = i; // Capture the current index
            Image backImage = contactDescriptions[index].GetComponentInChildren<Image>(true);
            if (backImage != null && backImage.name == "Back Button")
            {
                // Add an EventTrigger component if not already present
                EventTrigger trigger = backImage.gameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = backImage.gameObject.AddComponent<EventTrigger>();
                }

                // Create a new entry for Pointer Click event
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) =>
                {
                    HideDescription(index);
                });

                // Add the entry to the EventTrigger events list
                trigger.triggers.Add(entry);
            }
            else
            {
                Debug.LogError($"Back Button not found in description {i}.");
            }
        }
    }

    private void AddContactImageListeners()
    {
        for (int i = 0; i < contactImages.Length; i++)
        {
            int index = i; // Capture the current index
            EventTrigger trigger = contactImages[index].gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = contactImages[index].gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) =>
            {
                ShowDescription(index);
            });

            trigger.triggers.Add(entry);
        }
    }

    private IEnumerator ZoomOutAndDisableContactsUI()
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = contactsUI.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Zoom out to zero size

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            contactsUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        contactsUI.transform.localScale = targetScale;

        // Disable the contactsUI
        contactsUI.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set contactsUI back to its original scale
        contactsUI.transform.localScale = originalScale;
    }

    private void ShowDescription(int index)
    {
        StartCoroutine(ZoomIn(contactDescriptions[index]));
        // contactsUI.SetActive(false);
    }

    private void HideDescription(int index)
    {
        StartCoroutine(ZoomOutAndDisableDescription(contactDescriptions[index]));
        contactsUI.SetActive(true);
    }

    private IEnumerator ZoomIn(GameObject description)
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = Vector3.zero; // Start from zero size
        Vector3 targetScale = Vector3.one; // Zoom in to full size

        description.SetActive(true);
        description.transform.localScale = originalScale;

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            description.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        description.transform.localScale = targetScale;
    }

    private IEnumerator ZoomOutAndDisableDescription(GameObject description)
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = description.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Zoom out to zero size

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            description.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        description.transform.localScale = targetScale;

        // Disable the contactsUI
        description.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set contactsUI back to its original scale
        description.transform.localScale = originalScale;
    }

    private void AddScrollbarListener(ScrollRect scrollRect)
    {
        if (scrollRect != null && scrollRect.verticalScrollbar != null)
        {
            // Add a listener to the vertical scrollbar's onValueChanged event
            scrollRect.verticalScrollbar.onValueChanged.AddListener((value) => { OnScrollbarValueChanged(value, scrollRect); });
        }
        else
        {
            Debug.LogError("Scrollbar or ScrollRect is null.");
        }
    }

    private void OnScrollbarValueChanged(float value, ScrollRect scrollRect)
    {
        // Set the vertical scrollbar's value directly
        scrollRect.verticalScrollbar.value = value;
    }
}
