using UnityEngine;

public class EvidenceSlot : MonoBehaviour
{
    private Item storedItem;
    public string SlotID;

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

    // Method to clear the slot
    public void ClearSlot()
    {
        storedItem = null;
        GetComponent<SpriteRenderer>().sprite = null;
        Debug.Log("Cleared evidence slot.");
    }

    // Method to get the stored item
    public Item GetStoredItem()
    {
        return storedItem;
    }

    public void Interact()
    {
        if (storedItem != null)
        {
            InventoryUI.instance.OpenExaminePanel(storedItem);
            Debug.Log("Opened examine panel for" + storedItem.name);
        }
    }
}
