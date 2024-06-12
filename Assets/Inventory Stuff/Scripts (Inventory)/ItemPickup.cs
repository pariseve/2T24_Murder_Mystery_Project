using UnityEngine;
using System;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public string uniqueID; // Unique identifier for each item pickup

    private void Start()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = Guid.NewGuid().ToString();
        }

        // Check if this item has already been picked up
        if (Inventory.instance.PickedUpItems.Contains(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Player interacted with item: " + item.itemName);

            if (Inventory.instance.Add(item))
            {
                Debug.Log("Item added to inventory: " + item.itemName);

                // Mark the item as picked up
                Inventory.instance.PickedUpItems.Add(uniqueID);
                Inventory.instance.SaveInventory(); // Save the inventory including picked up items

                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Failed to add item to inventory: " + item.itemName);
            }
        }
    }
}
