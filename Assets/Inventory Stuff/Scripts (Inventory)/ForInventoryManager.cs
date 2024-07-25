using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForInventoryManager : MonoBehaviour
{
    [SerializeField] private Item[] itemsToCheck;
    [SerializeField] private Item[] itemsToAdd;

    public void ForAddingItem()
    {
        if (itemsToCheck == null || itemsToAdd == null)
        {
            Debug.LogWarning("Items to check or add are not set.");
            return;
        }

        // Dictionary to map items to check with their corresponding items to add
        Dictionary<Item, Item> itemMap = new Dictionary<Item, Item>();

        // Populate the dictionary
        for (int i = 0; i < Mathf.Min(itemsToCheck.Length, itemsToAdd.Length); i++)
        {
            itemMap[itemsToCheck[i]] = itemsToAdd[i];
        }

        // Check if all items to check are in the inventory
        bool allItemsFound = true;
        foreach (Item itemToCheck in itemsToCheck)
        {
            if (!InventoryManager.instance.HasItem(itemToCheck))
            {
                Debug.LogWarning("Item to check not found in inventory: " + itemToCheck.itemName);
                allItemsFound = false;
                break;
            }
        }

        // If all items are found, add the corresponding items to the inventory
        if (allItemsFound)
        {
            foreach (Item itemToAdd in itemsToAdd)
            {
                InventoryManager.instance.AddItem(itemToAdd);
                InventoryUI.instance.ShowPickupDisplay(itemToAdd);
            }
            Debug.Log("All items checked and items added.");
        }
    }
}
