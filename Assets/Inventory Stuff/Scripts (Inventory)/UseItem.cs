using UnityEngine;

public class UseItem : MonoBehaviour
{
    public void DetermineUse(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Evidence:
                // Code to handle evidence items
                Debug.Log("Using evidence item: " + item.itemName);
                break;
            case ItemType.Collectable:
                // Code to handle collectable items
                Debug.Log("Using collectable item: " + item.itemName);
                break;
            case ItemType.Usable:
                // Code to handle usable items
                Debug.Log("Using usable item: " + item.itemName);
                break;
            default:
                Debug.Log("Unknown item type: " + item.itemType);
                break;
        }
    }
}
