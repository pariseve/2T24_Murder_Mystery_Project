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

        // check if this item has already been picked up
        if (InventoryManager.instance.PickedUpItems.Contains(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("Player interacted with item: " + item.itemName);

            //if (Inventory.instance.Add(item))
            //{
            //    Debug.Log("Item added to inventory: " + item.itemName);

            //    // Mark the item as picked up
            //    Inventory.instance.PickedUpItems.Add(uniqueID);
            //    Inventory.instance.SaveInventory(); // Save the inventory including picked up items

            //    // Show pickup text through the UIManager
            //    InventoryUI.instance.ShowPickupText("Picked up " + item.itemName + ", press [i]");

            //    Destroy(gameObject);
            //}
            //else
            //{
            //    Debug.Log("Failed to add item to inventory: " + item.itemName);
            //}

            if (InventoryManager.instance.AddItem(item))
            {
                Debug.Log("Item added to inventory: " + item.itemName);
                // Add the item to the picked up items list
                InventoryManager.instance.PickedUpItems.Add(uniqueID);
                // Show pickup text through the UIManager
                InventoryUI.instance.ShowPickupText("Picked up " + item.itemName + ", press [i]");
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
