using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Reminder
{
    public string name;
    public List<DescriptionPair> descriptionPairs = new List<DescriptionPair>();

    public Reminder()
    {
        descriptionPairs = new List<DescriptionPair>();
    }
}

[System.Serializable]
public class DescriptionPair
{
    public string boolKey;
    [TextArea(3, 10)]
    public List<string> descriptions = new List<string>();
}

public class RemindersManager : MonoBehaviour
{
    [SerializeField] private GameObject reminderPrefab;
    [SerializeField] private Transform remindersParent;
    [SerializeField] private GameObject reminderDescriptionPrefab;
    [SerializeField] private Transform reminderDescriptionParent;
    [SerializeField] private GameObject reminderDescriptionTextPrefab;
    [SerializeField] private GameObject remindersUI;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float remindersSpacing = 0.5f;

    [SerializeField] private GameObject notificationPrefab; // Reference to the notification prefab
    [SerializeField] private Transform notificationParent; // Parent transform for notifications

    private Dictionary<string, bool> instantiatedReminders = new Dictionary<string, bool>();
    private Dictionary<string, List<GameObject>> instantiatedReminderDescriptions = new Dictionary<string, List<GameObject>>();

    [SerializeField] private List<Reminder> reminders = new List<Reminder>();

    private Dictionary<string, HashSet<string>> shownDescriptions = new Dictionary<string, HashSet<string>>();

