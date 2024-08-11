using UnityEngine;
using System.Collections;
using DialogueEditor;

public class ObjectClickDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue; // Assign NPCConversation for this instance in the Inspector

    [SerializeField] private bool dialogueStarted = false; // Track dialogue state for this instance

    [SerializeField] private KeyCode startKey = KeyCode.Mouse0;

    private CameraZoom cameraZoom;

    private void Start()
    {
        cameraZoom = FindObjectOfType<CameraZoom>();
        //npcConversation = FindObjectOfType<NPCConversation>();
    }

    void Update()
    {
        if (Input.GetKeyDown(startKey) && !dialogueStarted && !ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            StartConversation();
        }
    }

    private void StartConversation()
    {
        if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            // Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
            return;
        }
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
                if (cameraZoom != null)
                {
                    cameraZoom.ZoomInToObject();
                }
                ConversationManager.Instance.StartConversation(dialogue);
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

