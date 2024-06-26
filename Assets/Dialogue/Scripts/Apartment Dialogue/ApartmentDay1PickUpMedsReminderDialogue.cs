using UnityEngine;
using TMPro;
using DialogueEditor;

public class ApartmentDay1PickUpMedsReminderDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private bool isInsideTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartConversation();
            isInsideTrigger = true;
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
        // Disable player movement or any other setup needed before dialogue
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        // Start the conversation
        ConversationManager.Instance.StartConversation(dialogue);
    }
}
