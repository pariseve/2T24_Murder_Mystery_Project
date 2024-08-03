using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MapleMapsManager : MonoBehaviour
{
    public static MapleMapsManager Instance { get; private set; }

    [SerializeField] private GameObject mapleMapsUI;
    // [SerializeField] private ScrollRect scrollRect;

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
        // AddScrollbarListener(scrollRect);
        AddBackImageListener();
    }

    private void AddBackImageListener()
    {
        Image backImage = mapleMapsUI.GetComponentInChildren<Image>(true); // Search in all children
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
                // Zoom out effect before disabling notesUI
                StartCoroutine(ZoomOutAndDisableMapleMapsUI());
            });

            // Add the entry to the EventTrigger events list
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Image not found in the chat instance prefab.");
        }
    }

    private IEnumerator ZoomOutAndDisableMapleMapsUI()
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = mapleMapsUI.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Zoom out to zero size

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            mapleMapsUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        mapleMapsUI.transform.localScale = targetScale;

        // Disable the notesUI
        mapleMapsUI.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set notesUI back to its original scale
        mapleMapsUI.transform.localScale = originalScale;
    }

    /*private void AddScrollbarListener(ScrollRect scrollRect)
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
    }*/
}
