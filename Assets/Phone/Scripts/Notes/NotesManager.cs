using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[System.Serializable]
public class Note
{
    public string name;
    [TextArea(3, 10)]
    public string description;
    public string boolKey; // The key used to check against BoolManager
    public bool isActive;
}

public class NotesManager : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab; // Reference to the note prefab
    [SerializeField] private Transform notesParent; // Parent transform to organize notes in the hierarchy
    [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect

    [SerializeField] private GameObject notificationPrefab; // Reference to the notification prefab
    [SerializeField] private Transform notificationParent; // Parent transform for notifications

    [SerializeField] private GameObject notesUI;

    [SerializeField] private List<Note> notes = new List<Note>(); // List of notes

    private float noteSpacing = 10f; // Spacing between notes

    private Dictionary<string, bool> instantiatedNotes = new Dictionary<string, bool>();

    public static NotesManager Instance { get; private set; }

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
        AddScrollbarListener(scrollRect);
        AddBackImageListener();
        // InstantiateNotes();
    }

    private void AddBackImageListener()
    {
        Image backImage = notesUI.GetComponentInChildren<Image>(true); // Search in all children
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
                StartCoroutine(ZoomOutAndDisableNotesUI());
            });

            // Add the entry to the EventTrigger events list
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Image not found in the chat instance prefab.");
        }
    }

    private IEnumerator ZoomOutAndDisableNotesUI()
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = notesUI.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Zoom out to zero size

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            notesUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        notesUI.transform.localScale = targetScale;

        // Disable the notesUI
        notesUI.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set notesUI back to its original scale
        notesUI.transform.localScale = originalScale;
    }

    public void InstantiateNotes()
    {
        foreach (Note note in notes)
        {
            if (BoolManager.Instance == null)
            {
                Debug.LogError("BoolManager.Instance is null.");
                return;
            }

            bool isBoolActive = BoolManager.Instance.GetBool(note.boolKey);
            Debug.Log($"Note: {note.name}, BoolKey: {note.boolKey}, isBoolActive: {isBoolActive}");

            if (isBoolActive && !IsNoteInstantiated(note.name))
            {
                CreateNote(note.description, note.name);
                ShowNoteNotification(note.description);
            }
        }
        UpdateContentSize();
    }

    public void CreateNote(string text, string noteKey)
    {
        GameObject noteInstance = Instantiate(notePrefab, notesParent);
        TextMeshProUGUI textComponent = noteInstance.GetComponentInChildren<TextMeshProUGUI>();

        if (textComponent != null)
        {
            textComponent.text = text;
            // Mark this note as instantiated
            instantiatedNotes[noteKey] = true;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the note prefab.");
        }
    }

    public void SetNoteBool(string noteName)
    {
        Note note = notes.Find(n => n.name == noteName);
        if (note != null)
        {
            note.isActive = true;
            ShowNoteNotification(note.description);
            // InstantiateNotes();
        }
        else
        {
            Debug.LogError("Invalid note name.");
        }
    }

    bool IsNoteInstantiated(string noteKey)
    {
        return instantiatedNotes.ContainsKey(noteKey) && instantiatedNotes[noteKey];
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

    private void UpdateContentSize()
    {
        if (notesParent != null)
        {
            // Calculate the total height of the content
            float totalHeight = 0f;
            foreach (RectTransform child in notesParent)
            {
                totalHeight += noteSpacing; // Add spacing between children
                totalHeight += child.rect.height; // Add height of each child
            }

            // Set the size of the content RectTransform
            RectTransform contentRectTransform = notesParent.GetComponent<RectTransform>();
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);

            // Scroll to the bottom
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        else
        {
            Debug.LogError("Notes Parent is not assigned.");
        }
    }

    public void ShowNoteNotification(string noteText)
    {
        GameObject notificationInstance = Instantiate(notificationPrefab, notificationParent);

        // Find the TextMeshPro component labeled "Last Note" and set the text
        TextMeshProUGUI[] textComponents = notificationInstance.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textComponent in textComponents)
        {
            if (textComponent.name == "Last Note")
            {
                textComponent.text = noteText;
                break;
            }
        }

        // Set notification position to the center of the parent
        if (notificationParent != null)
        {
            notificationInstance.transform.SetParent(notificationParent);
            notificationInstance.transform.localPosition = Vector3.zero;
        }

        // Ensure it takes the next available slot in the GridLayoutGroup
        GridLayoutGroup gridLayoutGroup = notificationParent.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            RectTransform rectTransform = notificationInstance.GetComponent<RectTransform>();
            rectTransform.SetParent(notificationParent);
            rectTransform.SetAsLastSibling(); // Move to the next available slot
        }

        PlayNotificationSound();

        // Start coroutine for fading in, staying, fading out, and destroying the notification
        StartCoroutine(FadeInOutAndDestroy(notificationInstance.GetComponent<CanvasGroup>()));
    }

    private void PlayNotificationSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(SFXContext.PhoneNotification);
        }
    }

    private IEnumerator FadeInOutAndDestroy(CanvasGroup canvasGroup)
    {
        float fadeDuration = 1.0f; // Adjust as needed
        float stayDuration = 2.0f; // Adjust as needed

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // Stay for a while
        yield return new WaitForSeconds(stayDuration);

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;

        Destroy(canvasGroup.gameObject);
    }
}