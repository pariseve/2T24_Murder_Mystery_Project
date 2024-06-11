using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class MessageManager : MonoBehaviour
{
    public GameObject npcMessageWindowPrefab;
    public GameObject dialogueBoxPrefab;
    public GameObject contactPrefab;
    public Transform messageChatParent;
    public Transform contactParent;

    public GameObject messageNotificationPrefab; // Reference to the message notification prefab
    public Transform messageNotificationParent; // Parent transform for the message notifications

    private Dictionary<string, GameObject> messageChatInstances = new Dictionary<string, GameObject>();
    private Dictionary<string, List<string>> chatHistory = new Dictionary<string, List<string>>();

    private bool testContactCreated = false;
    private float contactSpacing = 0.5f; // Vertical spacing between contacts
    private float messageSpacing = 1.5f; // Vertical spacing between messages

    private bool isDraggingScrollbar = false;

    private static MessageManager instance;

    private Transform contactScrollViewContent; // Reference to the content area of the Contact ScrollView

    private Dictionary<string, GameObject> contactInstances = new Dictionary<string, GameObject>();

    // Ensure only one instance exists across scenes
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find the content area of the Contact ScrollView
        GameObject contactScrollView = GameObject.Find("Contact Scroll View");
        if (contactScrollView != null)
        {
            contactScrollViewContent = contactScrollView.transform.Find("Viewport/Content");
        }
        else
        {
            Debug.LogError("Contact Scroll View not found.");
        }
    }

    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!testContactCreated)
            {
                CreateTestContact();
                testContactCreated = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (testContactCreated)
            {
                SendMessageToTestContact("test");
            }
        }
        */

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DeactivateAllChatHistories();
        }
    }

    private void DeactivateAllChatHistories()
    {
        foreach (var chatInstance in messageChatInstances.Values)
        {
            chatInstance.SetActive(false);
        }
    }

    public void CreateMessageNotification(string npcName, string lastMessage)
    {
        // Instantiate the message notification prefab
        GameObject notification = Instantiate(messageNotificationPrefab, messageNotificationParent);

        // Set NPC name
        TextMeshProUGUI npcNameText = notification.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        if (npcNameText != null)
        {
            npcNameText.text = npcName;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component for NPC name not found in the message notification prefab.");
        }

        // Set last message
        TextMeshProUGUI lastMessageText = notification.transform.Find("Last Message").GetComponent<TextMeshProUGUI>();
        if (lastMessageText != null)
        {
            lastMessageText.text = lastMessage;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component for last message not found in the message notification prefab.");
        }

        // Set notification position to the center of the parent
        if (messageNotificationParent != null)
        {
            notification.transform.SetParent(messageNotificationParent);
            notification.transform.localPosition = Vector3.zero;
        }

        // Play notification audio
        PlayNotificationSound();

        // Start coroutine for fading in, staying, fading out, and destroying the notification
        StartCoroutine(FadeInOutAndDestroy(notification.GetComponent<CanvasGroup>()));
    }

    private IEnumerator FadeInOutAndDestroy(CanvasGroup canvasGroup)
    {
        float fadeSpeed = 1;
        // Fade in
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Destroy the notification object
        Destroy(canvasGroup.gameObject);
    }

    private void PlayNotificationSound()
    {
        // Implement audio playback logic here
    }

    public void InstantiateOrOpenMessageChat(string npcName)
    {
        if (messageChatInstances.ContainsKey(npcName))
        {
            OpenChatWithNPC(npcName);
        }
        else
        {
            InstantiateMessageChat(npcName);
            InstantiateContact(npcName, contactScrollViewContent);
        }
    }

    private void InstantiateMessageChat(string npcName)
    {
        // Instantiate the NPC message window prefab
        GameObject messageChatUI = Instantiate(npcMessageWindowPrefab, messageChatParent);

        // Find the TextMeshProUGUI component responsible for displaying the NPC name
        TextMeshProUGUI nameText = messageChatUI.GetComponentInChildren<TextMeshProUGUI>();

        // Check if the TextMeshProUGUI component is found
        if (nameText != null)
        {
            // Set the NPC name
            nameText.text = npcName;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component for NPC name not found in the message chat prefab.");
        }

        // Add the message chat instance to the dictionary
        messageChatInstances.Add(npcName, messageChatUI);
    }

    private void OpenChatWithNPC(string npcName)
    {
        // Check if there's an existing chat history content for the NPC
        var chatHistoryTuple = FindChatHistoryContent(npcName);

        if (chatHistoryTuple != null)
        {
            Transform existingChatHistory = chatHistoryTuple.Item1;
            ScrollRect scrollRect = chatHistoryTuple.Item2; // Change variable type to ScrollRect

            // Activate the existing chat history content
            existingChatHistory.gameObject.SetActive(true);

            // Initialize the scrollbar to the bottom
            if (scrollRect != null)
            {
                // Here you might want to access the verticalNormalizedPosition property to scroll to the bottom
                scrollRect.verticalNormalizedPosition = 0f;

                // Add scrollbar listener
                AddScrollbarListener(scrollRect);
            }
        }
        else
        {
            // Create a new chat history content for the NPC
            GameObject newChatHistory = Instantiate(npcMessageWindowPrefab, messageChatParent);
            newChatHistory.name = npcName + "_ChatHistory";

            // Find the 'Name' TextMeshProUGUI component in the new chat history content
            TextMeshProUGUI nameText = newChatHistory.GetComponentInChildren<TextMeshProUGUI>();

            // Check if the TextMeshProUGUI component is found
            if (nameText != null)
            {
                // Set the NPC name
                nameText.text = npcName;
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component for NPC name not found in the chat history prefab.");
            }

            // Find the Scrollbar within the new chat history content
            ScrollRect scrollRect = newChatHistory.GetComponentInChildren<ScrollRect>(); // Change to ScrollRect

            // Add the new chat history content to the dictionary for future reference
            messageChatInstances.Add(npcName, newChatHistory);

            // Display the new chat history content
            newChatHistory.gameObject.SetActive(true);

            // Initialize the scrollbar to the bottom
            if (scrollRect != null)
            {
                // Here you might want to access the verticalNormalizedPosition property to scroll to the bottom
                scrollRect.verticalNormalizedPosition = 0f;

                // Add scrollbar listener
                AddScrollbarListener(scrollRect);
            }
        }
    }

    private Tuple<Transform, ScrollRect> FindChatHistoryContent(string npcName)
    {
        // Check if a chat history content already exists for the NPC
        if (messageChatInstances.ContainsKey(npcName))
        {
            // Get the chat history content for the NPC
            GameObject messageChatObject = messageChatInstances[npcName];

            // Find the specific chat history object by name within the messageChatParent
            Transform chatHistoryObject = messageChatParent.Find(npcName + "_ChatHistory");

            // Check if the chat history object is found
            if (chatHistoryObject != null)
            {
                Transform chatHistoryTransform = chatHistoryObject.transform;
                // Find the ScrollRect within the chat history object
                ScrollRect scrollRect = chatHistoryObject.GetComponentInChildren<ScrollRect>();

                // Check if the ScrollRect is found
                if (scrollRect != null)
                {
                    return new Tuple<Transform, ScrollRect>(chatHistoryObject, scrollRect);
                }
                else
                {
                    Debug.LogError("ScrollRect not found in the chat history for NPC: " + npcName);
                }
            }
            else
            {
                Debug.LogError("Chat history object not found for NPC: " + npcName);
            }
        }

        return null;
    }

    // CONTACT STUFF

    public void ForInstantiateContact(string npcName)
    {
        InstantiateContact(npcName, contactScrollViewContent);
    }

    public void InstantiateContact(string npcName, Transform parentTransform)
    {
        // Instantiate the contact UI prefab
        GameObject contact = Instantiate(contactPrefab, parentTransform);

        // Set the name of the instantiated contact prefab
        contact.name = npcName + "_Contact";

        // Store reference to the instantiated contact prefab
        contactInstances.Add(npcName, contact);

        // Set NPC name text
        contact.GetComponentInChildren<TextMeshProUGUI>().text = npcName;

        // Add event trigger for left-clicking on the contact image
        EventTrigger trigger = contact.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => { OpenChatWithNPC(npcName); });

        trigger.triggers.Add(entry);

        // Set position based on the number of contacts already created
        RectTransform contactTransform = contact.GetComponent<RectTransform>();
        float yPos = -contactSpacing * (parentTransform.childCount - 1);
        contactTransform.localPosition = new Vector3(0f, yPos, 0f);

        // Add scrollbar listener to synchronize with main chat scrollbar
        ScrollRect contactScrollRect = parentTransform.GetComponent<ScrollRect>();
        if (contactScrollRect != null)
        {
            AddScrollbarListenerContact(contactScrollRect);
            contactScrollRect.verticalNormalizedPosition = 0f;
        }

        // Update content size after adding the contact
        UpdateContactListContentSize();
    }

    private void UpdateContactListContentSize()
    {
        if (contactScrollViewContent != null)
        {
            // Calculate the total height of the content
            float totalHeight = 0f;
            foreach (RectTransform child in contactScrollViewContent)
            {
                totalHeight += contactSpacing; // Add spacing between children
                totalHeight += child.rect.height; // Add height of each child
            }

            // Set the size of the content RectTransform
            RectTransform contentRectTransform = contactScrollViewContent.GetComponent<RectTransform>();
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);
        }
        else
        {
            Debug.LogError("Contact ScrollView content area not found.");
        }
    }

    public void UpdateLastMessageInContactPrefab(string npcName, string lastMessage)
    {
        // Check if the NPC chat history exists in the dictionary
        if (chatHistory.ContainsKey(npcName))
        {
            // Add the message to the existing chat history
            chatHistory[npcName].Add(lastMessage);
        }
        else
        {
            // Create a new chat history list for the NPC if it doesn't exist
            chatHistory[npcName] = new List<string>() { lastMessage };
        }

        // Check if the contact prefab instance exists in the dictionary
        if (contactInstances.ContainsKey(npcName))
        {
            // Retrieve the contact prefab instance
            GameObject contactPrefabInstance = contactInstances[npcName];

            // Find the "Last Message" object relative to the contact prefab
            Transform lastMessageTransform = contactPrefabInstance.transform.Find("Last Message");

            // Check if the "Last Message" object is found
            if (lastMessageTransform != null)
            {
                // Get the TextMeshProUGUI component from the "Last Message" object
                TextMeshProUGUI lastMessageText = lastMessageTransform.GetComponent<TextMeshProUGUI>();

                // Check if the TextMeshProUGUI component is found
                if (lastMessageText != null)
                {
                    // Update the last message text
                    lastMessageText.text = lastMessage;
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found in the 'Last Message' object of the contact prefab: " + npcName);
                }
            }
            else
            {
                Debug.LogError("'Last Message' object not found in the contact prefab: " + npcName);
            }
        }
        else
        {
            Debug.LogError("Contact prefab not found for NPC: " + npcName);
        }
    }

    private void AddScrollbarListenerContact(ScrollRect contactScrollRect)
    {
        if (contactScrollRect != null && contactScrollRect.verticalScrollbar != null)
        {
            // Add a listener to the vertical scrollbar's onValueChanged event
            contactScrollRect.verticalScrollbar.onValueChanged.AddListener((value) => { OnScrollbarValueChanged(value, contactScrollRect); });
        }
        else
        {
            Debug.LogError("Contact ScrollView's scrollbar or ScrollRect is null.");
        }
    }

    // CHAT HISTORY STUFF

    private void UpdateScrollbarPosition(ScrollRect scrollRect)
    {
        // Ensure the scroll view scrolls to the bottom
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void UpdateContentSize(Transform content, VerticalLayoutGroup layoutGroup)
    {
        // Calculate the total height of the content
        float totalHeight = 0f;
        foreach (RectTransform child in content)
        {
            totalHeight += layoutGroup.spacing; // Add spacing between children
            totalHeight += child.rect.height; // Add height of each child
        }

        // Set the size of the content RectTransform
        RectTransform contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);
    }


    public void AddMessageToHistory(string npcName, string message)
    {
        // Check if the NPC chat history exists in the dictionary
        if (chatHistory.ContainsKey(npcName))
        {
            // Add the message to the existing chat history
            chatHistory[npcName].Add(message);
        }
        else
        {
            // Create a new chat history list for the NPC if it doesn't exist
            chatHistory[npcName] = new List<string>() { message };
        }

        // Find the chat history content for the NPC
        var chatHistoryTuple = FindChatHistoryContent(npcName);
        // GameObject parentObject = GameObject.Find("ParentObject");

        if (chatHistoryTuple != null)
        {
            Transform chatHistoryContent = chatHistoryTuple.Item1;
            ScrollRect scrollRect = chatHistoryTuple.Item2;

            // Find the dialogue box parent within the chat history content
            Transform dialogueBoxParent = chatHistoryContent.Find("Scroll View/Viewport/Content");

            if (dialogueBoxParent != null)
            {
                // Set spacing between dialogue boxes
                VerticalLayoutGroup layoutGroup = dialogueBoxParent.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup != null)
                {
                    layoutGroup.spacing = 10f; // Set spacing here
                }

                // Instantiate dialogue box under the dialogue box parent within the chat history
                GameObject dialogueBox = Instantiate(dialogueBoxPrefab, dialogueBoxParent);

                // Set dialogue text
                dialogueBox.GetComponentInChildren<TextMeshProUGUI>().text = message;

                // Layout will handle the positioning
                // Ensure the dialogue box fits properly within the parent
                RectTransform dialogueBoxTransform = dialogueBox.GetComponent<RectTransform>();

                // Ensure the pivot is at the top left for proper vertical alignment
                dialogueBoxTransform.pivot = new Vector2(0.5f, 1f);
                dialogueBoxTransform.anchorMin = new Vector2(0.5f, 1f);
                dialogueBoxTransform.anchorMax = new Vector2(0.5f, 1f);
                dialogueBoxTransform.anchoredPosition = new Vector2(0f, 0f);

                // Scroll to bottom after adding the message
                UpdateScrollbarPosition(scrollRect);

                UpdateContentSize(dialogueBoxParent, layoutGroup);

                // Update the last message in the contact prefab
                UpdateLastMessageInContactPrefab(npcName, message);

                CreateMessageNotification(npcName, message);
            }
            else
            {
                Debug.LogError("Dialogue Box parent not found for NPC: " + npcName);
            }
        }
        else
        {
            Debug.LogError("Chat history not found for NPC: " + npcName);
        }
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


    private void ScrollDialogueToBottom(Transform chatHistoryContent)
    {
        // Get the ScrollRect component of the chat history content
        ScrollRect scrollRect = chatHistoryContent.GetComponent<ScrollRect>();

        // Ensure the scroll view scrolls to the bottom
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    // TEST CONTACT

    private void CreateTestContact()
    {
        if (contactScrollViewContent != null)
        {
            InstantiateContact("June", contactScrollViewContent);
        }
        else
        {
            Debug.LogError("Contact ScrollView content area not found.");
        }
    }

    private void SendMessageToTestContact(string message)
    {
        InstantiateOrOpenMessageChat("June");
        AddMessageToHistory("June", message);
    }

    // CREATE MESSAGE

    public void ForCreateNPCMessage(string parameters)
    {
        // Split the parameters into npcName and message
        string[] parts = parameters.Split(new char[] { '|' }, 2);
        if (parts.Length == 2)
        {
            string npcName = parts[0];
            string message = parts[1];
            CreateNPCMessage(npcName, message);
            OpenChatWithNPC(npcName);
        }
        else
        {
            Debug.LogError("Invalid parameters format. Expected 'npcName|message'.");
        }
    }

    private IEnumerator SendMessageAfterDelay(string npcName, string message)
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second

        if (!chatHistory.ContainsKey(npcName))
        {
            Debug.Log("Couroutine Finished");
            // string parameters = $"{npcName}|{message}";
            // ForCreateNPCMessage(parameters);
            AddMessageToHistory(npcName, message);
        }

    }

    public void CreateNPCMessage(string npcName, string message)
    {
        if (chatHistory.ContainsKey(npcName))
        {
            AddMessageToHistory(npcName, message);
        }
        else
        {
            StartCoroutine(SendMessageAfterDelay(npcName, message));
        }
    }
}










