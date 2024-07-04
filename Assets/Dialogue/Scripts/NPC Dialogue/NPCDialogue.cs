using UnityEngine;
using TMPro;
using DialogueEditor;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private bool isInsideTrigger;
    [SerializeField] private GameObject interactTextPrefab; // Reference to the TextMeshPro prefab
    private GameObject interactTextInstance; // Instance of the prefab
    private KeyCode startConversationKey = KeyCode.E;

    [SerializeField] private string conversationTriggeredKey = "NPCDialogueKey";

    private void Start()
    {
        // Load the key from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey(conversationTriggeredKey))
        {
            string keyName = PlayerPrefs.GetString(conversationTriggeredKey);
            startConversationKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;

            if (!PlayerPrefs.HasKey(conversationTriggeredKey))
            {
                ShowInteractText();
                // Debug.Log("Showing interact text on trigger enter");
            }
            else
            {
                HideInteractText();
                // Debug.Log("Hiding interact text on trigger enter");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!PlayerPrefs.HasKey(conversationTriggeredKey) && !ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            ShowInteractText();
            // Debug.Log("Showing interact text on trigger stay");
        }
        else
        {
            HideInteractText();
            // Debug.Log("Hiding interact text on trigger stay");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
            HideInteractText();
            // Debug.Log("Hiding interact text on trigger exit");
        }
    }

    private void StartConversation()
    {
        // Check if DialoguePanel is active
        if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
            return;
        }

        // Disable player movement or any other setup needed before dialogue
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Start the conversation
        ConversationManager.Instance.StartConversation(dialogue);

        // Save the key to PlayerPrefs
        PlayerPrefs.SetString(conversationTriggeredKey, startConversationKey.ToString());
        PlayerPrefs.Save();
    }

    private void Update()
    {
        // Check if the conversation has not started yet
        if (isInsideTrigger && !PlayerPrefs.HasKey(conversationTriggeredKey) && Input.GetKeyDown(startConversationKey))
        {
            StartConversation();
        }
    }

    private void ShowInteractText()
    {
        if (interactTextInstance == null && interactTextPrefab != null)
        {
            // Get collider bounds
            Collider collider = GetComponent<Collider>();
            Vector3 colliderCenter = collider.bounds.center;
            Vector3 colliderExtents = collider.bounds.extents;

            // Calculate position just above the collider
            Vector3 textPosition = colliderCenter + Vector3.up * colliderExtents.y;

            // Instantiate the interactTextPrefab and position it
            interactTextInstance = Instantiate(interactTextPrefab, textPosition, Quaternion.identity, transform);

            // Optionally adjust the position or rotation as needed
            // interactTextInstance.transform.localPosition = new Vector3(0, 0.5f, 0); // Example: Adjust local position

            TextMeshProUGUI textComponent = interactTextInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = "[E] to Interact";
            }
        }
    }

    private void HideInteractText()
    {
        if (interactTextInstance != null)
        {
            Destroy(interactTextInstance);
        }
    }

    // You can call this method if you ever need to reset the PlayerPrefs key (for testing purposes, etc.)
    public void ResetConversationTrigger()
    {
        PlayerPrefs.DeleteKey(conversationTriggeredKey);
        PlayerPrefs.Save();
    }
}