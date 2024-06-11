using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton

    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }

    #endregion

    // The list of items in the inventory
    public List<Item> items = new List<Item>();
    public int maxSlots = 20;

    // Delegate for item changed callback
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    // Method to add item to the inventory
    public bool Add(Item item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("Not enough room in inventory.");
            return false;
        }

        items.Add(item);
        onItemChangedCallback?.Invoke(); // Trigger the callback
        return true;
    }

    // Method to remove item from the inventory
    public void Remove(Item item)
    {
        items.Remove(item);
        onItemChangedCallback?.Invoke(); // Trigger the callback
    }
}
