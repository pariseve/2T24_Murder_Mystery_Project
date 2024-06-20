using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotesManager : MonoBehaviour
{
    public GameObject notePrefab; // Reference to the note prefab
    public Transform notesParent; // Parent transform to organize notes in the hierarchy
    public ScrollRect scrollRect; // Reference to the ScrollRect

    public bool dream1brokenclock = false;
    public bool note2 = false;
    public bool note3 = false;

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

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            InstantiateNotes();
        }
        */
    }

    public void InstantiateNotes()
    {
        if (dream1brokenclock && !IsNoteInstantiated("dream1brokenclock"))
        {
            CreateNote("There was a broken clock in my dream with the time on 9:00... what could that mean?", "dream1brokenclock");
        }
        if (note2 && !IsNoteInstantiated("note2"))
        {
            CreateNote("This is the text for note 2", "note2");
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
                break;
            case "note2":
                note2 = true;
                break;
            case "note3":
                note3 = true;
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
}





