using UnityEngine;
using DialogueEditor;

public class ObjectClickDialogue : MonoBehaviour
{
    public NPCConversation dialogue; // Assign NPCConversation for this instance in the Inspector

    public bool dialogueStarted = false; // Track dialogue state for this instance

    private void Start()
    {
        //npcConversation = FindObjectOfType<NPCConversation>();
    }

    void Update()
    {
        if (dialogue != null && dialogue.isDialogueActive)
        {
            // If dialogue is active, do not allow zooming
            return;
        }

        if (Input.GetMouseButtonDown(0) && !dialogueStarted)// && !dialogue.isDialogueActive)
        {
            // Raycast to detect object click
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is the object this script is attached to
                if (hit.collider.gameObject == gameObject)
                {
                    //DisableAllColliders();
                    // Start the conversation for this instance
                    dialogueStarted = true;
                    ConversationManager.Instance.StartConversation(dialogue);
                }
            }
        }
    }

    public void DisableAllColliders()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (Collider collider in allColliders)
        {
            collider.enabled = false;
        }
    }

    public void EnableAllColliders()
    {
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (Collider collider in allColliders)
        {
            collider.enabled = true;
        }
    }
}

