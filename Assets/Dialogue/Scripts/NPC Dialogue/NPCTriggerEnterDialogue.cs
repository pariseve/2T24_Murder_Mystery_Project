using UnityEngine;
using TMPro;
using DialogueEditor;

public class NPCTriggerEnterDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private bool isInsideTrigger;

    [SerializeField] private string conversationTriggeredKey = "NPCDialogueKey";

    private void Start()
    {
        // Load the key from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey(conversationTriggeredKey))
        {
            string keyName = PlayerPrefs.GetString(conversationTriggeredKey);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;

            if (!PlayerPrefs.HasKey(conversationTriggeredKey))
            {
                StartConversation();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;

            if (!PlayerPrefs.HasKey(conversationTriggeredKey))
            {
                StartConversation();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
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
        PlayerPrefs.SetString(conversationTriggeredKey, "true");
        PlayerPrefs.Save();
    }

    public void ResetConversationTrigger()
    {
        PlayerPrefs.DeleteKey(conversationTriggeredKey);
        PlayerPrefs.Save();
    }
}

