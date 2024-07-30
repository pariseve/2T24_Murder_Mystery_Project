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
        // Find all items in the inventory that are of type Evidence
        List<Item> evidenceItems = InventoryManager.instance.FindAllItemsOfType(ItemType.Evidence);

        int slotIndex = 0;

        foreach (Item evidenceItem in evidenceItems)
        {
            // Find the next available slot
            while (slotIndex < evidenceSlots.Length)
            {
                GameObject slot = evidenceSlots[slotIndex];
                EvidenceSlot evidenceSlot = slot.GetComponent<EvidenceSlot>();

                if (evidenceSlot != null && !evidenceSlot.HasItem())
                {
                    // Remove the item from the inventory
                    bool removed = InventoryManager.instance.RemoveItem(evidenceItem);

                    if (removed)
                    {
                        // Store the item in the evidence slot
                        evidenceSlot.StoreItem(evidenceItem);
                        break;
                    }
                }

                slotIndex++;
            }

            if (slotIndex >= evidenceSlots.Length)
            {
                Debug.Log("Not enough slots to store all evidence items.");
                break;
            }
        }
    }
}
