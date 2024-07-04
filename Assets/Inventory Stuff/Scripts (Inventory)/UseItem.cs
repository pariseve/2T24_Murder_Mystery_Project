using UnityEngine;

public class UseItem : MonoBehaviour
{
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
        InventoryManager.instance.ExchangeItem(item);
    }
}
