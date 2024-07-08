using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DialogueEditor;

public class RemoveItemFromInventory : MonoBehaviour
{
    [SerializeField] private Item itemToRemove;
    [SerializeField] private KeyCode useKey = KeyCode.E;
    [SerializeField] private string boolName = "";

    [SerializeField] private string[] requiredBoolNamesTrue; // Array of boolean names that need to be true
    [SerializeField] private string[] requiredBoolNamesFalse; // Array of boolean names that need to be false

    [SerializeField] private GameObject interactTextPrefab;
    private GameObject interactTextInstance;

    [SerializeField] private bool isInsideTrigger = false;

    [SerializeField] private NPCConversation returnDialogue;
    [SerializeField] private NPCConversation successfulDialogue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (AllRequiredBoolsTrue() && AllRequiredBoolsFalse())
            {
                ShowInteractText();
            }
            else
            {
                HideInteractText();
            }

            // Use the item when the player presses the use key and conditions are met
            // if (Input.GetKeyDown(useKey))
            // {
            //     UseItem();
            // }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideInteractText();
            isInsideTrigger = false;
        }
    }

    private void Update()
    {
        if (isInsideTrigger && Input.GetKeyDown(useKey))
        {
            // Check if DialoguePanel is active
            if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
                return;
            }

            // Check if all required booleans are true and all required booleans are false
            if (AllRequiredBoolsTrue() && AllRequiredBoolsFalse())
            {
                UseItem();
            }

            if (AllRequiredBoolsTrue() && AllRequiredBoolsFalse() && !InventoryManager.instance.HasItem(itemToRemove))
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

        // Disable player movement or any other setup needed before dialogue
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableMovement();
        }

        if (!InventoryManager.instance.HasItem(itemToRemove))
        {
            // Start the conversation
            ConversationManager.Instance.StartConversation(returnDialogue);
        }
        else
        {
            UseItem();
        }
    }

        private void UseItem()
    {
        if (itemToRemove == null)
        {
            Debug.LogWarning("Item to remove is not assigned.");
            return;
        }
        if (AllRequiredBoolsTrue() && AllRequiredBoolsFalse())
        {
            // Call RemoveItem from InventoryManager to remove the item
            if (InventoryManager.instance.HasItem(itemToRemove))
            {
                // Check if DialoguePanel is active
                if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
                    return;
                }

                Debug.Log("Item used from inventory: " + itemToRemove.itemName);
                // Implement your item use logic here
                InventoryManager.instance.RemoveItem(itemToRemove);
                ExecuteItemUseLogic(itemToRemove);
                BoolManager.Instance.SetBool(boolName, true);

                // Disable player movement or any other setup needed before dialogue
                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.DisableMovement();
                }

                ConversationManager.Instance.StartConversation(successfulDialogue);
            }
            else
            {
                Debug.Log("Failed to use item from inventory: " + itemToRemove.itemName);
            }
        }
        else
        {
            Debug.Log("Item not found in inventory: " + itemToRemove.itemName);
        }
    }

    private void ExecuteItemUseLogic(Item item)
    {
        // BoolManager.Instance.SetBool(boolName, true);
        Debug.Log("Executing item use logic for: " + item.itemName);
    }

    public void RemoveItemFromInv()
    {
        if (InventoryManager.instance.RemoveItem(itemToRemove))
        {
            Debug.Log("Item removed from inventory: " + itemToRemove.itemName);
        }
        else
        {
            Debug.Log("Failed to remove item from inventory: " + itemToRemove.itemName);
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

    private void ShowInteractText()
    {
        // Check if all required booleans are true and all required booleans are false before showing the interact text
        if (AllRequiredBoolsTrue() && AllRequiredBoolsFalse())
        {
            if (interactTextInstance == null && interactTextPrefab != null)
            {
                // Get collider bounds
                Collider collider = GetComponent<Collider>();
                Vector3 colliderCenter = collider.bounds.center;
                Vector3 colliderExtents = collider.bounds.extents;

                // Calculate position just above the collider
                Vector3 textPosition = colliderCenter + Vector3.up * colliderExtents.y;

                // Instantiate the interactTextPrefab and position it
                interactTextInstance = Instantiate(interactTextPrefab, textPosition, Quaternion.identity, transform);

                // Optionally adjust the position or rotation as needed
                // interactTextInstance.transform.localPosition = new Vector3(0, 0.5f, 0); // Example: Adjust local position

                TextMeshProUGUI textComponent = interactTextInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = $"[{useKey}] to Interact";
                }
            }
        }
        else
        {
            Debug.LogWarning("Cannot show interact text because one or more required booleans are not in the correct state.");
        }
    }

    private void HideInteractText()
    {
        if (interactTextInstance != null)
        {
            Destroy(interactTextInstance);
        }
    }
}
