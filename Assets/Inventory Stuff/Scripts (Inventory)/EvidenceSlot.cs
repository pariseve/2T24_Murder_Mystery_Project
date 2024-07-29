using UnityEngine;

public class EvidenceSlot : MonoBehaviour
{
    private Item storedItem;

    public string uniqueIDReference;

    // Method to check if the slot has an item
    public bool HasItem()
    {
        return storedItem != null;
    }

    // Method to store an item in the slot
    public void StoreItem(Item item)
    {
        storedItem = item;
        // Update the sprite or visual representation of the slot with the item's icon
        GetComponent<SpriteRenderer>().sprite = item.icon;
        Debug.Log("Stored " + item.itemName + " in evidence slot.");
    }
}
