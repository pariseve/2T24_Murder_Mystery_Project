using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; 
    public GameObject inventoryUI; 

    Inventory inventory; 
    InventorySlot[] slots; 

    void Start()
    {
        // Get the singleton instance of the inventory
        inventory = Inventory.instance;
        // Subscribe UpdateUI to the onItemChangedCallback
        inventory.onItemChangedCallback += UpdateUI;

        // Get all the InventorySlot components in the children of itemsParent
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI()
    {
        Debug.Log("Updating UI with " + inventory.items.Count + " items");

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                Debug.Log("Adding item to slot " + i + ": " + inventory.items[i].itemName);
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                Debug.Log("Clearing slot " + i);
                slots[i].ClearSlot();
            }
        }
    }
}