    public static RemindersManager Instance { get; private set; }

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
        // InstantiateReminders();
        AddBackImageListener();
        AddRemindersUIBackImageListener();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DeactivateAllReminderDescriptions();
        }

        // TESTING PURPOSES
        /*
        if (Input.GetKeyDown(KeyCode.E))
        {
            InstantiateReminders();
        }
        */

    }

    public void ShowRemindersNotification(Reminder reminder)
    {
        GameObject notificationInstance = Instantiate(notificationPrefab, notificationParent);

        // Find the TextMeshPro component labeled "Last Note" and set the text
        TextMeshProUGUI[] textComponents = notificationInstance.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var textComponent in textComponents)
        {
            if (textComponent.name == "Last Reminder")
            {
                textComponent.text = reminder.name;
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

        // Play notification audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPhoneNotification();
        }

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

    public void InstantiateReminders()
    {
        foreach (Reminder reminder in reminders)
        {
            if (BoolManager.Instance == null)
            {
                Debug.LogError("BoolManager.Instance is null.");
                return;
            }

            bool isReminderActive = false;

            foreach (DescriptionPair pair in reminder.descriptionPairs)
            {
                if (BoolManager.Instance.GetBool(pair.boolKey))
                {
                    isReminderActive = true;
                    break;
                }
            }

            if (isReminderActive && !instantiatedReminders.ContainsKey(reminder.name))
            {
                CreateReminder(reminder);
                instantiatedReminders[reminder.name] = true;
            }

            // Show notification for new content added to the reminder
            ShowNotificationForNewContent(reminder);
        }

        // After creating reminders, check if any need to be updated
        UpdateAllReminders();
    }

    private void ShowNotificationForNewContent(Reminder reminder)
    {
        if (!shownDescriptions.ContainsKey(reminder.name))
        {
            shownDescriptions[reminder.name] = new HashSet<string>();
        }

        foreach (DescriptionPair pair in reminder.descriptionPairs)
        {
            if (BoolManager.Instance.GetBool(pair.boolKey))
            {
                foreach (string description in pair.descriptions)
                {
                    if (!shownDescriptions[reminder.name].Contains(description))
                    {
                        ShowRemindersNotification(reminder);
                        shownDescriptions[reminder.name].Add(description);
                    }
                }
            }
        }
    }

    public void UpdateAllReminders()
    {
        foreach (Reminder reminder in reminders)
        {
            UpdateReminder(reminder);
        }

        UpdateContentSizeRemindersUI(); // Update UI after all reminders are processed
    }

    private GameObject FindReminderInstance(string reminderName)
    {
        foreach (Transform child in remindersParent)
        {
            if (child.name == reminderName + "_Reminder")
            {
                return child.gameObject;
            }
        }
        return null; // Return null if reminder instance is not found
    }

    public void UpdateReminder(Reminder reminder)
    {
        if (BoolManager.Instance == null)
        {
            Debug.LogError("BoolManager.Instance is null.");
            return;
        }

        GameObject reminderInstance = FindReminderInstance(reminder.name);
        if (reminderInstance == null)
        {
            Debug.LogError($"Reminder instance for {reminder.name} not found.");
            return;
        }

        Transform descriptionParent = DeepFind(reminderInstance.transform, "Description");
        if (descriptionParent != null)
        {
            TextMeshProUGUI descriptionText = descriptionParent.GetComponent<TextMeshProUGUI>();
            if (descriptionText != null)
            {
                bool allCompleted = true;
                foreach (DescriptionPair pair in reminder.descriptionPairs)
                {
                    if (!BoolManager.Instance.GetBool(pair.boolKey))
                    {
                        allCompleted = false;
                        break; // Exit early if any key is false
                    }
                }

                if (allCompleted)
                {
                    descriptionText.text = "Complete!";
                }
                else
                {
                    descriptionText.text = "Click for more information.";
                }
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the description parent.");
            }
        }
        else
        {
            Debug.LogError("Description parent not found in the reminder instance.");
        }
    }


    public void CreateReminder(Reminder reminder)
    {
        Debug.Log("running create reminders");
        GameObject reminderInstance = Instantiate(reminderPrefab, remindersParent);
        reminderInstance.name = reminder.name + "_Reminder";

        // Set reminder name text
        Transform nameTransform = DeepFind(reminderInstance.transform, "Name");
        if (nameTransform != null)
        {
            TextMeshProUGUI nameTextComponent = nameTransform.GetComponent<TextMeshProUGUI>();
            if (nameTextComponent != null)
            {
                nameTextComponent.text = reminder.name;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component for 'Name' not found in the reminder prefab.");
            }
        }
        else
        {
            Debug.LogError("'Name' child transform not found in the reminder prefab.");
        }

        // Find description parent
        Transform descriptionParent = DeepFind(reminderInstance.transform, "Description");
        if (descriptionParent != null)
        {
            TextMeshProUGUI descriptionText = descriptionParent.GetComponent<TextMeshProUGUI>();
            if (descriptionText != null)
            {
                // Set default text
                descriptionText.text = "Click for more information.";

                // Check if all bool keys associated with the reminder are true
                bool allCompleted = true;
                foreach (DescriptionPair pair in reminder.descriptionPairs)
                {
                    if (!BoolManager.Instance.GetBool(pair.boolKey))
                    {
                        Debug.Log("going through all pairs");
                        allCompleted = false;
                    }
                }

                // Update description text if all bool keys are true
                if (allCompleted)
                {
                    Debug.Log("quest complete");
                    descriptionText.text = "Complete!";
                }
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the description parent.");
            }
        }
        else
        {
            Debug.LogError("Description parent not found in the reminder instance.");
        }

        // Add pointer click event listener
        EventTrigger trigger = reminderInstance.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => OnReminderClicked(reminder));
        trigger.triggers.Add(entry);

        // Store instantiated reminder state
        instantiatedReminders[reminder.name] = true;
        // ShowRemindersNotification(reminder);
    }


    private void UpdateDescriptionTexts(GameObject descriptionInstance, Reminder reminder)
    {
        Transform contentParent = DeepFind(descriptionInstance.transform, "Content");
        if (contentParent != null)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            foreach (DescriptionPair pair in reminder.descriptionPairs)
            {
                if (BoolManager.Instance.GetBool(pair.boolKey))
                {
                    foreach (string descriptionText in pair.descriptions)
                    {
                        GameObject descriptionTextInstance = Instantiate(reminderDescriptionTextPrefab, contentParent);
                        TextMeshProUGUI descriptionTextComponent = descriptionTextInstance.GetComponentInChildren<TextMeshProUGUI>();
                        if (descriptionTextComponent != null)
                        {
                            descriptionTextComponent.text = descriptionText;
                        }
                        else
                        {
                            Debug.LogError("TextMeshProUGUI component for reminder description not found in the reminder description text prefab.");
                        }
                    }
                }
            }
            UpdateContentSize(contentParent);
        }
    }

    private void UpdateContentSize(Transform content)
    {
        float totalHeight = 0f; // Start with zero

        // Calculate total height including spacing between children
        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform child = content.GetChild(i) as RectTransform;
            if (child != null)
            {
                totalHeight += child.rect.height; // Add height of each child

                // Add spacing between children if not the last child
                if (i < content.childCount - 1)
                {
                    totalHeight += remindersSpacing;
                }
            }
        }

        RectTransform contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);
    }

    private void AddScrollbarListener(ScrollRect scrollRect)
    {
        if (scrollRect != null && scrollRect.verticalScrollbar != null)
        {
            scrollRect.verticalScrollbar.onValueChanged.AddListener((value) => { OnScrollbarValueChanged(value, scrollRect); });
        }
        else
        {
            Debug.LogError("Scrollbar or ScrollRect is null.");
        }
    }

    private void OnScrollbarValueChanged(float value, ScrollRect scrollRect)
    {
        scrollRect.verticalScrollbar.value = value;
    }

    private void UpdateContentSizeRemindersUI()
    {
        if (remindersParent != null)
        {
            float totalHeight = -remindersSpacing;
            foreach (RectTransform child in remindersParent)
            {
                totalHeight += remindersSpacing;
                totalHeight += child.rect.height;
            }

            RectTransform contentRectTransform = remindersParent.GetComponent<RectTransform>();
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
        else
        {
            Debug.LogError("Notes Parent is not assigned.");
        }
    }

    public void OnReminderClicked(Reminder reminder)
    {
        ShowReminderDescription(reminder);
    }

    public void ShowReminderDescription(Reminder reminder)
    {
        if (reminderDescriptionPrefab != null && reminderDescriptionParent != null)
        {
            Transform descriptionTransform = reminderDescriptionParent.Find(reminder.name + "_Description");

            if (descriptionTransform != null)
            {
                GameObject descriptionInstance = descriptionTransform.gameObject;
                descriptionInstance.SetActive(true);

                UpdateDescriptionTexts(descriptionInstance, reminder);

                AddBackButtonListenerToDescription(descriptionInstance);
/*
                VerticalLayoutGroup layoutGroup = descriptionTransform.GetComponentInChildren<VerticalLayoutGroup>();
                if (layoutGroup != null)
                {
                    layoutGroup.spacing = 10f;
                }
                UpdateContentSize(descriptionTransform, layoutGroup);
*/
                return;
            }

            GameObject newDescriptionInstance = Instantiate(reminderDescriptionPrefab, reminderDescriptionParent);
            newDescriptionInstance.name = reminder.name + "_Description";

            TextMeshProUGUI nameTextComponent = DeepFind(newDescriptionInstance.transform, "Name").GetComponent<TextMeshProUGUI>();
            if (nameTextComponent != null)
            {
                nameTextComponent.text = reminder.name;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component for reminder name not found in the reminder description prefab.");
            }

            UpdateDescriptionTexts(newDescriptionInstance, reminder);

            AddBackButtonListenerToDescription(newDescriptionInstance);
        }
    }

    private void AddBackImageListener()
    {
        Image backImage = remindersUI.GetComponentInChildren<Image>(true); // Search in all children
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
                // Perform the deactivation action
                DeactivateAllReminderDescriptions();

                // Activate the reminders UI
                remindersUI.SetActive(true);

            });

            // Add the entry to the EventTrigger events list
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Image not found in the reminders UI.");
        }
    }

    private void AddRemindersUIBackImageListener()
    {
        Image backImage = remindersUI.GetComponentInChildren<Image>(true); // Search in all children
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
                // Zoom out effect before disabling remindersUI
                StartCoroutine(ZoomOutAndDisableRemindersUI());
            });

            // Add the entry to the EventTrigger events list
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Image not found in the reminders UI.");
        }
    }

    private IEnumerator ZoomOutAndDisableRemindersUI()
    {
        float zoomDuration = 0.5f; // Adjust duration as needed
        Vector3 originalScale = remindersUI.transform.localScale;
        Vector3 targetScale = Vector3.zero; // Zoom out to zero size

        float elapsedTime = 0f;
        while (elapsedTime < zoomDuration)
        {
            remindersUI.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it ends at the target scale
        remindersUI.transform.localScale = targetScale;

        // Disable the remindersUI
        remindersUI.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set remindersUI back to its original scale
        remindersUI.transform.localScale = originalScale;

    }

    public void DeactivateAllReminderDescriptions()
    {
        foreach (Transform child in reminderDescriptionParent)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void AddBackButtonListenerToDescription(GameObject descriptionInstance)
    {
        Image backImage = descriptionInstance.GetComponentInChildren<Image>(true); // Search in all children
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
                // Deactivate this specific reminder description
                DeactivateReminderDescription(descriptionInstance);
            });

            // Add the entry to the EventTrigger events list
            trigger.triggers.Add(entry);
        }
        else
        {
            Debug.LogError("Back Image not found in the reminder description prefab.");
        }
    }

    private void DeactivateReminderDescription(GameObject descriptionInstance)
    {
        if (descriptionInstance != null)
        {
            // Find and remove the descriptionInstance from the dictionary
            foreach (var kvp in instantiatedReminderDescriptions)
            {
                if (kvp.Value.Contains(descriptionInstance))
                {
                    kvp.Value.Remove(descriptionInstance);
                    break; // Exit loop once found and removed
                }
            }

            // Deactivate the descriptionInstance
            descriptionInstance.SetActive(false);
        }
    }

    private Transform DeepFind(Transform parent, string name)
    {
        Transform result = parent.Find(name);
        if (result != null)
            return result;

        foreach (Transform child in parent)
        {
            result = DeepFind(child, name);
            if (result != null)
                return result;
        }

        return null;
    }
}