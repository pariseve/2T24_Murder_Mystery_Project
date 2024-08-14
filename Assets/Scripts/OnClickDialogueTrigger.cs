using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;

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
            StartConversation();
        }
    }

    private void StartConversation()
    {
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

                // Check if DialoguePanel is active
                if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
                    return;
                }

                // Disable player movement or any other setup needed before dialogue
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.DisableMovement();
                }

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
}
