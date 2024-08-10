using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Add this to use UI components
using DialogueEditor;

public class EatingFoodUI : MonoBehaviour
{
    private InventoryManager inventoryManager;

    [SerializeField] private GameObject parentObject;
    [SerializeField] private NPCConversation dialogue;

    // Reference to the item that will be consumed
    private Item foodItem;

    [SerializeField] private float animationDuration = 0.5f;

    [SerializeField] private Button eatButton;

    private void Start()
    {
        inventoryManager = InventoryManager.instance;

        if (eatButton != null)
        {
            eatButton.onClick.AddListener(EatFood);
        }
    }

    // Method to set the food item to be consumed
    public void SetFoodItem(Item item)
    {
        foodItem = item;
    }

    public void EatFood()
    {
        if (foodItem != null)
        {
            // Remove the item from the inventory
            bool removed = inventoryManager.RemoveItem(foodItem);
            if (removed)
            {
                Debug.Log("Food item consumed: " + foodItem.itemName);

                // Check if DialoguePanel is active
                if (ConversationManager.Instance.DialoguePanel.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("Cannot start a new conversation while the dialogue panel is active.");
                    return;
                }

                PlayerController playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.DisableMovement();
                }

                // Start the conversation
                ConversationManager.Instance.StartConversation(dialogue);
            }
            else
            {
                Debug.LogWarning("Failed to consume the food item.");
            }
        }
        else
        {
            Debug.LogWarning("No food item set for consumption.");
        }
    }

    public void DisableUI()
    {
        StartCoroutine(AnimateUI(false));
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableMovement();
        }
    }

    private IEnumerator AnimateUI(bool enable)
    {
        if (enable)
        {
            parentObject.SetActive(true);
            LeanTween.scale(parentObject, Vector3.one, animationDuration).setEaseOutBounce();
        }
        else
        {
            LeanTween.scale(parentObject, Vector3.zero, animationDuration).setEaseInBounce().setOnComplete(() =>
            {
                parentObject.SetActive(false);
                Destroy(gameObject);
            });
        }

        yield return new WaitForSeconds(animationDuration);
    }
}

