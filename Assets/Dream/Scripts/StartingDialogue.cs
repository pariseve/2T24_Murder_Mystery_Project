using UnityEngine;
using DialogueEditor;

public class StartingDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    [SerializeField] private bool conversationStarted;

    private void Start()
    {
        if (!conversationStarted)
        {
            // Start the conversation and set the conversationStarted flag to true
            ConversationManager.Instance.StartConversation(dialogue);
            conversationStarted = true;
        }
    }
}
