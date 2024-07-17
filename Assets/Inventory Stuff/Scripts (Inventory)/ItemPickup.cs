using UnityEngine;
using System;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public string uniqueID; // Unique identifier for each item pickup

    private void Start()
    {
        // Generate a unique ID for this item pickup if it doesn't have one
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
        }

        // Check if this item has already been picked up
        if (InventoryManager.instance.PickedUpItems.Contains(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            // Check if the item is exchangeable
            if (item.itemType == ItemType.Exchangable && item.exchangedItem != null)
            {
                // Attempt to exchange the item
                if (InventoryManager.instance.HasItem(item.exchangedItem) && InventoryManager.instance.ExchangeItem(item))
                {
                    Debug.Log("Item exchanged: " + item.itemName + " with " + item.exchangedItem.itemName);
                    // Mark the item as picked up
                    InventoryManager.instance.PickedUpItems.Add(uniqueID);
                    // Show pickup text through the UIManager
                    InventoryUI.instance.ShowPickupDisplay(item);
                    // Destroy the item pickup
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Failed to exchange item: " + item.itemName);
                }
            }
            else
            {
                // Regular item pickup process
                if (InventoryManager.instance.AddItem(item))
                {
                    Debug.Log("Item added to inventory: " + item.itemName);
                    // Mark the item as picked up
                    InventoryManager.instance.PickedUpItems.Add(uniqueID);
                    // Show pickup text through the UIManager
                    InventoryUI.instance.ShowPickupDisplay(item);
                    // Destroy the item pickup
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log("Failed to add item to inventory: " + item.itemName);
                }
            }
        }
    }
}
