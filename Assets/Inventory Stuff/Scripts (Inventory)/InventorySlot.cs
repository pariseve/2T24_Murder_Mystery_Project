using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon; 
    private Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        Debug.Log("Item added to slot: " + item.itemName + ", Sprite: " + item.icon.name);
    }

    // Method to clear the slot
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null; // Clear the sprite
        icon.enabled = false; // Hide the icon
        Debug.Log("Slot cleared"); // Debug log for confirmation
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }
}
