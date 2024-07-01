using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    
    public GameObject[] inventorySlots;
    public GameObject inventoryItemPrefab;

    [Header("Inventory Settings")]
    [SerializeField] private int maxStackCount = 999;

    // Scene object management variables
    [HideInInspector] public List<string> PickedUpItems = new List<string>(); // List to store picked up item IDs

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this);
    }

    public bool AddItem(Item item)
    {
        // Iterate through slots to try and find a slot that already contains the added item
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Get a reference to the slot
            GameObject slot = inventorySlots[i];
            // Get a reference to the item in the slot
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            // Check if the slot has an item, the item is the same as the added item, the item is stackable, and the item's current stack count is less than the max stack count
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.item.isStackable == true && itemInSlot.stackCount < maxStackCount)
            {
                // Increase the stack count by 1
                itemInSlot.stackCount++;
                // Update the stack count text in the UI
                itemInSlot.RefreshCount();
                onItemChangedCallback?.Invoke();
                // Return out of the function
                return true;
            }
        }

        // Iterate through slots to try and find an empty slot
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            // Get a reference to the slot
            GameObject slot = inventorySlots[i];
            // Get a reference to the item in the slot
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            // Check if the slot has an item
            if (itemInSlot == null)
            {
                // Spawn a new item in that slot
                SpawnNewItem(item, slot);
                onItemChangedCallback?.Invoke();
                // Return out of the function
                return true;
            }
        }

        // Return false if no slot is available
        return false;
    }

    public void ClearInventory(Item item)
    {
        item = null;
        item.icon = null; // Clear the sprite
        Debug.Log("Slot cleared"); // Debug log for confirmation
    }

    private void SpawnNewItem(Item item, GameObject slot)
    {
        // Instantiate a new item at the slot's position with the inventory item prefab
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        // Get a reference to the new item's script
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        // Call Initialise Item with the item parameter passed in
        inventoryItem.InitialiseItem(item);
    }

    public Item GetSelectedItem()
    {
        //GameObject slot = inventorySlots[currentSlotIndex];
        //InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        //if (itemInSlot != null)
        //{
        //    Item item = itemInSlot.item;
        //    return item;
        //}

        Debug.Log("Called GetSelectedItem, returning null.");
        return null;
    }

    public void ConsumeItem()
    {
        //GameObject slot = inventorySlots[currentSlotIndex];
        //InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        //itemInSlot.stackCount--;
        //if (itemInSlot.stackCount <= 0)
        //{
        //    Destroy(itemInSlot.gameObject);
        //}
        //else
        //{
        //    itemInSlot.RefreshCount();
        //}
        Debug.Log("Called ConsumeItem.");
    }

    public InventoryItem GetSelectedInventoryItem()
    {
        //GameObject slot = inventorySlots[currentSlotIndex];
        //InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        //if (itemInSlot != null)
        //{
        //    return itemInSlot;
        //}

        Debug.Log("Called GetSelectedInventoryItem, returning null.");
        return null;
    }

    //-------------------------------------------------------------------
    // PLAYER PREFS
    //-------------------------------------------------------------------


    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public void Clear()
    {
        PickedUpItems.Clear(); // Clear picked up items list when inventory is cleared
        onItemChangedCallback?.Invoke();
    }

    public void SaveInventory()
    {
        PlayerPrefs.SetString("PickedUpItems", string.Join(",", PickedUpItems));
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey("PickedUpItems"))
        {
            string pickedUpItemsString = PlayerPrefs.GetString("PickedUpItems");
            PickedUpItems = new List<string>(pickedUpItemsString.Split(','));
        }
    }
}
