using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreEvidenceItems : MonoBehaviour
{
    // Array of sprite objects where the items will be stored
    public GameObject[] evidenceSlots;

    // Scene name to check
    public string targetSceneName;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == targetSceneName)
        {
            StoreItems();
        }
    }

    // Method to store items
    public void StoreItems()
    {
        // Iterate through each evidence slot
        foreach (GameObject slot in evidenceSlots)
        {
            EvidenceSlot evidenceSlot = slot.GetComponent<EvidenceSlot>();

            if (evidenceSlot != null)
            {
                // Get the uniqueIDReference from the evidence slot
                string uniqueIDReference = evidenceSlot.uniqueIDReference;

                // Check if this uniqueIDReference exists in the PickedUpItems list
                if (InventoryManager.instance.PickedUpItems.Contains(uniqueIDReference))
                {
                    // Create a placeholder Item to pass to RemoveItem
                    Item itemToRemove = null;

                    // Iterate through all inventory slots to find the item with the matching uniqueID
                    foreach (GameObject invSlot in InventoryManager.instance.inventorySlots)
                    {
                        InventoryItem inventoryItem = invSlot.GetComponentInChildren<InventoryItem>();

                        if (inventoryItem != null && inventoryItem.item != null)
                        {
                            // Use the ItemPickup component to get the uniqueID (assuming you have this setup)
                            ItemPickup itemPickup = invSlot.GetComponent<ItemPickup>();

                            if (itemPickup != null && itemPickup.uniqueID == uniqueIDReference)
                            {
                                itemToRemove = inventoryItem.item;
                                break;
                            }
                        }
                    }

                    if (itemToRemove != null)
                    {
                        // Remove the item from the inventory
                        bool removed = InventoryManager.instance.RemoveItem(itemToRemove);

                        if (removed)
                        {
                            // Store the item in the evidence slot
                            evidenceSlot.StoreItem(itemToRemove);
                        }
                        else
                        {
                            Debug.Log("Failed to remove item from inventory.");
                        }
                    }
                }
            }
        }
    }
}
