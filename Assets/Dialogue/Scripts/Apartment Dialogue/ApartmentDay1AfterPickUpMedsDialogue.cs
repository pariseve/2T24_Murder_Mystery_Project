using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ApartmentDay1AfterPickUpMedsDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private GameObject itemToMonitor;

    private const string CONVERSATION_TRIGGERED_KEY = "ApartmentDay1AfterPickUpMedsTriggered";

    private void Start()
    {
        // Load the key from PlayerPrefs if it exists
        if (PlayerPrefs.HasKey(CONVERSATION_TRIGGERED_KEY))
        {
            Debug.Log("Dialogue has already been triggered.");
        }
    }

    void Update()
    {
        // Check if the item to monitor is null (destroyed)
        if (itemToMonitor == null && !PlayerPrefs.HasKey(CONVERSATION_TRIGGERED_KEY))
        {
            StartConversation();
        }
    }

    private void StartConversation()
    {
        // Disable player movement or any other setup needed before dialogue
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Check if the ConversationManager is available
        if (ConversationManager.Instance != null)
        {
            // Start the conversation
            ConversationManager.Instance.StartConversation(dialogue);

            // Save the key to PlayerPrefs
            PlayerPrefs.SetString(CONVERSATION_TRIGGERED_KEY, "true");
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("ConversationManager instance is not available.");
        }
    }
}

