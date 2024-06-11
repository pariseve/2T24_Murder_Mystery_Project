using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Player interacted with item: " + item.itemName);

            // Try to add the item to the inventory
            if (Inventory.instance.Add(item))
            {
                Debug.Log("Item added to inventory: " + item.itemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Failed to add item to inventory: " + item.itemName);
            }
        }
    }
}
