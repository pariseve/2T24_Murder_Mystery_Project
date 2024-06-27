using UnityEngine;
using System.Collections;
using DialogueEditor;

public class ObjectClickDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue; // Assign NPCConversation for this instance in the Inspector

    [SerializeField] private bool dialogueStarted = false; // Track dialogue state for this instance

    private CameraZoom cameraZoom;

    private void Start()
    {
        cameraZoom = FindObjectOfType<CameraZoom>();
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
                    // Start the conversation for this instance
                    dialogueStarted = true;
                    // DisableAllColliders();
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

