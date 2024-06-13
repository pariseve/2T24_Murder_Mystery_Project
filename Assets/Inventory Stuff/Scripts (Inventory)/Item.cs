using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public bool isStackable;
    public GameObject prefab;
    public ItemType itemType;
    [TextArea(15, 20)]
    public string description;
}

public enum ItemType
{
    Evidence,
    Collectable,
    Usable
}
