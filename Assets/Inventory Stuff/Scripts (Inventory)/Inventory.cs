using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadInventory(); // Load inventory and picked up items on start
        //Clear(); //for testing
    }

    public List<Item> items = new List<Item>();
    public List<string> PickedUpItems = new List<string>(); // List to store picked up item IDs
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public bool Add(Item item)
    {
        items.Add(item);
        onItemChangedCallback?.Invoke();
        return true;
    }

    public void Remove(Item item)
    {
        items.Remove(item);
        onItemChangedCallback?.Invoke();
    }

    public void Clear()
    {
        items.Clear();
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
