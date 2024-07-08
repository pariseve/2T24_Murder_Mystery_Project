using UnityEngine;
using DialogueEditor;

public class DoorDreamDialogue : MonoBehaviour
{
    [SerializeField] private NPCConversation doorCantAccessDialogue;

    // public NPCConversation doorAccessDialogue;

    [SerializeField] private bool dialogueStarted = false; // Track dialogue state for this instance

    private ObjectClickSceneTransitionDream objectClickSceneTransitionDream;

    [SerializeField] private string[] requiredBoolNamesTrue;
    [SerializeField] private string[] requiredBoolNamesFalse;

    [SerializeField] private KeyCode startKey = KeyCode.E;

    private void Start()
    {
        objectClickSceneTransitionDream = FindObjectOfType<ObjectClickSceneTransitionDream>();
    }

    void Update()
    {
        if (Input.GetKeyDown(startKey))// && !dialogue.isDialogueActive)
        {
                // Raycast to detect object click
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the hit object is the object this script is attached to
                    if (hit.collider.gameObject == gameObject)
                    {
                    if (AllRequiredBoolsTrue())
                    {
                        objectClickSceneTransitionDream.FoundAllClues();
                        dialogueStarted = true;
                        //DisableAllColliders();
                        // Start the conversation for this instance
                        // ConversationManager.Instance.StartConversation(doorAccessDialogue);
                    }
                }
            }
        }

        if (Input.GetKeyDown(startKey) && !dialogueStarted && AllRequiredBoolsFalse())// && !dialogue.isDialogueActive)
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

    private bool AllRequiredBoolsTrue()
    {
        foreach (string boolName in requiredBoolNamesTrue)
        {
            if (!BoolManager.Instance.GetBool(boolName))
            {
                return false;
            }
        }
        return true;
    }

    private bool AllRequiredBoolsFalse()
    {
        foreach (string boolName in requiredBoolNamesFalse)
        {
            if (BoolManager.Instance.GetBool(boolName))
            {
                return false;
            }
        }
        return true;
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
