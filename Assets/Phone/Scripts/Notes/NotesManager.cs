using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotesManager : MonoBehaviour
{
    [SerializeField] private GameObject notePrefab; // Reference to the note prefab
    [SerializeField] private Transform notesParent; // Parent transform to organize notes in the hierarchy
    [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect

    [SerializeField] private GameObject notificationPrefab; // Reference to the notification prefab
    [SerializeField] private Transform notificationParent; // Parent transform for notifications

    [SerializeField] private bool dream1brokenclock = false;
    [SerializeField] private bool dream1mirror = false;
    [SerializeField] private bool note3 = false;

    private float noteSpacing = 10f; // Spacing between notes

    private Dictionary<string, bool> instantiatedNotes = new Dictionary<string, bool>();

    public static NotesManager Instance { get; private set; }

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

    void Start()
    {
        AddScrollbarListener(scrollRect);
    }

    public void InstantiateNotes()
    {
        if (dream1brokenclock && !IsNoteInstantiated("dream1brokenclock"))
        {
            CreateNote("There was a broken clock in my dream near an empty coffee cup. What could that mean?", "dream1brokenclock");
        }
        if (dream1mirror && !IsNoteInstantiated("dream1mirror"))
        {
            CreateNote("There was a mirror in my dream that had a reflection of me with my hands covered in blood...", "dream1mirror");
        }
        if (note3 && !IsNoteInstantiated("note3"))
        {
            CreateNote("This is the text for note 3", "note3");
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
        switch (noteName)
        {
            case "dream1brokenclock":
                dream1brokenclock = true;
                ShowNoteNotification("There was a broken clock in my dream near an empty coffee cup. What could that mean?");
                break;
            case "dream1mirror":
                dream1mirror = true;
                ShowNoteNotification("There was a mirror in my dream that had a reflection of me with my hands covered in blood...");
                break;
            case "note3":
                note3 = true;
                ShowNoteNotification("This is the text for note 3");
                break;
            default:
                Debug.LogError("Invalid note name.");
                break;
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

        // Play notification audio if needed
        // PlayNotificationSound();

        // Start coroutine for fading in, staying, fading out, and destroying the notification
        StartCoroutine(FadeInOutAndDestroy(notificationInstance.GetComponent<CanvasGroup>()));
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







