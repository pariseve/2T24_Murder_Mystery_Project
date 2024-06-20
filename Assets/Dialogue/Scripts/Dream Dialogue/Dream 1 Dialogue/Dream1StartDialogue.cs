using UnityEngine;
using DialogueEditor;

public class Dream1StartDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation Dream1Start;
    private const string METHOD_TRIGGERED_KEY = "Dream1StartDialogueMethodTriggered";
    private PlayerController playerController;

    void Start()
    {
        // Check if the method has been triggered before
        if (!PlayerPrefs.HasKey(METHOD_TRIGGERED_KEY))
        {
            // If not triggered before, start the dialogue
            StartDialogue();
        }
        else
        {
            // Method has been triggered before, handle accordingly
            Debug.Log("Dialogue has already been triggered.");
        }
    }

    private void StartDialogue()
    {
        // Disable player movement or any other setup needed before dialogue
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Start the conversation
        ConversationManager.Instance.StartConversation(Dream1Start);
        Debug.Log("Method has been triggered for the first time.");

        // Set the flag to prevent this method from being triggered again
        PlayerPrefs.SetInt(METHOD_TRIGGERED_KEY, 1);
        PlayerPrefs.Save();
    }

    // You can call this method if you ever need to reset the flag (for testing purposes, etc.)
    public void ResetTriggerFlag()
    {
        PlayerPrefs.DeleteKey(METHOD_TRIGGERED_KEY);
    }
}

