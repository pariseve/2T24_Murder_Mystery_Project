using UnityEngine;
using DialogueEditor;

public class DoorDream1Dialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation doorCantAccessDialogue;

    // public NPCConversation doorAccessDialogue;

    [SerializeField] private bool dialogueStarted = false; // Track dialogue state for this instance

    private Dream1BoolManager dream1BoolManager;

    private ObjectClickSceneTransitionDream1 objectClickSceneTransitionDream1;

    private void Start()
    {
        //npcConversation = FindObjectOfType<NPCConversation>();
        dream1BoolManager = FindObjectOfType<Dream1BoolManager>();
        if (dream1BoolManager == null)
        {
            Debug.LogError("Dream1BoolManager not found in the scene.");
        }

        objectClickSceneTransitionDream1 = FindObjectOfType<ObjectClickSceneTransitionDream1>();
    }

    void Update()
    {
        if (doorCantAccessDialogue != null && doorCantAccessDialogue.isDialogueActive)
        {
            // If dialogue is active, do not allow zooming
            return;
        }

        if (Input.GetMouseButtonDown(0))// && !dialogue.isDialogueActive)
        {
            if (dream1BoolManager != null && dream1BoolManager.brokenClock && dream1BoolManager.mirror)
            {
                // Raycast to detect object click
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the hit object is the object this script is attached to
                    if (hit.collider.gameObject == gameObject)
                    {
                        objectClickSceneTransitionDream1.FoundAllClues();
                        dialogueStarted = true;
                        //DisableAllColliders();
                        // Start the conversation for this instance
                        // ConversationManager.Instance.StartConversation(doorAccessDialogue);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && !dialogueStarted )// && !dialogue.isDialogueActive)
        {
                // Raycast to detect object click
                RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                    // Check if the hit object is the object this script is attached to
                    if (hit.collider.gameObject == gameObject)
                    {
                        dialogueStarted = true;
                        //DisableAllColliders();
                        // Start the conversation for this instance
                        ConversationManager.Instance.StartConversation(doorCantAccessDialogue);
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
