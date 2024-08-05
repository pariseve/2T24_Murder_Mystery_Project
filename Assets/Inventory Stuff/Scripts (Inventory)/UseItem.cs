using UnityEngine;

public class UseItem : MonoBehaviour
{
    private bool isInArea = false;
    public string uniqueID;
    public void DetermineUse(Item item)
    {
        switch (item.itemType)
        {
            case ItemType.Evidence:
                Debug.Log("Using evidence item: " + item.itemName);
                EvidenceFunction();
                break;
            case ItemType.Collectable:
                Debug.Log("Using collectable item: " + item.itemName);
                break;
            case ItemType.Usable:
                Debug.Log("Using usable item: " + item.itemName);
                UsableFunction();
                break;
            case ItemType.Exchangable:
                Debug.Log("Using exchangeable item: " + item.itemName);
                ExchangeFunction(item);
                break;
            default:
                Debug.Log("Unknown item type: " + item.itemType);
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        isInArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInArea = false;
    }

    void EvidenceFunction()
    {
        // evidenceEvent.Invoke();
    }

    void UsableFunction()
    {
        // usableEvent.Invoke();
    }

    void ExchangeFunction(Item item)
    {
        // Exchange the item using the InventoryManager
        if (isInArea && InventoryManager.instance.HasItem(item.exchangedItem) && InventoryManager.instance.ExchangeItem(item))
        {
            Debug.Log("Item exchanged: " + item.itemName + " with " + item.exchangedItem.itemName);
            // Mark the item as picked up
            InventoryManager.instance.PickedUpItems.Add(uniqueID);
            // Show pickup text through the UIManager
            InventoryUI.instance.ShowPickupDisplay(item);
            // Destroy the item pickup
            Destroy(gameObject);
        }
    }
}