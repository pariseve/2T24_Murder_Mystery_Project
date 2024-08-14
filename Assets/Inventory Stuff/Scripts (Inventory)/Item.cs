using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public string slotID;
    public Sprite icon;
    public bool isStackable;
    public GameObject prefab;
    public ItemType itemType;
    [TextArea(15, 20)]
    public string description;
    public Item exchangedItem; //item needed for exchange
    public GameObject interactionUIPrefab;
    public Transform triggerTransform;
}

public enum ItemType
{
    Evidence,
    Collectable,
    Usable,
    Exchangable
}
