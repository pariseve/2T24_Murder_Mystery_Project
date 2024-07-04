using UnityEngine;
using DialogueEditor;
using System.Collections;

public class DreamStartDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dreamStartDialogue;
    [SerializeField] private string METHOD_TRIGGERED_KEY = "";
    private PlayerController playerController;
    private ToggleLookAround toggleLookAround;

    void Start()
    {
        toggleLookAround = FindObjectOfType<ToggleLookAround>();
        // Check if the method has been triggered before
        if (!PlayerPrefs.HasKey(METHOD_TRIGGERED_KEY))
        {
            toggleLookAround.DisableComponent();
            // If not triggered before, start the dialogue
            StartCoroutine(StartTheDialogue());
        }
        else
        {
            // Method has been triggered before, handle accordingly
            Debug.Log("Dialogue has already been triggered.");
        }
    }

    private IEnumerator StartTheDialogue()
    {
        yield return new WaitForSeconds(0.5f);
        StartDialogue();

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
        ConversationManager.Instance.StartConversation(dreamStartDialogue);
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

