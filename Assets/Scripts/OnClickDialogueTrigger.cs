using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;
using UnityEngine.EventSystems;

public class OnClickDialogueTrigger : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private LayerMask Interactable; // LayerMask to filter interactable objects
    [SerializeField] private NPCConversation dialogue; // Dialogue assigned to this object
    private KeyCode startConversationKey = KeyCode.Mouse0;

    private void Start()
    {
        cam = Camera.main; // Use Camera.main to get the main camera
    }

    private void Update()
    {
        // Check if the player has interacted with the object
        if (Input.GetKeyDown(startConversationKey))
        {
            if (!IsPointerOverUIElement()) // Only start conversation if the pointer is not over UI
            {
                StartConversation();
            }
        }
    }

    private void StartConversation()
    {
        // Check if DialoguePanel is active
        if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Interactable))
        {
            // Ensure the hit object has an OnClickDialogueTrigger component
            OnClickDialogueTrigger clickedTrigger = hit.collider.GetComponent<OnClickDialogueTrigger>();
            if (clickedTrigger != null)
            {
                NPCConversation hitDialogue = clickedTrigger.dialogue;
                Debug.Log($"Starting conversation with dialogue: {hitDialogue}");

                // Start the conversation
                ConversationManager.Instance.StartConversation(hitDialogue);
            }
            else
            {
                Debug.LogWarning("No OnClickDialogueTrigger component found on the hit object.");
            }
        }
        else
        {
            Debug.LogWarning("No object hit by the raycast.");
        }
    }

    private bool IsPointerOverUIElement()
    {
        // Get the current event system
        EventSystem eventSystem = EventSystem.current;

        // Get all the EventSystems in the scene, including those in DontDestroyOnLoad objects
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

        // Loop through each EventSystem to check if the pointer is over any UI elements
        foreach (var es in eventSystems)
        {
            if (es.IsPointerOverGameObject())
            {
                // Check if the pointer is over a UI element
                PointerEventData eventData = new PointerEventData(es)
                {
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                es.RaycastAll(eventData, results);

                foreach (var result in results)
                {
                    // Check if the UI element is in front of the collider
                    if (result.gameObject != null && result.gameObject.GetComponent<CanvasRenderer>() != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
