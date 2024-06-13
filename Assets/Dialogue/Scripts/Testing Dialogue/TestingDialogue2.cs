using UnityEngine;
using DialogueEditor;

public class TestingDialogue2 : MonoBehaviour
{
    [SerializeField] private NPCConversation testingDialogue2;
    [SerializeField] private bool isInsideTrigger;
    [SerializeField] private bool conversationStarted;

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
        // Start the conversation and set the conversationStarted flag to true
        ConversationManager.Instance.StartConversation(testingDialogue2);
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