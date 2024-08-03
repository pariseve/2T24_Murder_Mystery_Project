using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class MessageManager : MonoBehaviour
{
    [SerializeField] private GameObject npcMessageWindowPrefab;
    [SerializeField] private GameObject dialogueBoxPrefab;
    [SerializeField] private GameObject contactPrefab;
    [SerializeField] private Transform messageChatParent;
    [SerializeField] private Transform contactParent;
    [SerializeField] private GameObject messageApplication;
    [SerializeField] private GameObject contactsUI;

    [SerializeField] private GameObject messageNotificationPrefab; // Reference to the message notification prefab
    [SerializeField] private Transform messageNotificationParent; // Parent transform for the message notifications
    [SerializeField] private TextMeshProUGUI unreadMessageCountText;

    [SerializeField] private Dictionary<string, GameObject> messageChatInstances = new Dictionary<string, GameObject>();
    private Dictionary<string, List<string>> chatHistory = new Dictionary<string, List<string>>();

    private bool testContactCreated = false;
    private float contactSpacing = 0.5f; // Vertical spacing between contacts
    private float messageSpacing = 1.5f; // Vertical spacing between messages

    private bool isDraggingScrollbar = false;

    private Transform contactScrollViewContent; // Reference to the content area of the Contact ScrollView

    private Dictionary<string, GameObject> contactInstances = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject contactScrollView; // Add this line

    private Dictionary<string, bool> unreadMessages = new Dictionary<string, bool>();

    // PLAYER REPLY VARIABLES
    [SerializeField] private GameObject playerDialogueBoxPrefab;

    private string playerReply;

    public static MessageManager Instance { get; private set; }

    // Ensure only one instance exists across scenes
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

    private void Start()
    {
        // Find the content area of the Contact ScrollView
        if (contactScrollView != null)
        {
            contactScrollViewContent = contactScrollView.transform.Find("Viewport/Content");
        }
        else
        {
            Debug.LogError("Contact Scroll View not assigned in the inspector.");
            return; // Exit early if contact scroll view is not assigned
        }

        // Find existing contact instance under contactParent and add it to the dictionary
        if (contactParent != null)
        {
            Transform ryanContactTransform = FindInactiveObject(contactParent, "Ryan_Contact");
            if (ryanContactTransform != null)
            {
                string npcName = "Ryan";

                // Check if "Ryan" is already in contactInstances
                if (!contactInstances.ContainsKey(npcName))
                {
                    contactInstances.Add(npcName, ryanContactTransform.gameObject);

                    // Add event trigger to open chat history
                    EventTrigger contactTrigger = ryanContactTransform.gameObject.GetComponent<EventTrigger>();
                    if (contactTrigger == null)
                    {
                        contactTrigger = ryanContactTransform.gameObject.AddComponent<EventTrigger>();
                    }

                    EventTrigger.Entry contactEntry = new EventTrigger.Entry();
                    contactEntry.eventID = EventTriggerType.PointerClick;
                    contactEntry.callback.AddListener((eventData) => { OpenChatWithNPC(npcName); });

                    // UpdateContactListContentSize();

                    contactTrigger.triggers.Add(contactEntry);

                    Debug.Log("Contact for " + npcName + " found and added to contactInstances.");
                }
                else
                {
                    Debug.LogWarning("Contact for " + npcName + " already exists in contactInstances.");
                }
            }
            else
            {
                Debug.LogError("Ryan_Contact not found under contactParent.");
            }
        }
        else
        {
            Debug.LogError("Contact Parent not assigned in the inspector.");
        }

        // Find existing chat history under messageChatParent and add it to the dictionary
        if (messageChatParent != null)
        {
            string npcName = "Ryan";
            Transform ryanChatHistoryTransform = FindInactiveObject(messageChatParent, "Ryan_ChatHistory");
            if (ryanChatHistoryTransform != null)
            {
                // Check if "Ryan" is already in messageChatInstances
                if (!messageChatInstances.ContainsKey(npcName))
                {
                    messageChatInstances.Add(npcName, ryanChatHistoryTransform.gameObject);
                }
                else
                {
                    Debug.LogWarning("Chat history instance for " + npcName + " already exists in messageChatInstances.");
                }

                // Check if "Ryan" is already in chatHistory
                if (!chatHistory.ContainsKey(npcName))
                {
                    // Initialize or load your chat history list here if needed
                    chatHistory.Add(npcName, new List<string>());
                }
                else
                {
                    Debug.LogWarning("Chat history for " + npcName + " already exists in chatHistory.");
                }
            }
            else
            {
                Debug.LogError("Ryan_ChatHistory not found under messageChatParent.");
            }
        }
        else
        {
            Debug.LogError("Message Chat Parent not assigned in the inspector.");
        }

        // Initialize unreadMessages for "Ryan"
        if (!unreadMessages.ContainsKey("Ryan"))
        {
            unreadMessages.Add("Ryan", false); // Assuming false means unread initially
        }

        AddBackImageListener();
        AddBackImageListenerToContacts();
    }

    private Transform FindInactiveObject(Transform parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>(true); // Include inactive children
        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    private void AddBackImageListener()
    {
        foreach (var chatInstance in messageChatInstances.Values)
        {
            Image backImage = chatInstance.GetComponentInChildren<Image>(true); // Search in all children
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
                    // Deactivate all message chat histories
                    DeactivateAllChatHistories();

                    // Activate the message application
                    if (messageApplication != null)
                    {
                        messageApplication.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("Message Application not assigned in the inspector.");
                    }
                });

                // Add the entry to the EventTrigger events list
                trigger.triggers.Add(entry);
            }
            else
            {
                Debug.LogError("Back Image not found in the chat instance prefab.");
            }
        }
    }

    private void AddBackImageListenerToContacts()
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

        // Disable the notesUI
        contactsUI.SetActive(false);

        // Wait for a short time
        yield return new WaitForSeconds(0.1f); // Adjust delay as needed

        // Set notesUI back to its original scale
        contactsUI.transform.localScale = originalScale;
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
        GameObject notification = Instantiate(messageNotificationPrefab);

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

        // Ensure it takes the next available slot in the GridLayoutGroup
        if (messageNotificationParent != null)
        {
            GridLayoutGroup gridLayoutGroup = messageNotificationParent.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup != null)
            {
                RectTransform rectTransform = notification.GetComponent<RectTransform>();
                rectTransform.SetParent(messageNotificationParent);
                rectTransform.localScale = Vector3.one; // Reset scale to avoid scaling issues
                rectTransform.SetAsLastSibling(); // Move to the next available slot
            }
            else
            {
                Debug.LogError("GridLayoutGroup component not found in the message notification parent.");
            }
        }

        // Play notification audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPhoneNotification();
        }

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

    public void InstantiateOrOpenMessageChat(string npcName, string playerReply)
    {
        if (messageChatInstances.ContainsKey(npcName))
        {
            OpenChatWithNPC(npcName);
        }
        else
        {
            InstantiateMessageChat(npcName, playerReply);
            InstantiateContact(npcName, contactScrollViewContent);
        }
    }

    private void InstantiateMessageChat(string npcName, string playerReply)
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

        // Add a reply button listener
        // Button replyButton = messageChatUI.transform.Find("Reply Image/ReplyButton").GetComponent<Button>();
        // replyButton.onClick.AddListener(() => OnReplyButtonClicked(npcName, playerReply));
        // replyButton.gameObject.SetActive(false); // Initially set to false
        AddBackImageListener();
    }

    private void OpenChatWithNPC(string npcName)
    {
        SetMessageRead(npcName);

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

            // Show the reply button
            // GameObject messageChatUI = messageChatInstances[npcName];
            // Button replyButton = messageChatUI.transform.Find("Reply Image/ReplyButton").GetComponent<Button>();
            // replyButton.gameObject.SetActive(true);
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

            AddBackImageListener();

            // Show the reply button
            // Button replyButton = newChatHistory.transform.Find("Reply Image/ReplyButton").GetComponent<Button>();
            // replyButton.onClick.AddListener(() => OnReplyButtonClicked(npcName, playerReply));
            // replyButton.gameObject.SetActive(true); // Initially set to false
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

    // Define a class-level variable to store the playerReply for each NPC
    private Dictionary<string, string> lastPlayerReplies = new Dictionary<string, string>();

    public void ForCreateNPCMessage(string parameters)
    {
        Debug.Log("Parameters: " + parameters);
        // Split the parameters into npcName, message, and player reply
        string[] parts = parameters.Split(new char[] { '|' }, 3);
        if (parts.Length == 3)
        {
            string npcName = parts[0];
            string message = parts[1];
            string playerReply = parts[2];
            OpenChatWithNPC(npcName);
            CreateNPCMessage(npcName, message, playerReply);
            SetMessageUnread(npcName);

            // Remove the previous playerReply for this NPC, if it exists
            if (lastPlayerReplies.ContainsKey(npcName))
            {
                lastPlayerReplies.Remove(npcName);
            }

            // Store the last playerReply for this NPC
            lastPlayerReplies[npcName] = playerReply;
        }
        else
        {
            Debug.LogError("Invalid parameters format. Expected 'npcName|message|playerReply'.");
        }
    }

    private void SetMessageUnread(string npcName)
    {
        if (unreadMessages.ContainsKey(npcName))
        {
            unreadMessages[npcName] = true;
        }
        else
        {
            unreadMessages.Add(npcName, true);
        }

        // Update the "New Message" indicator for the contact
        UpdateNewMessageIndicator(npcName);
        // Update unread message count
        UpdateUnreadMessageCount();
    }

    private void SetMessageRead(string npcName)
    {
        if (unreadMessages.ContainsKey(npcName))
        {
            unreadMessages[npcName] = false;
        }

        // Update the "New Message" indicator for the contact
        UpdateNewMessageIndicator(npcName);
        // Update unread message count
        UpdateUnreadMessageCount();
    }

    private void UpdateNewMessageIndicator(string npcName)
    {
        if (contactInstances.ContainsKey(npcName))
        {
            GameObject contactInstance = contactInstances[npcName];
            Transform newMessageIndicator = contactInstance.transform.Find("New Message");

            if (newMessageIndicator != null)
            {
                newMessageIndicator.gameObject.SetActive(unreadMessages[npcName]);
            }
        }
    }

    public bool HasUnreadMessages()
    {
        foreach (var unread in unreadMessages.Values)
        {
            if (unread)
            {
                return true;
            }
        }
        return false;
    }

    // Add this method to display the new message notification
    public void DisplayNewMessageNotification()
    {
        foreach (var npc in unreadMessages)
        {
            if (contactInstances.ContainsKey(npc.Key))
            {
                GameObject contactInstance = contactInstances[npc.Key];
                Transform newMessageIndicator = contactInstance.transform.Find("New Message");

                if (newMessageIndicator != null)
                {
                    newMessageIndicator.gameObject.SetActive(npc.Value);
                }
            }
        }
    }

    // Method to update the unread message count
    private void UpdateUnreadMessageCount()
    {
        int unreadCount = CountUnreadMessages();
        unreadMessageCountText.text = unreadCount.ToString();
    }

    // Method to count the unread messages
    private int CountUnreadMessages()
    {
        int count = 0;
        foreach (var unread in unreadMessages.Values)
        {
            if (unread)
            {
                count++;
            }
        }
        return count;
    }

private void OnReplyButtonClicked(string npcName, string playerReply)
    {
        Debug.Log("Button Clicked");
        Debug.Log("Player Reply: " + playerReply); // Debugging

        if (lastPlayerReplies.ContainsKey(npcName))
        {
            playerReply = lastPlayerReplies[npcName];
        }
        else
        {
            Debug.LogWarning("No previous playerReply found for NPC: " + npcName);
        }

        // Find the chat history content for the NPC
        var chatHistoryTuple = FindChatHistoryContent(npcName);

        if (chatHistoryTuple == null)
        {
            Debug.LogError("Chat history content tuple is null for NPC: " + npcName);
            return;
        }

        Transform chatHistoryContent = chatHistoryTuple.Item1;

        if (chatHistoryContent == null)
        {
            Debug.LogError("Chat history content transform is null for NPC: " + npcName);
            return;
        }

        // Find the dialogue box parent within the chat history content
        Transform dialogueBoxParent = chatHistoryContent.Find("Scroll View/Viewport/Content");

        if (dialogueBoxParent == null)
        {
            Debug.LogError("Dialogue Box parent not found for NPC: " + npcName);
            return;
        }

        // Instantiate player dialogue box as a child of the dialogue box parent
        GameObject dialogueBox = Instantiate(playerDialogueBoxPrefab, dialogueBoxParent);

        // Set the player's reply text
        TextMeshProUGUI dialogueText = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        if (dialogueText != null)
        {
            dialogueText.text = playerReply;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in dialogue box prefab.");
        }

        // Position the player dialogue box on the right side
        RectTransform rectTransform = dialogueBox.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 0.5f);
        rectTransform.anchorMax = new Vector2(1f, 0.5f);
        rectTransform.pivot = new Vector2(1f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(-10f, 0f); // Adjust as needed

        // Add the player's reply to the chat history
        AddMessageToHistoryForPlayerReply(npcName, playerReply);

        // Set spacing between dialogue boxes
        VerticalLayoutGroup layoutGroup = dialogueBoxParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = 10f; // Set spacing here
        }

        ScrollRect scrollRect = chatHistoryTuple.Item2;

        // Scroll to bottom after adding the message
        UpdateScrollbarPosition(scrollRect);

        UpdateContentSize(dialogueBoxParent, layoutGroup);

        HideReplyButton(npcName);
    }

    public void ForInstantiateContact(string npcName)
    {
        InstantiateContact(npcName, contactScrollViewContent);
    }

    private void InstantiateContact(string npcName, Transform parentTransform)
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

        if (!unreadMessages.ContainsKey(npcName))
        {
            unreadMessages.Add(npcName, true);
        }
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

            // Scroll to the bottom
            Canvas.ForceUpdateCanvases();
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

    private void AddMessageToHistoryForNPC(string npcName, string message, string playerReply = null)
    {
        if (string.IsNullOrEmpty(npcName))
        {
            Debug.LogError("NPC Name is null or empty.");
            return;
        }

        if (chatHistory == null)
        {
            Debug.LogError("Chat history dictionary is null.");
            return;
        }

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

        if (chatHistoryTuple == null)
        {
            Debug.LogError("Chat history content tuple is null for NPC: " + npcName);
            return;
        }

        Transform chatHistoryContent = chatHistoryTuple.Item1;
        ScrollRect scrollRect = chatHistoryTuple.Item2;

        if (chatHistoryContent == null)
        {
            Debug.LogError("Chat history content transform is null for NPC: " + npcName);
            return;
        }

        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect is null for NPC: " + npcName);
            return;
        }

        // Find the dialogue box parent within the chat history content
        Transform dialogueBoxParent = chatHistoryContent.Find("Scroll View/Viewport/Content");

        if (dialogueBoxParent == null)
        {
            Debug.LogError("Dialogue Box parent not found for NPC: " + npcName);
            return;
        }

        // Set spacing between dialogue boxes
        VerticalLayoutGroup layoutGroup = dialogueBoxParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = 10f; // Set spacing here
        }

        // Instantiate dialogue box under the dialogue box parent within the chat history
        if (dialogueBoxPrefab == null)
        {
            Debug.LogError("Dialogue box prefab is null.");
            return;
        }

        GameObject dialogueBox = Instantiate(dialogueBoxPrefab, dialogueBoxParent);

        // Set dialogue text
        TextMeshProUGUI dialogueText = dialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        if (dialogueText != null)
        {
            dialogueText.text = message;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in dialogue box prefab.");
        }

        // Layout will handle the positioning
        // Ensure the dialogue box fits properly within the parent
        RectTransform dialogueBoxTransform = dialogueBox.GetComponent<RectTransform>();

        // Ensure the pivot is at the top left for proper vertical alignment
        dialogueBoxTransform.pivot = new Vector2(0f, 1f);
        dialogueBoxTransform.anchorMin = new Vector2(0f, 1f);
        dialogueBoxTransform.anchorMax = new Vector2(1f, 1f);
        dialogueBoxTransform.anchoredPosition = new Vector2(0f, 0f);

        // Scroll to bottom after adding the message
        UpdateScrollbarPosition(scrollRect);

        UpdateContentSize(dialogueBoxParent, layoutGroup);

        // Update the last message in the contact prefab
        UpdateLastMessageInContactPrefab(npcName, message);

        CreateMessageNotification(npcName, message);

        // Add reply button listener for NPC messages
        GameObject messageChatUI = messageChatInstances[npcName];
        Button replyButton = messageChatUI.transform.Find("Reply Image/ReplyButton").GetComponent<Button>();

        replyButton.onClick.RemoveAllListeners();
        string localPlayerReply = playerReply; // Create a local copy
        replyButton.onClick.AddListener(() => OnReplyButtonClicked(npcName, localPlayerReply)); // Use the local copy
        replyButton.gameObject.SetActive(true);
    }

    public void AddMessageToHistoryForPlayerReply(string npcName, string playerReply)
    {
        if (string.IsNullOrEmpty(npcName))
        {
            Debug.LogError("NPC Name is null or empty.");
            return;
        }

        if (chatHistory == null)
        {
            Debug.LogError("Chat history dictionary is null.");
            return;
        }

        // Check if the NPC chat history exists in the dictionary
        if (chatHistory.ContainsKey(npcName))
        {
            // Add the player's reply to the existing chat history
            chatHistory[npcName].Add(playerReply);
        }
        else
        {
            // Create a new chat history list for the NPC if it doesn't exist
            chatHistory[npcName] = new List<string>() { playerReply };
        }

        // Find the chat history content for the NPC
        var chatHistoryTuple = FindChatHistoryContent(npcName);

        if (chatHistoryTuple == null)
        {
            Debug.LogError("Chat history content tuple is null for NPC: " + npcName);
            return;
        }

        Transform chatHistoryContent = chatHistoryTuple.Item1;
        ScrollRect scrollRect = chatHistoryTuple.Item2;

        if (chatHistoryContent == null)
        {
            Debug.LogError("Chat history content transform is null for NPC: " + npcName);
            return;
        }

        if (scrollRect == null)
        {
            Debug.LogError("ScrollRect is null for NPC: " + npcName);
            return;
        }

        // Find the dialogue box parent within the chat history content
        /*
        Transform dialogueBoxParent = chatHistoryContent.Find("Scroll View/Viewport/Content");

        if (dialogueBoxParent == null)
        {
            Debug.LogError("Dialogue Box parent not found for NPC: " + npcName);
            return;
        }

        // Set spacing between dialogue boxes
        VerticalLayoutGroup layoutGroup = dialogueBoxParent.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.spacing = 10f; // Set spacing here
        }

        // Instantiate dialogue box under the dialogue box parent within the chat history
        if (dialogueBoxPrefab == null)
        {
            Debug.LogError("Dialogue box prefab is null.");
            return;
        }

        GameObject playerDialogueBox = Instantiate(playerDialogueBoxPrefab, dialogueBoxParent);

        // Set dialogue text
        TextMeshProUGUI playerDialogueText = playerDialogueBox.GetComponentInChildren<TextMeshProUGUI>();
        if (playerDialogueText != null)
        {
            playerDialogueText.text = playerReply;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in dialogue box prefab.");
        }
        *
        // Layout will handle the positioning
        // Ensure the dialogue box fits properly within the parent
        RectTransform dialogueBoxTransform = playerDialogueBox.GetComponent<RectTransform>();

        // Ensure the pivot is at the top left for proper vertical alignment
        dialogueBoxTransform.pivot = new Vector2(0f, 1f);
        dialogueBoxTransform.anchorMin = new Vector2(0f, 1f);
        dialogueBoxTransform.anchorMax = new Vector2(1f, 1f);
        dialogueBoxTransform.anchoredPosition = new Vector2(0f, 0f);

        // Scroll to bottom after adding the message
        UpdateScrollbarPosition(scrollRect);

        UpdateContentSize(dialogueBoxParent, layoutGroup);
        */
        // Update the last message in the contact prefab
        UpdateLastMessageInContactPrefab(npcName, playerReply);

        // Show the reply button for NPC messages
        // ShowReplyButton(npcName);

        // Create notification for NPC messages
        // CreateMessageNotification(npcName, playerReply);

        // Add reply button listener for NPC messages
        /*
        GameObject messageChatUI = messageChatInstances[npcName];
        Transform replyButtonTransform = messageChatUI.transform.Find("Reply Image/ReplyButton");
        if (replyButtonTransform == null)
        {
            Debug.LogError("Reply button not found in prefab.");
            return;
        }

        Button replyButton = replyButtonTransform.GetComponent<Button>();
        if (replyButton != null)
        {
            // Use the lastPlayerReply variable inside the lambda expression
            replyButton.onClick.AddListener(() => OnReplyButtonClicked(npcName, playerReply));
            replyButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Button component not found on reply button.");
        }
        */
    }


    private void ShowReplyButton(string npcName)
    {
        // Check if the NPC message window exists
        if (messageChatInstances.ContainsKey(npcName))
        {
            // Get the NPC message window
            GameObject messageChatUI = messageChatInstances[npcName];

            // Find the reply button within the NPC message window
            Transform replyButtonTransform = messageChatUI.transform.Find("Reply Image/ReplyButton");

            // Check if the reply button is found
            if (replyButtonTransform != null)
            {
                Button replyButton = replyButtonTransform.GetComponent<Button>();

                // Activate the reply button
                if (replyButton != null)
                {
                    replyButton.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogError("Reply button component not found for NPC: " + npcName);
                }
            }
            else
            {
                Debug.LogError("Reply button not found in message window for NPC: " + npcName);
            }
        }
        else
        {
            Debug.LogError("Message window not found for NPC: " + npcName);
        }
    }

    private void HideReplyButton(string npcName)
    {
        // Check if the NPC message window exists
        if (messageChatInstances.ContainsKey(npcName))
        {
            // Get the NPC message window
            GameObject messageChatUI = messageChatInstances[npcName];

            // Find the reply button within the NPC message window
            Transform replyButtonTransform = messageChatUI.transform.Find("Reply Image/ReplyButton");

            // Check if the reply button is found
            if (replyButtonTransform != null)
            {
                Button replyButton = replyButtonTransform.GetComponent<Button>();

                // Activate the reply button
                if (replyButton != null)
                {
                    replyButton.gameObject.SetActive(false);
                }
                else
                {
                    Debug.LogError("Reply button component not found for NPC: " + npcName);
                }
            }
            else
            {
                Debug.LogError("Reply button not found in message window for NPC: " + npcName);
            }
        }
        else
        {
            Debug.LogError("Message window not found for NPC: " + npcName);
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
        // InstantiateOrOpenMessageChat("June");
        // AddMessageToHistory("June", message);
    }

    // CREATE MESSAGE

    private void CreateNPCMessage(string npcName, string message, string playerReply)
    {
        if (chatHistory.ContainsKey(npcName))
        {
            // Add NPC message to chat history
            AddMessageToHistoryForNPC(npcName, message, playerReply);

            // Show the reply button for the NPC
            ShowReplyButton(npcName);
        }
        else
        {
            StartCoroutine(SendMessageAfterDelay(npcName, message, playerReply));
        }
    }

    private IEnumerator SendMessageAfterDelay(string npcName, string message, string playerReply)
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second

        if (!chatHistory.ContainsKey(npcName))
        {
            AddMessageToHistoryForNPC(npcName, message, playerReply);
        }
    }
}










