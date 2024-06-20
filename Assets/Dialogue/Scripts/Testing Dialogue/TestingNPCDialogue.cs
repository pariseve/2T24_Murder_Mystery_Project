using UnityEngine;
using DialogueEditor;

public class TestingNPCDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation TestingDialogue;
    [SerializeField] private bool isInsideTrigger;
    [SerializeField] private bool conversationStarted;
    private PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }
        // Start the conversation and set the conversationStarted flag to true
        ConversationManager.Instance.StartConversation(TestingDialogue);
        conversationStarted = true;
    }

    private void Update()
    {
        // Check if the conversation has not started yet
        if (isInsideTrigger && !conversationStarted && Input.GetMouseButtonDown(0))
        {
            StartConversation();
        }
    }
}



